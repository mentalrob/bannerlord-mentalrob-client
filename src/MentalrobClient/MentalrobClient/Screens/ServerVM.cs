using MentalrobClient.MissionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace MentalrobClient.Screens
{
    class ServerVM : ViewModel
    {
        private string _name = "Server Name";
        private string _ip = "ip";
        private int _ping = 0;
        private int _maxSlot = 0;
        private string _description = "description";
        private int _playerCount = 0;



        private Action<ServerVM> _onSelection;

        public ServerVM(string name, string ip, int ping, string description, int maxSlot, int playerCount)
        {
            this.Name = name;
            this.Ip = ip;
            this.Ping = ping;
            this.Description = description;
            this.MaxSlot = maxSlot;
            this.PlayerCount = playerCount;
        }

        [DataSourceProperty]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        [DataSourceProperty]
        public string Ip
        {
            get => _ip;
            set
            {
                _ip = value;
                OnPropertyChanged();
            }
        }
        [DataSourceProperty]
        public int Ping
        {
            get => _ping;
            set
            {
                _ping = value;
                OnPropertyChanged();
            }
        }
        [DataSourceProperty]
        public int MaxSlot
        {
            get => _maxSlot;
            set
            {
                _maxSlot = value;
                OnPropertyChanged();
            }
        }
        [DataSourceProperty]
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        [DataSourceProperty]
        public int PlayerCount
        {
            get => _playerCount;
            set
            {
                _playerCount = value;
                OnPropertyChanged();
            }
        }

        [MissionMethod]
        public static Mission TestMission(String ip, int port)
        {
            return MissionState.OpenNew("MentalrobMission", new MissionInitializerRecord("scn_warbandarena")
            {
                PlayingInCampaignMode = false,
                AtmosphereOnCampaign = null,
            }, (Mission missionController) => new MissionBehavior[]
            {
                new MissionOptionsComponent(),
                new MentalrobMissionController(ip, port),
                new EquipmentControllerLeaveLogic()
            }, true, true);
        }
        public void ExecuteConnect()
        {
            InformationManager.DisplayMessage(new InformationMessage(_ip));
            ServerVM.TestMission(_ip, 1337);
        }

    }
}
