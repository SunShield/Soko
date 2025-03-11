using System.Linq;
using Soko.Core.Models.Levels;
using Soko.Unity.Game.Save.Impl.LevelsData;
using UnityEngine;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelPackInfoController : MonoBehaviour
    {
        [SerializeField] private LevelPackInfoView _view;

        public void SetLevelPackInfo(LevelPack levelPack, LevelPackSaveData levelPackSaveData)
        {
            var levelsWon = levelPackSaveData != null 
                ? levelPackSaveData.Levels.Count(l => l.BestTurnsCount != 0) 
                : 0;
            _view.SetLevelPackInfo(levelPack.Name, levelsWon, levelPack.Levels.Count);
        }
    }
}