using BannerlordDedicatedServer.NetworkPackets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using BannerlordDedicatedServer.CustomAttributes;
using System.Linq;
using MentalrobClient.Networking;

namespace BannerlordDedicatedServer.Helpers
{
    class CommunicatorHelper
    {
        public static void ClientSendPacket<T>(T dataPacket)
        {
            MethodInfo mi = typeof(ProtobufHelper).GetMethod("ProtoSerialize", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(dataPacket.GetType());
            byte[] content = (byte[])mi.Invoke(null, new object[] { dataPacket });
            Packet packet = new Packet
            {
                Id = dataPacket.GetType().GetCustomAttribute<PacketIdAttribute>().id,
                Data = content
            };
            // Console.WriteLine("Sended packet Content " + BitConverter.ToString(ProtobufHelper.ProtoSerialize<Packet>(packet)));
            byte[] serialized = ProtobufHelper.ProtoSerialize<Packet>(packet);
            NetworkClient.Current.Send(new ArraySegment<byte>(serialized));
        }
    }
}
