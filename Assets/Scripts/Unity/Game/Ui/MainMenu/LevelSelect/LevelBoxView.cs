using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelBoxView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _levelIndexText;
        
        private void Awake() => _button.onClick.AddListener(ClickHandler);
        
        public void SetLevelIndexText(int levelIndex) => _levelIndexText.text = levelIndex.ToString();
        
        private void ClickHandler() => OnClick?.Invoke();
        public event Action OnClick;
    }
}