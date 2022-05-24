using MentalrobClient.NetworkPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace MentalrobClient.PacketHandlers
{
    interface IServerPacketHandler
    {
        void Handle(Mission mission, IServerDataPacket packet);
    }
}
