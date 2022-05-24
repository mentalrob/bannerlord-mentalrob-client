using MentalrobClient.MissionManager;
using MentalrobClient.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace MentalrobClient.Screens
{
    class ServerListVM : ViewModel
    {
        private string _refreshText = "Refresh";
        private MBBindingList<ServerVM> _servers;
        private MBBindingList<TestVM> _tests;
        [DataSourceProperty]
        public string RefreshText
        {
            get => _refreshText;
            set
            {
                _refreshText = value;
                OnPropertyChanged();
            }
        }
        [DataSourceProperty]
        public MBBindingList<ServerVM> Servers
        {
            get => _servers;
            set
            {
                if (_servers != value)
                {
                    _servers = value;
                    OnPropertyChanged();
                }
            }
        }
        [DataSourceProperty]
        public MBBindingList<TestVM> Tests
        {
            get => _tests;
            set
            {
                if (_tests != value)
                {
                    _tests = value;
                    OnPropertyChanged();
                }
            }
        }

        public ServerListVM()
        {
            this.Servers = new MBBindingList<ServerVM>();
            this.Tests = new MBBindingList<TestVM>();
        }

        public void GoBack()
        {
            Game.Current.GameStateManager.PopState(0);
        }

        public void ExecuteRefresh()
        {
            this.RefreshText = "Refreshing";
            this.Servers.Clear();
            this.Servers.Add(new ServerVM("TR_Avrasya_RPG", "193.164.7.141", 15, "Hello world", 200, 10));
            this.Servers.Add(new ServerVM("localhost", "localhost", 15, "Hello world", 200, 10));
        }
        private void LogMessage(String s)
        {
            InformationManager.DisplayMessage(new InformationMessage(s));
        }

        [MissionMethod]
        public static Mission TestMission() {
            return MissionState.OpenNew("MentalrobMission", new MissionInitializerRecord("scn_warbandarena")
            {
                PlayingInCampaignMode = false,
                AtmosphereOnCampaign = null,
            }, (Mission missionController) => new MissionBehavior[]
            {
                new MissionOptionsComponent(),
                new MentalrobMissionController("localhost", 1337),
                new EquipmentControllerLeaveLogic()
            }, true, true);
        }

        public void Connect()
        {
            ServerListVM.TestMission();
        }

    }
}
