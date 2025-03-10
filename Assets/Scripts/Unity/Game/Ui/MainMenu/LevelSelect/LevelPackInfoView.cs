using TMPro;
using UnityEngine;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelPackInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelPackInfo;

        public void SetLevelPackInfo(string levelPackName, int completedLevelsCount, int totalLevels)
        {
            _levelPackInfo.text = $"{levelPackName} {completedLevelsCount}/{totalLevels}";
        }
    }
}