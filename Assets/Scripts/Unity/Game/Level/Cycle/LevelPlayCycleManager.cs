using System;
using System.Linq;
using Soko.Core.Models.Levels;
using Soko.Unity.Game.Level.Grid;
using Soko.Unity.Game.Level.Grid.Building;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Level.Visuals;
using Soko.Unity.Game.Sounds;
using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.Management;
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
        
        public LevelData2 LevelData { get; private set; }
        public LevelGrid LevelGrid { get; private set; }
        public int TurnCount { get; private set; }

        public void Initialize()
        {
            _levelsManager.SetCycleManager(this);
            _uiManager.OpenUiElement(UiElements.LevelMainScreen, 1);
            StartLevel();
        }

        private void StartLevel()
        {
            LevelData = _levelsManager.CurrentLevelData;
            LevelGrid = _levelGridBuilder.BuildLevelGrid(LevelRoot, LevelData);
            _soundsManager.PlayMusic(_levelsManager.CurrentLevelPack.MusicKey);
            _levelBackgroundManager.SetBackground(_levelsManager.CurrentLevelPack.LevelBackground);
        }
        
        public void AdvanceTurnCount()
        {
            TurnCount++;
            OnTurnCountChanged?.Invoke(TurnCount);
        }

        public void CheckWin()
        {
            var isWin = LevelGrid.SpotComponents.All(c => c.Activated);
            if (!isWin) return;
            
            WinLevel();
        }

        private async void WinLevel()
        {
            _uiManager.CloseUiElement(UiElements.LevelMainScreen);
            _levelsManager.WinCurrentLevel(TurnCount);
            _levelsManager.EndCurrentLevel();
        }
        
        public event Action<int> OnTurnCountChanged;
    }
}