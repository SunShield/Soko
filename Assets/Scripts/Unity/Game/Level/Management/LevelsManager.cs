using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Events;
using Soko.Core.Models.Levels;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Cycle;
using Soko.Unity.Game.Level.Enums;
using Soko.Unity.Game.Save.Impl.LevelsData;
using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.MainMenu;
using Soko.Unity.Game.Ui.MainMenu.LevelSelect;
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
        [Inject] private EventBus _eventBus;
        
        private Dictionary<string, LevelPack> _levelPacks = new();

        public string LevelPackKey
        {
            get => SaveData.LastPackKey;
            private set => SaveData.LastPackKey = value;
        }

        public string LevelKey
        {
            get => SaveData.LastLevelKey;
            private set => SaveData.LastLevelKey = value;
        }

        public LevelPlayCycleManager PlayCycleManager { get; private set; }

        private LevelsProgressSaveData SaveData => _progressSaveDataManager.SaveData;
        public LevelPack CurrentLevelPack => _levelPacks[LevelPackKey];
        public LevelData CurrentLevelData => CurrentLevelPack.Levels.First(lp => lp.Key == LevelKey);
        
        public void Initialize()
        {
            FetchLevelPackDatas();
            CreateStartingDataIfNeeded();
        }

        private void FetchLevelPackDatas() =>
            _levelPacks = _levelPacksSo.LevelPacks.Select(so => so.LevelPack).ToDictionary(lp => lp.Key);

        private void CreateStartingDataIfNeeded()
        {
            CreateSaveData();
            GetKeys();
            _progressSaveDataManager.Save();
        }

        private void CreateSaveData()
        {
            foreach (var levelPackKey in _levelPacks.Keys)
            {
                if (!SaveData.PackSaveDatas.ContainsKey(levelPackKey))
                    SaveData.PackSaveDatas.Add(levelPackKey, new ());
                
                var levelPack = _levelPacks[levelPackKey];
                var packSave = SaveData.PackSaveDatas[levelPackKey];
                foreach (var level in levelPack.Levels)
                {
                    if (!packSave.Levels.ContainsKey(level.Key))
                        packSave.Levels.Add(level.Key, new());
                }
            }
        }

        private void GetKeys()
        {
            string unpassedLevelKey = null;
            if (string.IsNullOrEmpty(LevelPackKey) || !_levelPacks.ContainsKey(LevelPackKey))
            {
                LevelPackKey = GetFirstLevelPackWithUnpassedLevelsKey(out unpassedLevelKey);
                if (string.IsNullOrEmpty(LevelPackKey))
                    LevelPackKey = _levelPacksSo.LevelPacks[^1].LevelPack.Key;
            }

            if (string.IsNullOrEmpty(SaveData.LastLevelKey) || 
                CurrentLevelPack.Levels.Select(lp => lp.Key).All(k => k != LevelKey) ||
                CheckLevelState(LevelPackKey, LevelKey) == LevelState.Passed)
            {
                LevelKey = unpassedLevelKey;
                if (string.IsNullOrEmpty(LevelKey))
                    LevelKey = _levelPacksSo.LevelPacks[^1].LevelPack.Levels[^1].Key;
            }
        }

        private string GetFirstLevelPackWithUnpassedLevelsKey(out string unpassedLevelKey)
        {
            unpassedLevelKey = null;
            foreach (var levelPackSo in _levelPacksSo.LevelPacks)
            {
                var levelPack = levelPackSo.LevelPack;
                unpassedLevelKey = GetFirstUnpassedLevelKey(levelPack);
                if (!string.IsNullOrEmpty(unpassedLevelKey)) return levelPack.Key; 
            }

            return null;
        }

        private string GetFirstUnpassedLevelKey(LevelPack levelPack)
        {
            foreach (var level in levelPack.Levels)
            {
                var state = CheckLevelState(levelPack.Key, level.Key);
                if (state != LevelState.Passed) return level.Key;
            }
            
            return null;
        }
        
        public LevelState CheckLevelState(string packKey, string levelKey)
        {
            if (!SaveData.PackSaveDatas.ContainsKey(packKey) || 
                !SaveData.PackSaveDatas[packKey].Levels.ContainsKey(levelKey))
                return LevelState.Missing;
                
            var packData  = SaveData.PackSaveDatas[packKey];
            var levelData = packData.Levels[levelKey];
            return levelData.BestTurnsCount > 0 ? LevelState.Passed : LevelState.Playable;
        }
        
        // this soultion is 'kinda' weird, but also kinda consistent
        // we keep level stuff isolated inside and avoid DI crap
        public void SetCycleManager(LevelPlayCycleManager levelPlayCycleManager)
            => PlayCycleManager = levelPlayCycleManager;

        public async void StartCurrentLevel(string packKey, string levelKey)
        {
            LevelPackKey = packKey;
            LevelKey = levelKey;
            _progressSaveDataManager.Save();
            _uiManager.CloseUiElement<LevelSelectScreenController>();
            _uiManager.CloseUiElement<MainMenuScreenController>();
            // TODO: add ui elements parenting
            await SceneManager.LoadSceneAsync(UnityConstants.Scenes.Level);
        }

        public void WinCurrentLevel(int bestTurnCount)
        {
            var currentPackSaveData = SaveData.PackSaveDatas[LevelPackKey];
            var currentTurnCount = currentPackSaveData.Levels[LevelKey].BestTurnsCount;
            if (currentTurnCount == 0 || currentTurnCount > bestTurnCount) 
                currentPackSaveData.Levels[LevelKey].BestTurnsCount = bestTurnCount;
            
            GetNextLevel();
            _progressSaveDataManager.Save();
            _eventBus.GetEvent<LevelWinEvent>().InvokeForGlobal(new());
        }

        private void GetNextLevel()
        {
            string unpassedLevelKey = null;
            LevelPackKey = GetFirstLevelPackWithUnpassedLevelsKey(out unpassedLevelKey);
            if (string.IsNullOrEmpty(LevelPackKey))
                LevelPackKey = _levelPacksSo.LevelPacks[^1].LevelPack.Key;
            
            LevelKey = unpassedLevelKey;
            if (string.IsNullOrEmpty(LevelKey))
                LevelKey = _levelPacksSo.LevelPacks[^1].LevelPack.Levels[^1].Key;
        }

        public async void EndCurrentLevel()
        {
            _eventBus.GetEvent<LevelPreLeaveEvent>().InvokeForGlobal(new());
            PlayCycleManager = null;
            await SceneManager.LoadSceneAsync(UnityConstants.Scenes.MainMenu);
            if (_uiManager.GetUiElementState<MainMenuScreenController>() != UiElementState.Active)
                await UniTask.WaitUntil(() => 
                    _uiManager.GetUiElementState<MainMenuScreenController>() == UiElementState.Active);
            _uiManager.SimpleOpenUiElement<LevelSelectScreenController>();
        }
    }
}