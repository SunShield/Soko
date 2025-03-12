using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelPackInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelPackInfo;
        [SerializeField] private Image _levelPackImage;

        public void SetLevelPackInfo(string levelPackName, int completedLevelsCount, int totalLevels)
        {
            _levelPackInfo.text = $"{levelPackName} {completedLevelsCount}/{totalLevels}";
        }

        public void SetLevelPackImage(Sprite levelPackImage)
        {
            _levelPackImage.sprite = levelPackImage;
        }
    }
}