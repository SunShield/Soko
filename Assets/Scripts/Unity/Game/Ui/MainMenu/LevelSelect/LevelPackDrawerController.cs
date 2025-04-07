using System;
using System.Collections.Generic;
using Soko.Core.Models.Levels;
using Soko.Unity.Game.Level.Management;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelPackDrawerController : MonoBehaviour
    {
        [SerializeField] private LevelPackDrawerView  _view;
        [SerializeField] private LevelBoxController _levelBoxPrefab;

        [Inject] private LevelsManager _levelsManager;
        [Inject] private IObjectResolver _objectResolver;
        
        private readonly List<LevelBoxController> _levelBoxControllers = new();
        private int _selectedLevelIndex;
        
        private LevelBoxController CurrentLevelBoxController => _levelBoxControllers[_selectedLevelIndex];

        public void SetLevelPack(string packKey, int selectedLevelIndex, LevelPack levelPack)
        {
            ClearLevelBoxes();

            for (int i = 0; i < levelPack.Levels.Count; i++)
            {
                var levelData = levelPack.Levels[i];
                var levelBox = Instantiate(_levelBoxPrefab, transform);
                _objectResolver.InjectGameObject(levelBox.gameObject);
                _levelBoxControllers.Add(levelBox);
                var levelState = _levelsManager.CheckLevelState(packKey, levelData.Key);
                levelBox.Setup(i, levelState);
                levelBox.OnClicked += LevelBoxClickHandler;
                _view.AddLevelBox(levelBox.View);
            }
            
            _selectedLevelIndex = selectedLevelIndex;
            CurrentLevelBoxController.SetSelected(true);
        }

        private void ClearLevelBoxes()
        {
            foreach (var levelBoxController in _levelBoxControllers)
                Destroy(levelBoxController.gameObject);
            _levelBoxControllers.Clear();
            _view.ClearLevelBoxes();
        }

        private void LevelBoxClickHandler(int levelIndex)
        {
            CurrentLevelBoxController.SetSelected(false);
            _selectedLevelIndex = levelIndex;
            CurrentLevelBoxController.SetSelected(true);
            OnLevelBoxClicked?.Invoke(levelIndex);
        }

        public event Action<int> OnLevelBoxClicked;
    }
}