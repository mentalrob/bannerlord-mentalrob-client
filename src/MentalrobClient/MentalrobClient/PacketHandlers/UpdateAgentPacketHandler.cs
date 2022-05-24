using BannerlordDedicatedServer.CustomAttributes;
using BannerlordDedicatedServer.NetworkPackets.ServerPackets;
using MentalrobClient.Helpers;
using MentalrobClient.MissionManager;
using MentalrobClient.Networking;
using MentalrobClient.NetworkPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.Agent;

namespace MentalrobClient.PacketHandlers
{
    [Handles(typeof(UpdateAgentPacket))]
    class UpdateAgentPacketHandler : IServerPacketHandler
    {
        private void LogMessage(String s)
        {
            InformationManager.DisplayMessage(new InformationMessage(s));
        }

        internal unsafe void UnsafeTeleport(Agent agent, ref Vec3 newPosition)
        {
            UIntPtr positionPtr = (UIntPtr)typeof(Agent).GetField("_positionPointer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(agent);
            Vec3* ptr = (Vec3*)positionPtr.ToPointer();
            *ptr = newPosition;
        }

        private void HandleServerReconciliation(int tick)
        {
            MentalrobMissionController.latestProcessedState = MentalrobMissionController.latestServerState;
            int serverStateBufferIndex = tick % MentalrobMissionController.BUFFER_SIZE;
            float positionError = Utils.CalculateDistance(MentalrobMissionController.latestServerState, MentalrobMissionController.clientPositions[serverStateBufferIndex]);
            if (positionError > 0.001f)
            {
                InformationManager.DisplayMessage(new InformationMessage("Reconciliation begin"));
                UnsafeTeleport(MentalrobMissionController.ControlledAgent, ref MentalrobMissionController.latestServerState);
                MentalrobMissionController.clientPositions[serverStateBufferIndex] = MentalrobMissionController.latestServerState;
            }
        }
        public void Handle(Mission mission, IServerDataPacket packet)
        {
            //   mission.FixedDeltaTime
            UpdateAgentPacket uap = (UpdateAgentPacket)packet;
            if (MentalrobMissionController.PlayerControlledAgents.ContainsKey(uap.index))
            {

                float serverX = uap.position[0];
                float serverY = uap.position[1];
                float serverZ = uap.position[2];
                Vec3 serverPosVec3 = new Vec3(serverX, serverY, serverZ);

                if (uap.index != MentalrobMissionController.controlledAgentIndex)
                {
                    uap.handleTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    UpdateAgentPacket currentPacketPrev = EntityInterpolationManager.Current.currentPacket.ContainsKey(uap.index) ? EntityInterpolationManager.Current.currentPacket[uap.index] : null;
                    EntityInterpolationManager.Current.lastPacket[uap.index] = currentPacketPrev;
                    EntityInterpolationManager.Current.currentPacket[uap.index] = uap;
                    long renderTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - (long)(mission.FixedDeltaTime);
                    EntityInterpolationManager.Current.Interpolate(MentalrobMissionController.PlayerControlledAgents[uap.index], uap.index, renderTime);
                }
                if (uap.index == MentalrobMissionController.controlledAgentIndex && MentalrobMissionController.ControlledAgent != null)
                {
                    MentalrobMissionController.latestServerState = new Vec3(serverX, serverY, serverZ);
                    MentalrobMissionController.serverPositions[uap.tick % MentalrobMissionController.BUFFER_SIZE] = MentalrobMissionController.latestServerState;
                    if (!MentalrobMissionController.latestServerState.Equals(MentalrobMissionController.latestProcessedState))
                    {
                        HandleServerReconciliation(uap.tick);
                    }
                }
            }
        }
    }
}
