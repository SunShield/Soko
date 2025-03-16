using System;
using System.Collections.Generic;
using Soko.Unity.Game.Level.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelBoxView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private List<TextMeshProUGUI> _levelIndexTexts;
        [SerializeField] private GameObject _passedGraphics;
        [SerializeField] private GameObject _lockedGraphics;
        [SerializeField] private GameObject _selectedBg;
        
        private void Awake() => _button.onClick.AddListener(ClickHandler);

        public void SetState(LevelState state)
        {
            _passedGraphics.SetActive(state == LevelState.Passed);
            _lockedGraphics.SetActive(state == LevelState.Locked);
            _button.interactable = state != LevelState.Locked;
        }
        
        public void SetSelected(bool selected) => _selectedBg.SetActive(selected);

        public void SetLevelIndexText(int levelIndex) => _levelIndexTexts.ForEach(t => t.text = $"{levelIndex + 1}");
        
        private void ClickHandler() => OnClick?.Invoke();
        public event Action OnClick;
    }
}