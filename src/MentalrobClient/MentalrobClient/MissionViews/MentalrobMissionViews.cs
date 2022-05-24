using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.LegacyGUI.Missions;
using TaleWorlds.MountAndBlade.View.Missions;

namespace MentalrobClient.MissionViews
{
    [ViewCreatorModule]
    class MentalrobMissionViews
    {  
        [ViewMethod("MentalrobMission")]
        public static MissionView[] OpenMentalrobMission(Mission mission)
        {
            return new List<MissionView> {
                ViewCreator.CreateMissionAgentStatusUIHandler(mission),
                ViewCreator.CreateMissionSingleplayerEscapeMenu(false),
                ViewCreator.CreateOptionsUIHandler(),
                new TestMissionView()
        }.ToArray();
        }
    }
}
