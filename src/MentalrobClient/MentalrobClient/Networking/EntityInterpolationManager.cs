using BannerlordDedicatedServer.NetworkPackets.ServerPackets;
using MentalrobClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.MountAndBlade.Agent;

namespace MentalrobClient.Networking
{
    class EntityInterpolationManager
    {
        public static EntityInterpolationManager Current { get; private set; }
        public Dictionary<int, UpdateAgentPacket> currentPacket = new Dictionary<int, UpdateAgentPacket>();
        public Dictionary<int, UpdateAgentPacket> lastPacket = new Dictionary<int, UpdateAgentPacket>();
        public Dictionary<int, Queue<UpdateAgentPacket>> actionPacket = new Dictionary<int, Queue<UpdateAgentPacket>>();

        public EntityInterpolationManager() {
            Current = this;
        }
        internal unsafe void UnsafeTeleport(Agent agent, ref Vec3 newPosition)
        {
            UIntPtr positionPtr = (UIntPtr)typeof(Agent).GetField("_positionPointer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(agent);
            Vec3* ptr = (Vec3*)positionPtr.ToPointer();
            *ptr = newPosition;
        }
        public void Interpolate(Agent targetAgent, int agentIndex, long renderTime)
        {
            if (!this.currentPacket.ContainsKey(agentIndex)) return;

            UpdateAgentPacket currentState = this.currentPacket[agentIndex];
            UpdateAgentPacket previousState = this.lastPacket.ContainsKey(agentIndex) ? this.lastPacket[agentIndex] : null;
            long t2 = currentState.handleTime;
            long t1 = previousState == null ? 0 : previousState.handleTime;

            if(renderTime <= t2 && renderTime >= t1 && previousState != null && t2 != t1 && targetAgent.IsActive())
            {
                
                targetAgent.MovementFlags = currentState.movementControlFlag;
                // Kick control
                if(currentState.eventControlFlag != EventControlFlag.Kick && (
                    targetAgent.GetCurrentActionType(0) == ActionCodeType.Kick 
                    || targetAgent.GetCurrentActionType(0) == ActionCodeType.KickContinue
                    || targetAgent.GetCurrentActionType(0) == ActionCodeType.JumpAllEnd
                    )
                )
                {
                    targetAgent.EventControlFlags &= ~(EventControlFlag.Kick);
                }
                if (currentState.eventControlFlag == EventControlFlag.Jump || currentState.actionCodeType == ActionCodeType.JumpAllBegin || currentState.actionCodeType == ActionCodeType.Jump)
                {
                    targetAgent.EventControlFlags |= EventControlFlag.Jump;
                }
                else if(currentState.eventControlFlag == EventControlFlag.Kick || currentState.actionCodeType == ActionCodeType.Kick || currentState.actionCodeType == ActionCodeType.KickContinue)
                {
                    targetAgent.EventControlFlags |= EventControlFlag.Kick;
                    Task.Delay(16).ContinueWith(t => targetAgent.EventControlFlags &= ~(EventControlFlag.Kick));

                }
                else if(currentState.eventControlFlag != 0)
                {
                    targetAgent.EventControlFlags = currentState.eventControlFlag;
                }

                // Look position, Position
                Vec3 currentServerPos = Utils.GetPositionFromUpdateAgentPacket(currentState);
                Vec3 currentServerLookPos = Utils.GetLookPositionFromUpdateAgentPacket(currentState);

                Vec3 previousServerPos = Utils.GetPositionFromUpdateAgentPacket(previousState);
                Vec3 previousServerLookPos = Utils.GetLookPositionFromUpdateAgentPacket(previousState);

                long total = t2 - t1;
                long portion = renderTime - t1;
                float ratio = portion / total;
                Vec3 teleportTo = MBMath.Lerp(previousServerPos, currentServerPos, ratio, 0.005f);
                Vec3 lookPositionTo = MBMath.Lerp(previousServerLookPos, currentServerLookPos, ratio, 0.005f);
                UnsafeTeleport(targetAgent, ref teleportTo);
                targetAgent.LookDirection = lookPositionTo;
                if(targetAgent.GetIsLeftStance() != currentState.agentLeftStance)
                {
                    if(currentState.agentLeftStance)
                    {
                        targetAgent.MovementFlags |= MovementControlFlag.StrafeLeft;
                    }
                    else
                    {
                        targetAgent.MovementFlags |= MovementControlFlag.StrafeRight;
                    }
                }
            }
            else
            {
                Vec3 serverPosVec3 = Utils.GetPositionFromUpdateAgentPacket(currentState);
                Vec3 serverLookPosVec3 = Utils.GetLookPositionFromUpdateAgentPacket(currentState);
                UnsafeTeleport(targetAgent, ref serverPosVec3);
                targetAgent.LookDirection = serverLookPosVec3;
            }
        }
    }
}
