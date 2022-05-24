using BannerlordDedicatedServer.CustomAttributes;
using BannerlordDedicatedServer.Helpers;
using BannerlordDedicatedServer.NetworkPackets.ServerPackets;
using MentalrobClient.MissionManager;
using MentalrobClient.Networking;
using MentalrobClient.NetworkPackets;
using MentalrobClient.NetworkPackets.ClientPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MentalrobClient.PacketHandlers
{
    [Handles(typeof(SpawnAgentPacket))]
    class SpawnAgentPacketHandler : IServerPacketHandler
    {
        
        public void Handle(Mission mission, IServerDataPacket packet)
        {
            SpawnAgentPacket sap = (SpawnAgentPacket)packet;

            BasicCharacterObject @object = Game.Current.ObjectManager.GetObject<BasicCharacterObject>(sap.CharacterId.Trim('\0'));
            MatrixFrame matrixFrame = MatrixFrame.Identity;
            Vec3 spawnLocation = new Vec3(sap.Location[0], sap.Location[1], sap.Location[2]);
            AgentBuildData agentBuildData = new AgentBuildData(new BasicBattleAgentOrigin(@object));
            AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(spawnLocation);
            Vec2 vec = matrixFrame.rotation.f.AsVec2;
            vec = vec.Normalized();
            agentBuildData2.InitialDirection(vec).Controller(sap.Controlled ? Agent.ControllerType.Player : Agent.ControllerType.None);
            agentBuildData.Team(mission.AttackerTeam);
            
            
            Agent agent = mission.SpawnAgent(agentBuildData2, false, 0);
            agent.Health = sap.health;
            if(sap.mainHandEquipment != EquipmentIndex.None)
            {
                agent.TryToWieldWeaponInSlot(sap.mainHandEquipment, Agent.WeaponWieldActionType.Instant, true);
            }
            if (sap.offHandEquipment != EquipmentIndex.None)
            {
                agent.TryToWieldWeaponInSlot(sap.offHandEquipment, Agent.WeaponWieldActionType.Instant, true);
            }

            if(sap.isPlayer)
            {
                MentalrobMissionController.PlayerControlledAgents[sap.agentIndex] = agent;
            }
            
            if (sap.Controlled)
            {
                Game.Current.PlayerTroop = @object;
                MentalrobMissionController.ControlledAgent = agent;
                MentalrobMissionController.controlledAgentIndex = sap.agentIndex;
                MentalrobMissionController.latestServerState = spawnLocation;
                MentalrobMissionController.latestProcessedState = spawnLocation;
                MentalrobMissionController.ControlledAgent.OnAgentWieldedItemChange += () =>
                {
                    EquipmentIndex mainIndex = MentalrobMissionController.ControlledAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
                    EquipmentIndex offIndex = MentalrobMissionController.ControlledAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
                    WieldItemPacket wip = new WieldItemPacket()
                    {
                        mainHandEquipmentIndex = mainIndex,
                        offHandEquipmentIndex = offIndex
                    };
                    CommunicatorHelper.ClientSendPacket<WieldItemPacket>(wip);
                    
                };
            }else
            {
                EntityInterpolationManager.Current.actionPacket[sap.agentIndex] = new Queue<UpdateAgentPacket>();
            }
            
        }
    }
}
