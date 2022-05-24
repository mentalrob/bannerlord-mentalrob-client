using BannerlordDedicatedServer.CustomAttributes;
using BannerlordDedicatedServer.NetworkPackets;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MentalrobClient.NetworkPackets.ClientPackets
{
    [ProtoContract, PacketId(6)]
    class AgentHitPacket: IDataPacket
    {
        [ProtoMember(1)]
        public byte[] blowData { get; set; }
        [ProtoMember(2)]
        public int victimIndex { get; set; }
    }
}
