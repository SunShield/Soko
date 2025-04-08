using System.Linq;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Events;
using Soko.Core.Models.Levels;
using Soko.Unity.Game.Level.Grid;
using Soko.Unity.Game.Level.Grid.Building;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Level.Metrics;
using Soko.Unity.Game.Level.Visuals;
using Soko.Unity.Game.Sounds;
using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.Level;
using Soko.Unity.Game.Ui.Management;
using Soko.Unity.Game.Ui.Special.Focus;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Level.Cycle
{
    public class LevelPlayCycleManager : MonoBehaviour, IInitializable
    {
        [field: SerializeField] public Transform LevelRoot { get; private set; }
        [SerializeField] private LevelBackgroundManager _levelBackgroundManager;
        
        [Inject] private LevelGridBuilder _levelGridBuilder;
        [Inject] private LevelsManager _levelsManager;
        [Inject] private UiManager _uiManager;
        [Inject] private SoundsManager _soundsManager;
        [Inject] private EventBus _eventBus;
        [Inject] private LevelTurnsCountTracker _turnsCountTracker;
        
        public LevelData LevelData { get; private set; }
        public LevelGrid LevelGrid { get; private set; }

        public void Initialize()
        {
            _levelsManager.SetCycleManager(this);
            _uiManager.OpenUiElement(UiElements.LevelMainScreen);
            StartLevel();
        }

        private void StartLevel()
        {
            LevelData = _levelsManager.CurrentLevelData;
            LevelGrid = _levelGridBuilder.BuildLevelGrid(LevelRoot, LevelData);
            _soundsManager.PlayMusic(_levelsManager.CurrentLevelPack.MusicKey);
            _levelBackgroundManager.SetBackground(_levelsManager.CurrentLevelPack.LevelBackground);
        }

        public void CheckWin()
        {
            var isWin = LevelGrid.SpotComponents.All(c => c.Activated);
            if (!isWin) return;
            
            ConfirmWin();
            ShowWinLevelPopup();
        }

        private void ConfirmWin() => _levelsManager.WinCurrentLevel(_turnsCountTracker.TurnCount);

        private void ShowWinLevelPopup()
        {
            var levelWinScreen = _uiManager.OpenUiElement(UiElements.LevelWinScreen) as LevelWinScreenController;
            levelWinScreen.OnClosed += LeaveLevel;
            levelWinScreen.SetLevelWinResults(LevelData.Name, _turnsCountTracker.TurnCount);
        }

        private void LeaveLevel()
        {
            var levelMainScreen = _uiManager.GetUiElement(UiElements.LevelMainScreen);
            levelMainScreen.OnClosed -= LeaveLevel;
            
            _uiManager.CloseUiElement(UiElements.LevelMainScreen);
            _levelsManager.EndCurrentLevel();
        }
        
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                var focusScreen = _uiManager.OpenUiElement<FocusObjectScreenController>(UiElements.FocusObjectScreen);
                var player = LevelGrid.LevelObjects.First(o => o.HasComponent<PlayerComponent>());
                focusScreen.Setup(player.gameObject, 150);
            }
        }
#endif
    }
}