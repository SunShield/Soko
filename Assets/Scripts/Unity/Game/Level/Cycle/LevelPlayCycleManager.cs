using System.Linq;
using Soko.Core.Models.Levels;
using Soko.Unity.Game.Level.Grid;
using Soko.Unity.Game.Level.Grid.Building;
using Soko.Unity.Game.Level.Management;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Level.Cycle
{
    public class LevelPlayCycleManager : MonoBehaviour, IInitializable
    {
        [field: SerializeField] public Transform LevelRoot { get; private set; }
        
        [Inject] private LevelGridBuilder _levelGridBuilder;
        [Inject] private LevelsManager _levelsManager;
        
        public LevelData LevelData { get; private set; }
        public LevelGrid LevelGrid { get; private set; }

        public void Initialize()
        {
            StartLevel();
        }

        private void StartLevel()
        {
            LevelData = _levelsManager.CurrentLevelData;
            LevelGrid = _levelGridBuilder.BuildLevelGrid(LevelRoot, LevelData);
        }

        public void CheckWin()
        {
            var isWin = LevelGrid.SpotComponents.All(c => c.Activated);
            if (!isWin) return;
            
            WinLevel();
        }

        private async void WinLevel()
        {
        }
    }
}