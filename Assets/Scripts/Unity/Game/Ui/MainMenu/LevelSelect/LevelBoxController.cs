using System;
using UnityEngine;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelBoxController : MonoBehaviour
    {
        [field: SerializeField] public LevelBoxView View { get; private set; }

        public int Index { get; private set; }

        private void Awake() => View.OnClick += ClickHandler;
        private void ClickHandler() => OnClicked?.Invoke(Index);

        public void Setup(int index)
        {
            Index = index;
            View.SetLevelIndexText(Index);
        }
        
        public event Action<int> OnClicked;
    }
}