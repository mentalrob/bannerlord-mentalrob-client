using BannerlordDedicatedServer.CustomAttributes;
using BannerlordDedicatedServer.NetworkPackets.ServerPackets;
using MentalrobClient.MissionManager;
using MentalrobClient.NetworkPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace MentalrobClient.PacketHandlers
{
    [Handles(typeof(SetWieldItemPacket))]
    class SetWieldItemHandler : IServerPacketHandler
    {
        public void Handle(Mission mission, IServerDataPacket packet)
        {
            SetWieldItemPacket swip = (SetWieldItemPacket)packet;
            if (!MentalrobMissionController.PlayerControlledAgents.ContainsKey(swip.agentIndex)) return;

            Agent targetAgent = MentalrobMissionController.PlayerControlledAgents[swip.agentIndex];
            if(swip.mainHandIndex == TaleWorlds.Core.EquipmentIndex.None)
            {
                targetAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.Instant);
            }else
            {
                targetAgent.TryToWieldWeaponInSlot(swip.mainHandIndex, Agent.WeaponWieldActionType.WithAnimation, false);
            }

            if (swip.offHandIndex == TaleWorlds.Core.EquipmentIndex.None)
            {
                targetAgent.TryToSheathWeaponInHand(Agent.HandIndex.OffHand, Agent.WeaponWieldActionType.Instant);
            }
            else
            {
                targetAgent.TryToWieldWeaponInSlot(swip.offHandIndex, Agent.WeaponWieldActionType.WithAnimation, false);
            }
        }
    }
}
