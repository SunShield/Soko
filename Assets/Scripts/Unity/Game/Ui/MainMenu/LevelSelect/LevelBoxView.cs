using System;
using Soko.Unity.Game.Level.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelBoxView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _levelIndexText;
        [SerializeField] private GameObject _passedGraphics;
        [SerializeField] private GameObject _lockedGraphics;
        
        private void Awake() => _button.onClick.AddListener(ClickHandler);

        public void SetState(LevelState state)
        {
            _passedGraphics.SetActive(state == LevelState.Passed);
            _lockedGraphics.SetActive(state == LevelState.Locked);
            _button.interactable = state != LevelState.Locked;
        }
        
        public void SetLevelIndexText(int levelIndex) => _levelIndexText.text = levelIndex.ToString();
        
        private void ClickHandler() => OnClick?.Invoke();
        public event Action OnClick;
    }
}