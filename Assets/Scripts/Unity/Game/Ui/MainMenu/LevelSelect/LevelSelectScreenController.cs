using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Soko.Core.Models.Levels;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Save.Impl.LevelsData;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelSelectScreenController : UiElement
    {
        [SerializeField] private LevelSelectScreenView _view;
        [SerializeField] private LevelPackInfoController _levelPackInfo;
        [SerializeField] private LevelPackDrawerController _levelPackDrawer;
        [SerializeField] private LevelInfoController _levelInfo;

        [Inject] private LevelPacksSo _levelPacksSo;
        [Inject] private LevelsManager _levelsManager;
        [Inject] private LevelsProgressSaveDataManager _progressSaveDataManager;
        
        private int _levelPackIndex;
        private int _levelIndex;
        private LevelPack CurrentLevelPack => _levelPacksSo.LevelPacks[_levelPackIndex].LevelPack;
        private LevelData CurrentLevelData => _levelPacksSo.LevelPacks[_levelPackIndex].LevelPack.Levels[_levelIndex];
        private List<LevelPackSaveData> PackSaveDatas => _progressSaveDataManager.SaveData.PackSaveDatas;
        private LevelPackSaveData CurrentPackSaveData 
            => PackSaveDatas.Count - 1 >= _levelPackIndex ? PackSaveDatas[_levelPackIndex] : null;
        private LevelSaveData CurrentLevelSaveData => CurrentPackSaveData?.Levels[_levelIndex];

        private void Awake()
        {
            _view.OnPreviousLevelPackClicked += SelectPreviousLevelPack;
            _view.OnNextLevelPackClicked += SelectNextLevelPack;
            _view.OnStartClicked += StartLevel;
            _view.OnBackClicked += Close;
            _levelPackDrawer.OnLevelBoxClicked += OnLevelBoxClicked;
        }

        protected override async UniTask OnEnabledAndConstructed()
        {
            SetLevelPack(_levelsManager.LevelPackIndex, _levelsManager.LevelIndex);
        }

        private void SelectPreviousLevelPack() => SetLevelPack(_levelPackIndex - 1);
        private void SelectNextLevelPack() => SetLevelPack(_levelPackIndex + 1);
        private void OnLevelBoxClicked(int levelIndex) => SetLevel(levelIndex);
        private void StartLevel() => _levelsManager.StartCurrentLevel(_levelPackIndex, _levelIndex);

        private void SetLevelPack(int levelPackIndex, int levelIndex = 0)
        {
            _levelPackIndex = levelPackIndex;
            _view.SetLevelPackButtonsState(_levelPackIndex == 0, _levelPackIndex == _levelPacksSo.LevelPacks.Count - 1);
            _levelPackInfo.SetLevelPackInfo(CurrentLevelPack, CurrentPackSaveData);
            _levelPackDrawer.SetLevelPack(_levelPackIndex, CurrentLevelPack);
            SetLevel(levelIndex);
        }

        private void SetLevel(int levelIndex)
        {
            _levelIndex = levelIndex;
            _levelInfo.SetLevel(CurrentLevelData, CurrentLevelSaveData);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _levelsManager.WinCurrentLevel(10);
                SetLevelPack(_levelsManager.LevelPackIndex);
            }
        }
#endif
    }
}