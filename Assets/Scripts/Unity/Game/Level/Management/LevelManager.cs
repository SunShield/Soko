﻿using System.Linq;
using Soko.Core.Models.Levels;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Grid;
using Soko.Unity.Game.Level.Grid.Building;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Level.Management
{
    public class LevelManager : MonoBehaviour, IInitializable
    {
        [field: SerializeField] public Transform LevelRoot { get; private set; }
        
        [Inject] private LevelPacksSo _levelPacksSo;
        [Inject] private LevelGridBuilder _levelGridBuilder;
        
        public LevelPack LevelPack { get; private set; }
        public LevelData LevelData { get; private set; }
        public LevelGrid LevelGrid { get; private set; }

        public void Initialize()
        {
        }

        public void StartLevel(string levelPackName, string levelName)
        {
            
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