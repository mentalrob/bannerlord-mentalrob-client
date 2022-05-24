using BannerlordDedicatedServer.CustomAttributes;
using MentalrobClient.MissionManager;
using MentalrobClient.NetworkPackets;
using MentalrobClient.NetworkPackets.ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace MentalrobClient.PacketHandlers
{
    [Handles(typeof(ApplyAgentHitPacket))]
    class ApplyAgentHitHandler : IServerPacketHandler
    {
        private Blow fromBytes(byte[] array)
        {
            Blow b = new Blow();
            int size = Marshal.SizeOf(b);
            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(array, 0, ptr, size);

            b = (Blow)Marshal.PtrToStructure(ptr, b.GetType());
            Marshal.FreeHGlobal(ptr);
            return b;
        }

        public void Handle(Mission mission, IServerDataPacket packet)
        {
            ApplyAgentHitPacket aahp = (ApplyAgentHitPacket)packet;
            byte[] blowData = aahp.blowData;
            Blow blow = fromBytes(blowData);

            if(MentalrobMissionController.PlayerControlledAgents.ContainsKey(aahp.victimIndex))
            {
                Agent targetAgent = MentalrobMissionController.PlayerControlledAgents[aahp.victimIndex];
                // Give blow to the agent
                MethodInfo privMethod = targetAgent.GetType().GetMethod("HandleBlow", BindingFlags.NonPublic | BindingFlags.Instance);
                privMethod.Invoke(targetAgent, new object[] { blow });
            }

        }
    }
}
