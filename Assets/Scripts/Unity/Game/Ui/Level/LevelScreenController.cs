using System;
using Cysharp.Threading.Tasks;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Args;
using Soko.Core.Events.Impl.Events;
using Soko.Unity.Game.Level.History;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.Management;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelScreenController : UiElement
    {
        [SerializeField] private LevelScreenView _view;
        [SerializeField] private LevelTurnsCounterController _levelTurnsCounterController;
        [SerializeField] private LevelTimerController _levelTimerController;
        
        [Inject] private LevelsManager _levelsManager;
        [Inject] private UiManager _uiManager;
        [Inject] private EventBus _eventBus;
        [Inject] private ContinuousTurnReverter _continuousTurnReverter;

        protected override void PostConstruct()
        {
            _view.OnBackClicked += EndLevel;
            _view.OnRevertTurnClickStarted += StartReverting;
            _view.OnRevertTurnClickReleased += EndReverting;
            _eventBus.GetEvent<LevelWinEvent>().SubscribeForGlobal(OnLevelWin);
        }
        
        private void EndLevel()
        {
            _uiManager.CloseUiElement(UiElements.LevelMainScreen);
            _levelsManager.EndCurrentLevel();
        }

        private void StartReverting() => _continuousTurnReverter.StartReverting();
        private void EndReverting() => _continuousTurnReverter.EndReverting();

        protected override async UniTask OnEnabledAndConstructed()
            => _levelTurnsCounterController.Initialize();
        
        private void DeactivateLevelMetrics() => _levelTimerController.SetActive(false);

        private void OnLevelWin(EmptyArgs eventArgs) => DeactivateLevelMetrics();

        private void OnDisable()
        {
            _view.OnBackClicked -= EndLevel;
            _view.OnRevertTurnClickStarted -= StartReverting;
            _view.OnRevertTurnClickReleased -= EndReverting;
            _eventBus.GetEvent<LevelWinEvent>().UnsubscribeFromGlobal(OnLevelWin);
        }
    }
}