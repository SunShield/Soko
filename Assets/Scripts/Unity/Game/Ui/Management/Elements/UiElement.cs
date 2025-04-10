using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Soko.Unity.Game.Ui.Enums;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Ui.Management.Elements
{
    public class UiElement : SerializedMonoBehaviour
    {
        [field: SerializeField] public UiElements Key { get; private set; }
        
        [Inject] protected UiManager UiManager;
        
        public UiContainer Container { get; private set; }
        
        public bool IsConstructed { get; private set; }

        [Inject]
        private void Construct()
        {
            IsConstructed = true;
            PostConstruct();
        }
        
        protected virtual void PostConstruct() { }

        private async void OnEnable()
        {
            if (!IsConstructed) await UniTask.WaitUntil(() => IsConstructed);
            OnPreEnabledAndConstructed();
            await OnEnabledAndConstructed();
        }
        
        protected virtual void OnPreEnabledAndConstructed() { }
        protected virtual async UniTask OnEnabledAndConstructed() { }
        
        public void SetContainer(UiContainer container) => Container = container;

        public void Close()
        {
            OnPreClose();
            UiManager.CloseUiElement(Key);
            OnClosed?.Invoke();
        }
        
        protected virtual void OnPreClose() { }
        
        public event Action OnClosed;
    }
}