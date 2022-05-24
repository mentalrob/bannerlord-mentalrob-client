using BannerlordDedicatedServer.CustomAttributes;
using BannerlordDedicatedServer.Helpers;
using BannerlordDedicatedServer.NetworkPackets;
using BannerlordDedicatedServer.NetworkPackets.ClientPackets;
using BannerlordDedicatedServer.NetworkPackets.ServerPackets;
using MentalrobClient.NetworkPackets;
using MentalrobClient.PacketHandlers;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BannerlordDedicatedServer.PacketHandlers
{
    [Handles(typeof(WelcomePacket))]
    class WelcomePacketHandler: IServerPacketHandler
    {
        public void Handle(Mission mission, IServerDataPacket packet)
        {
            WelcomePacket welcomePacket = (WelcomePacket)packet;
            InformationManager.DisplayMessage(new InformationMessage(welcomePacket.Message.Trim('\0')));
            String[] randomCommanders = new string[4] { "commander_1", "commander_2", "commander_3", "commander_4" };
            Random rand = new Random();
            RequestAgentSpawnPacket rasp = new RequestAgentSpawnPacket()
            {
                characterId = randomCommanders[rand.Next(randomCommanders.Length)],
            };
            CommunicatorHelper.ClientSendPacket<RequestAgentSpawnPacket>(rasp);
        }
    }
}
