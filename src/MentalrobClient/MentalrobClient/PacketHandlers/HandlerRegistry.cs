using MentalrobClient.PacketHandlers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BannerlordDedicatedServer.PacketHandlers
{
    class HandlerRegistry
    {
        public static Dictionary<Type, IServerPacketHandler> storage = new Dictionary<Type, IServerPacketHandler>();
    }
}
