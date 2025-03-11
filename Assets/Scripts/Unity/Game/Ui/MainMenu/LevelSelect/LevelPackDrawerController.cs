using System;
using System.Collections.Generic;
using Soko.Core.Models.Levels;
using Soko.Unity.Game.Level.Management;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelPackDrawerController : MonoBehaviour
    {
        [SerializeField] private LevelPackDrawerView  _view;
        [SerializeField] private LevelBoxController _levelBoxPrefab;

        [Inject] private LevelsManager _levelsManager;
        
        private readonly List<LevelBoxController> _levelBoxControllers = new();

        public void SetLevelPack(int packIndex, LevelPack levelPack)
        {
            ClearLevelBoxes();

            for (int i = 0; i < levelPack.Levels.Count; i++)
            {
                var levelData = levelPack.Levels[i];
                var levelBox = Instantiate(_levelBoxPrefab, transform);
                _levelBoxControllers.Add(levelBox);
                var levelState = _levelsManager.CheckLevelState(packIndex, i);
                levelBox.Setup(i, levelState);
                levelBox.OnClicked += LevelBoxClickHandler;
                _view.AddLevelBox(levelBox.View);
            }
        }

        private void ClearLevelBoxes()
        {
            foreach (var levelBoxController in _levelBoxControllers)
                Destroy(levelBoxController.gameObject);
            _levelBoxControllers.Clear();
            _view.ClearLevelBoxes();
        }

        private void LevelBoxClickHandler(int levelIndex) => OnLevelBoxClicked?.Invoke(levelIndex);
        
        public event Action<int> OnLevelBoxClicked;
    }
}