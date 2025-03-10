using System;
using System.Collections.Generic;
using Soko.Core.Models.Levels;
using UnityEngine;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelPackDrawerController : MonoBehaviour
    {
        [SerializeField] private LevelPackDrawerView  _view;
        [SerializeField] private LevelBoxController _levelBoxPrefab;

        private readonly List<LevelBoxController> _levelBoxControllers = new();

        public void SetLevelPack(LevelPack levelPack)
        {
            ClearLevelBoxes();

            for (int i = 0; i < levelPack.Levels.Count; i++)
            {
                var levelData = levelPack.Levels[i];
                var levelBox = Instantiate(_levelBoxPrefab, transform);
                _levelBoxControllers.Add(levelBox);
                levelBox.Setup(i);
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