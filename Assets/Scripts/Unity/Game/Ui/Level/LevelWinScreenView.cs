using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelWinScreenView : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _levelNameText;
        [SerializeField] private TextMeshProUGUI _levelTurnsText;

        private void Awake()
        {
            _continueButton.onClick.AddListener(ContinueButtonClickHandler);
        }

        public void SetLevelWinResults(string levelName, int levelTurns)
        {
            _levelNameText.text = levelName;
            _levelTurnsText.text = levelTurns.ToString();
        }
        
        private void ContinueButtonClickHandler() => OnContinueButtonClicked?.Invoke();
        
        public event Action OnContinueButtonClicked;
    }
}