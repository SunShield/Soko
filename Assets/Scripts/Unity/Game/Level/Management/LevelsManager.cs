using System.Collections.Generic;
using System.Linq;
using Soko.Core.Models.Levels;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Cycle;
using Soko.Unity.Game.Level.Enums;
using Soko.Unity.Game.Save.Impl.LevelsData;
using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.Management;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Level.Management
{
    public class LevelsManager : IInitializable
    {
        [Inject] private LevelPacksSo _levelPacksSo;
        [Inject] private LevelsProgressSaveDataManager _progressSaveDataManager;
        [Inject] private UiManager _uiManager;
        
        public List<LevelPack> LevelPacks = new();
        public int LevelPackIndex { get; private set; }
        public int LevelIndex { get; private set; }
        public LevelPlayCycleManager PlayCycleManager { get; private set; }

        public LevelsProgressSaveData SaveData => _progressSaveDataManager.SaveData;
        public LevelPack CurrentLevelPack => _levelPacksSo.LevelPacks[LevelPackIndex].LevelPack;
        public LevelData2 CurrentLevelData => CurrentLevelPack.Levels2[LevelIndex];
        
        public void Initialize()
        {
            FetchLevelPackDatas();
            CreateStartingDataIfNeeded();
        }

        private void FetchLevelPackDatas() => LevelPacks = _levelPacksSo.LevelPacks.Select(so => so.LevelPack).ToList();

        private void CreateStartingDataIfNeeded()
        {
            if (SaveData.PackSaveDatas.Count != 0) return;
            
            SaveData.PackSaveDatas.Add(new ());
            SaveData.PackSaveDatas[0].Levels.Add(new ());
            _progressSaveDataManager.Save();
        }
        
        public LevelState CheckLevelState(int packIndex, int levelIndex)
        {
            if (SaveData.PackSaveDatas.Count - 1 < packIndex) return LevelState.Locked;
            if (SaveData.PackSaveDatas.Count - 1 > packIndex) return LevelState.Passed;

            var levels = SaveData.PackSaveDatas[packIndex].Levels;
            if (levels.Count - 1 < levelIndex) return LevelState.Locked;
            if (levels.Count - 1 > levelIndex) return LevelState.Passed;

            // last pack and last level case
            if (packIndex == SaveData.PackSaveDatas.Count &&
                SaveData.PackSaveDatas.Count == LevelPacks.Count &&
                levelIndex == SaveData.PackSaveDatas[packIndex].Levels.Count &&
                SaveData.PackSaveDatas[packIndex].Levels[levelIndex].BestTurnsCount != 0) return LevelState.Passed;
            
            return LevelState.Playable;
        }
        
        // this soultion is 'kinda' weird, but also kinda consistens
        // we keep level stuff isolated inside and avoid DI crap
        public void SetCycleManager(LevelPlayCycleManager levelPlayCycleManager)
            => PlayCycleManager = levelPlayCycleManager;

        public async void StartCurrentLevel(int packIndex, int levelIndex)
        {
            LevelPackIndex = packIndex;
            LevelIndex = levelIndex;
            _uiManager.CloseUiElement(UiElements.LevelSelectScreen);
            _uiManager.CloseUiElement(UiElements.MainMenuScreen);
            // TODO: add ui elements parenting
            await SceneManager.LoadSceneAsync(UnityConstants.Scenes.Level);
        }

        public void WinCurrentLevel(int bestTurnCount)
        {
            var levelState = CheckLevelState(LevelPackIndex, LevelIndex);
            
            var currentPackSaveData = SaveData.PackSaveDatas[LevelPackIndex];
            var currentTurnCount = currentPackSaveData.Levels[LevelIndex].BestTurnsCount;
            if (currentTurnCount == 0 || currentTurnCount > bestTurnCount) 
                currentPackSaveData.Levels[LevelIndex].BestTurnsCount = bestTurnCount;

            if (levelState == LevelState.Playable)
                UnlockNextLevel(currentPackSaveData);
            
            _progressSaveDataManager.Save();
        }

        private void UnlockNextLevel(LevelPackSaveData currentPackSaveData)
        {
            if (currentPackSaveData.Levels.Count < CurrentLevelPack.Levels2.Count)
            {
                LevelIndex++;
                currentPackSaveData.Levels.Add(new ());
            }
            else if (LevelPackIndex < LevelPacks.Count - 1)
            {
                LevelPackIndex++;
                LevelIndex = 0;
                SaveData.PackSaveDatas.Add(new LevelPackSaveData());
                SaveData.PackSaveDatas[LevelPackIndex].Levels.Add(new ());
            }
        }

        public void EndCurrentLevel()
        {
            PlayCycleManager = null;
            SceneManager.LoadSceneAsync(UnityConstants.Scenes.MainMenu);
        }
    }
}