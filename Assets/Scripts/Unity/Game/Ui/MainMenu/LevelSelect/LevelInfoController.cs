using Soko.Core.Models.Levels;
using Soko.Unity.Game.Save.Impl.LevelsData;
using UnityEngine;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelInfoController : MonoBehaviour
    {
        [SerializeField] private LevelInfoView _view;

        public void SetLevel(LevelData2 levelData, LevelSaveData levelSaveData)
        {
            var turnsCount = levelSaveData?.BestTurnsCount ?? 0;
            _view.SetLevelInfo(levelData.Name, turnsCount);
        }
    }
}