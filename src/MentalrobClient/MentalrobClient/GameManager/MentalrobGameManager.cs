using MentalrobClient.GameManager.Games;
using MentalrobClient.GameManager.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace MentalrobClient.GameManager
{
    class MentalrobGameManager: MBGameManager
    {
		protected override void DoLoadingForGameManager(GameManagerLoadingSteps gameManagerLoadingStep, out GameManagerLoadingSteps nextStep)
		{
			nextStep = GameManagerLoadingSteps.None;
			switch (gameManagerLoadingStep)
			{
				case GameManagerLoadingSteps.PreInitializeZerothStep:
					MBGameManager.LoadModuleData(false);
					MBGlobals.InitializeReferences();
					Game.CreateGame(new MentalrobGame(), this).DoLoading();
					nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
					return;
				case GameManagerLoadingSteps.FirstInitializeFirstStep:
					{
						bool flag = true;
						foreach (MBSubModuleBase mbsubModuleBase in Module.CurrentModule.SubModules)
						{
							flag = (flag && mbsubModuleBase.DoLoading(Game.Current));
						}
						nextStep = (flag ? GameManagerLoadingSteps.WaitSecondStep : GameManagerLoadingSteps.FirstInitializeFirstStep);
						return;
					}
				case GameManagerLoadingSteps.WaitSecondStep:
					MBGameManager.StartNewGame();
					nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
					return;
				case GameManagerLoadingSteps.SecondInitializeThirdState:
					nextStep = (Game.Current.DoLoading() ? GameManagerLoadingSteps.PostInitializeFourthState : GameManagerLoadingSteps.SecondInitializeThirdState);
					return;
				case GameManagerLoadingSteps.PostInitializeFourthState:
					nextStep = GameManagerLoadingSteps.FinishLoadingFifthStep;
					return;
				case GameManagerLoadingSteps.FinishLoadingFifthStep:
					nextStep = GameManagerLoadingSteps.None;
					return;
				default:
					return;
			}
		}

		public override void OnLoadFinished()
		{
			base.OnLoadFinished();
			Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<DummyState>(), 0);
		}
	}
}
