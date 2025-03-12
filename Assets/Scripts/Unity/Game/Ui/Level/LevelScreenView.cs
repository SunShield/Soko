using System;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelScreenView : MonoBehaviour
    {
        [SerializeField] private Button _backButton;

        private void Awake()
        {
            _backButton.onClick.AddListener(BackButtonClickHandler);
        }
        
        private void BackButtonClickHandler() => OnBackClicked?.Invoke();
        
        public event Action OnBackClicked;
    }
}