using System;
using Soko.Core.Models.Levels;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelSelectScreenController : UiElement
    {
        [SerializeField] private LevelSelectScreenView _view;
        [SerializeField] private LevelPackInfoController _levelPackInfo;

        [Inject] private LevelPacksSo _levelPacksSo;
        
        private int _levelPackIndex;
        private int _levelIndex;
        private LevelPack CurrentLevelPack => _levelPacksSo.LevelPacks[_levelPackIndex].LevelPack;
        private LevelData CurrentLevelData => _levelPacksSo.LevelPacks[_levelPackIndex].LevelPack.Levels[_levelIndex];

        private void Awake()
        {
            _view.OnPreviousLevelPackClicked += SelectPreviousLevelPack;
            _view.OnNextLevelPackClicked += SelectNextLevelPack;
        }

        private void OnEnable()
        {
            SetLevelPack(0);
        }

        private void SelectPreviousLevelPack() => SetLevelPack(_levelPackIndex - 1);
        private void SelectNextLevelPack() => SetLevelPack(_levelPackIndex + 1);

        private void SetLevelPack(int levelPackIndex)
        {
            _levelPackIndex = levelPackIndex;
            _levelIndex = 0;
            _view.SetLevelPackButtonsState(_levelPackIndex > 0, _levelPackIndex < _levelPacksSo.LevelPacks.Count - 1);
            _levelPackInfo.SetLevelPackInfo(CurrentLevelPack);
        }
    }
}