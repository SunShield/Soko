using System;
using Soko.Unity.Game.Level.Enums;
using UnityEngine;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelBoxController : MonoBehaviour
    {
        [field: SerializeField] public LevelBoxView View { get; private set; }

        public int Index { get; private set; }

        private void Awake() => View.OnClick += ClickHandler;
        private void ClickHandler() => OnClicked?.Invoke(Index);

        public void Setup(int index, LevelState levelState)
        {
            Index = index;
            View.SetLevelIndexText(Index);
            View.SetState(levelState);
        }
        
        public event Action<int> OnClicked;
    }
}