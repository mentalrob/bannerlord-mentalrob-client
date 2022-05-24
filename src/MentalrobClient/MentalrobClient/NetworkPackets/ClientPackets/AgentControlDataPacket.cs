using BannerlordDedicatedServer.CustomAttributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TaleWorlds.MountAndBlade.Agent;

namespace MentalrobClient.NetworkPackets.ClientPackets
{
    [ProtoContract, PacketId(5)]
    class AgentControlDataPacket
    {
        [ProtoMember(1)]
        public EventControlFlag eventControlFlag { get; set; }
        [ProtoMember(2)]
        public MovementControlFlag movementControlFlag { get; set; }
    }
}
