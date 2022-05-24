using MentalrobClient.GameManager.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.View.Screen;
namespace MentalrobClient.Screens
{
    [GameStateScreen(typeof(DummyState))]
    class ServerListScreen : ScreenBase, IGameStateListener
    {
        private ServerListVM _dataSource;
        private GauntletLayer _gauntletLayer;
        private GauntletMovie _movie;
        private DummyState _state;
        private int _isFirstFrameCounter;

        public ServerListScreen(DummyState dummyState)
        {
            _state = dummyState;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this._dataSource = new ServerListVM();
            this._gauntletLayer = new GauntletLayer(100)
            {
                IsFocusLayer = true
            };
            AddLayer(_gauntletLayer);
            this._gauntletLayer.InputRestrictions.SetInputRestrictions();

            this._movie = (GauntletMovie)_gauntletLayer.LoadMovie("ServerList", this._dataSource);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this._isFirstFrameCounter = 2;
            ScreenManager.TrySetFocus(_gauntletLayer);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            _gauntletLayer.IsFocusLayer = false;
            ScreenManager.TryLoseFocus(_gauntletLayer);
        }

        protected override void OnFinalize()
        {
            base.OnFinalize();
            RemoveLayer(_gauntletLayer);
            _dataSource = null;
            _gauntletLayer = null;
        }

        protected override void OnFrameTick(float dt)
        {
            base.OnFrameTick(dt);
            if (this._isFirstFrameCounter >= 0)
            {
                if (this._isFirstFrameCounter == 0)
                {
                    LoadingWindow.DisableGlobalLoadingWindow();
                }
                else
                {
                    this._shouldTickLayersThisFrame = false;
                }
                this._isFirstFrameCounter--;
            }
        }

        void IGameStateListener.OnActivate()
        {
        }

        void IGameStateListener.OnDeactivate()
        {
        }

        void IGameStateListener.OnInitialize()
        {
        }

        void IGameStateListener.OnFinalize()
        {
            this._dataSource.OnFinalize();
        }
    }
}
