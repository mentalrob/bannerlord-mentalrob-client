using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.CustomBattle;
using TaleWorlds.ObjectSystem;

namespace MentalrobClient.GameManager.Games
{
    class MentalrobGame : GameType
    {

        public static MentalrobGame Current
        {
            get
            {
                return Game.Current.GameType as MentalrobGame;
            }
        }
        public override void OnDestroy()
        {
            
        }

        public override void OnStateChanged(GameState oldState)
        {
           
        }

        protected override void BeforeRegisterTypes(MBObjectManager objectManager)
        {
            
        }

        protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState)
        {
            nextState = GameTypeLoadingStates.None;
            switch (gameTypeLoadingState)
            {
                case GameTypeLoadingStates.InitializeFirstStep:
                    base.CurrentGame.Initialize();
                    nextState = GameTypeLoadingStates.WaitSecondStep;
                    return;
                case GameTypeLoadingStates.WaitSecondStep:
                    nextState = GameTypeLoadingStates.LoadVisualsThirdState;
                    return;
                case GameTypeLoadingStates.LoadVisualsThirdState:
                    nextState = GameTypeLoadingStates.PostInitializeFourthState;
                    break;
                case GameTypeLoadingStates.PostInitializeFourthState:
                    break;
                default:
                    return;
            }
        }

        private void InitializeGameModels(IGameStarter basicGameStarter)
        {
            basicGameStarter.AddModel(new MultiplayerAgentDecideKilledOrUnconsciousModel());
            basicGameStarter.AddModel(new CustomBattleAgentStatCalculateModel());
            basicGameStarter.AddModel(new DefaultMissionDifficultyModel());
            basicGameStarter.AddModel(new MultiplayerAgentApplyDamageModel());
            basicGameStarter.AddModel(new DefaultRidingModel());
            basicGameStarter.AddModel(new DefaultStrikeMagnitudeModel());
            basicGameStarter.AddModel(new DefaultDamageParticleModel());
            basicGameStarter.AddModel(new CustomBattleMoraleModel());
        }

        // Token: 0x060000D6 RID: 214 RVA: 0x000079EC File Offset: 0x00005BEC
        private void InitializeGameTexts(GameTextManager gameTextManager)
        {
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/multiplayer_strings.xml");
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/global_strings.xml");
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/module_strings.xml");
            gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/native_strings.xml");
        }

        protected override void OnInitialize()
        {
            Game currentGame = base.CurrentGame;
            GameTextManager gameTextManager = currentGame.GameTextManager;
            this.InitializeGameTexts(gameTextManager);
            IGameStarter gameStarter = new BasicGameStarter();
            this.InitializeGameModels(gameStarter);
            base.GameManager.InitializeGameStarter(currentGame, gameStarter);
            base.GameManager.OnGameStart(base.CurrentGame, gameStarter);
            MBObjectManager objectManager = currentGame.ObjectManager;
            currentGame.SetBasicModels(gameStarter.Models);
            currentGame.CreateGameManager();
            base.GameManager.BeginGameStart(base.CurrentGame);
            base.CurrentGame.SetRandomGenerators();
            currentGame.InitializeDefaultGameObjects();
            currentGame.LoadBasicFiles();
            currentGame.ObjectManager.LoadXML("Items", null, false);
            currentGame.ObjectManager.LoadXML("EquipmentRosters", null, false);
            currentGame.ObjectManager.LoadXML("NPCCharacters", null, false);
            currentGame.ObjectManager.LoadXML("SPCultures", null, false);
            this.LoadCustomGameXmls();
            objectManager.UnregisterNonReadyObjects();
            currentGame.SetDefaultEquipments(new Dictionary<string, Equipment>());
            objectManager.UnregisterNonReadyObjects();
            base.GameManager.OnNewCampaignStart(base.CurrentGame, null);
            base.GameManager.OnAfterCampaignStart(base.CurrentGame);
            base.GameManager.OnGameInitializationFinished(base.CurrentGame);
        }

        private void LoadCustomGameXmls()
        {
            base.ObjectManager.LoadXML("Items", null, false);
            base.ObjectManager.LoadXML("EquipmentRosters", null, false);
            base.ObjectManager.LoadXML("NPCCharacters", null, false);
            base.ObjectManager.LoadXML("SPCultures", null, false);
        }

        protected override void OnRegisterTypes(MBObjectManager objectManager)
        {
            objectManager.RegisterNonSerializedType<MBEquipmentRoster>("EquipmentRoster", "EquipmentRosters", 51U, true, false);
            objectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "NPCCharacters", 43U, true, false);
            objectManager.RegisterType<BasicCultureObject>("Culture", "SPCultures", 17U, true, false);
        }
    }
}
