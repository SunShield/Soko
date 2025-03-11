using TMPro;
using UnityEngine;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelNameText;
        [SerializeField] private TextMeshProUGUI _bestResultText;

        public void SetLevelInfo(string levelName, int bestResult)
        {
            _levelNameText.text = levelName;
            _bestResultText.text = $"Best Turns: {bestResult}";
        }
    }
}