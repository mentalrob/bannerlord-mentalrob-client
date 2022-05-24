using MentalrobClient.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade.View.Missions;

namespace MentalrobClient.MissionViews
{
    class TestMissionView : MissionView
    {
        public override void OnMissionScreenTick(float dt)
        {
           /* if (NetworkClient.Current.Connected)
                EntityInterpolationManager.Current.Tick(base.Mission, 120);*/
        }
    }
}
