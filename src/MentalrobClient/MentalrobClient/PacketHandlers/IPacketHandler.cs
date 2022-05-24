using BannerlordDedicatedServer.NetworkPackets;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.MountAndBlade;

namespace BannerlordDedicatedServer.PacketHandlers
{
    interface IPacketHandler
    {
        void Handle(Mission mission, IDataPacket packet);

    }
}
