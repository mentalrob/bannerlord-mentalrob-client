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
    [Handles(typeof(PlayerDisconnectedPacket))]
    class PlayerDisconnectedHandler : IServerPacketHandler
    {
        public void Handle(Mission mission, IServerDataPacket packet)
        {
            PlayerDisconnectedPacket pdp = (PlayerDisconnectedPacket)packet;
            if(MentalrobMissionController.PlayerControlledAgents.ContainsKey(pdp.agentIndex))
            {
                Agent agent = MentalrobMissionController.PlayerControlledAgents[pdp.agentIndex];
                if(agent.IsActive())
                {
                    agent.FadeOut(true, true);
                }
                // Handle if agent has a mount here TODO

                MentalrobMissionController.PlayerControlledAgents.Remove(pdp.agentIndex);

            }
        }
    }
}
