using Soko.Core.Models.Levels;
using UnityEngine;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelPackInfoController : MonoBehaviour
    {
        [SerializeField] private LevelPackInfoView _view;

        public void SetLevelPackInfo(LevelPack levelPack)
        {
            // todo get completed levels from savedata
            _view.SetLevelPackInfo(levelPack.Name, 0, levelPack.Levels.Count);
        }
    }
}