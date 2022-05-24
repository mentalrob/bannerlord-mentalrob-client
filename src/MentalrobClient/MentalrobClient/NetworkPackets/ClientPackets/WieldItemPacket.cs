using BannerlordDedicatedServer.CustomAttributes;
using BannerlordDedicatedServer.NetworkPackets;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;

namespace MentalrobClient.NetworkPackets.ClientPackets
{
    [ProtoContract, PacketId(4)]
    class WieldItemPacket : IDataPacket
    {
        [ProtoMember(1)]
        public EquipmentIndex mainHandEquipmentIndex { get; set; }
        [ProtoMember(2)]
        public EquipmentIndex offHandEquipmentIndex { get; set; }
    }
}
