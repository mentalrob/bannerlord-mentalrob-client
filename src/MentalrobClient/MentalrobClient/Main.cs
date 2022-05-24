using BannerlordDedicatedServer.CustomAttributes;
using BannerlordDedicatedServer.NetworkPackets;
using BannerlordDedicatedServer.PacketHandlers;
using HarmonyLib;
using MentalrobClient.GameManager;
using MentalrobClient.HarmonyPatches;
using MentalrobClient.Networking;
using MentalrobClient.NetworkPackets;
using MentalrobClient.PacketHandlers;
using MentalrobClient.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace MentalrobClient
{
    public class Main: MBSubModuleBase
    {
        public static Harmony harmony { get; private set; }
        public static void InitializeRegistries() {
            foreach (Type networkPacketType in Assembly.GetExecutingAssembly().GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(IServerDataPacket))))
            {
                PacketIdAttribute attribute = networkPacketType.GetCustomAttribute<PacketIdAttribute>();
                NetworkPacketRegistry.storage[attribute.id] = networkPacketType;
            }
            foreach (Type packetHandlerType in Assembly.GetExecutingAssembly().GetTypes().Where(mytype => mytype.GetInterfaces().Contains(typeof(IServerPacketHandler))))
            {
                HandlesAttribute attribute = packetHandlerType.GetCustomAttribute<HandlesAttribute>();
                HandlerRegistry.storage[attribute.handles] = (IServerPacketHandler)Activator.CreateInstance(packetHandlerType);
            }
        }

        private static void LoadHarmony()
        {
            harmony = new Harmony("com.mandalrobvearkadaslari.com");
            harmony.PatchAll();
        }

        override protected void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            LoadHarmony();
            /*MethodInfo mi = AccessTools.Method(typeof(Agent), "HandleBlow");
            MethodInfo prefixMethod = SymbolExtensions.GetMethodInfo(() => AgentPatches.PrefixHandleBlow());
            harmony.Patch(mi, new HarmonyMethod(prefixMethod));*/
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(new InitialStateOption("DedicatedServerList", new TextObject("Server List"), 9990, () => {
                InitializeRegistries();
                new NetworkClient();
                MBGameManager.StartNewGame(new MentalrobGameManager());
            }, () => (false, new TextObject("Ne bu"))));
        }
    }
}
