using BannerlordDedicatedServer.Helpers;
using HarmonyLib;
using MentalrobClient.NetworkPackets.ClientPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace MentalrobClient.HarmonyPatches
{
    [HarmonyPatch(typeof(Agent), "RegisterBlow")]
    class AgentPatches
    {
        public static bool Prefix(Agent __instance, Blow blow)
        {
            // We are handling this part, so disable this function
            return false;
        }
    }
}
