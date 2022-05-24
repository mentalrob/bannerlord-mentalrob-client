using BannerlordDedicatedServer.CustomAttributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MentalrobClient.NetworkPackets.ServerPackets
{
    [ProtoContract, PacketId(5)]
    class ApplyAgentHitPacket : IServerDataPacket
    {
        [ProtoMember(1)]
        public int attackerIndex { get; set; }
        [ProtoMember(2)]
        public int victimIndex { get; set; }
        [ProtoMember(3)]
        public byte[] blowData { get; set; }
    }
}
