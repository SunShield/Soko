using System;
using Cysharp.Threading.Tasks;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Events;
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

        protected override void PostConstruct()
        {
            _view.OnBackClicked += EndLevel;
            _eventBus.GetEvent<LevelWinEvent>().SubscribeForGlobal(args => DeactivateLevelMetrics());
        }

        private void EndLevel()
        {
            _uiManager.CloseUiElement(UiElements.LevelMainScreen);
            _levelsManager.EndCurrentLevel();
        }

        protected override async UniTask OnEnabledAndConstructed()
            => _levelTurnsCounterController.Initialize(_levelsManager);
        
        private void DeactivateLevelMetrics() => _levelTimerController.SetActive(false);
    }
}