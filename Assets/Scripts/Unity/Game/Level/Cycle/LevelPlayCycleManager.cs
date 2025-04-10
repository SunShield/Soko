using System.Linq;
using Soko.Core.Events;
using Soko.Core.Models.Levels;
using Soko.Unity.Game.Level.Grid;
using Soko.Unity.Game.Level.Grid.Building;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Level.Metrics;
using Soko.Unity.Game.Level.Visuals;
using Soko.Unity.Game.Sounds;
using Soko.Unity.Game.Ui.Level;
using Soko.Unity.Game.Ui.Management;
using Soko.Unity.Game.Ui.Management.Wrapper;
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
            _uiManager.SimpleOpenUiElement<LevelScreenController>();
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
            var levelWinScreen = _uiManager.SimpleOpenUiElement<LevelWinScreenController>();
            levelWinScreen.OnClosed += LeaveLevel;
            levelWinScreen.SetLevelWinResults(LevelData.Name, _turnsCountTracker.TurnCount);
        }

        private void LeaveLevel()
        {
            var levelMainScreen = _uiManager.GetUiElement<LevelScreenController>();
            levelMainScreen.OnClosed -= LeaveLevel;
            
            _uiManager.CloseUiElement<LevelScreenController>();
            _levelsManager.EndCurrentLevel();
        }
        
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Test();
            }
        }

        private async void Test()
        {
            var player = LevelGrid.LevelObjects.First(o => o.HasComponent<PlayerComponent>());
            await _uiManager.StartUiElementOpenProcess<FocusObjectScreenController>()
                .ConfigureElement(new FocusData(player.gameObject, 150))
                .FinishOpeningProcess()
                .AwaitForResult();
            Debug.Log("T");
        }
#endif
    }
}