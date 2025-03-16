using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Soko.Unity.Game.Ui.Enums;
using VContainer;

namespace Soko.Unity.Game.Ui.Management.Elements
{
    public class UiElement : SerializedMonoBehaviour
    {
        [Inject] protected UiManager UiManager;
        
        private UiContainer _container;
        private UiElements _key;
        
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
            await OnEnabledAndConstructed();
        }
        
        protected virtual async UniTask OnEnabledAndConstructed() { }
        
        public void SetKey(UiElements key) => _key = key;
        public void Initialize(UiContainer container) => _container = container;

        public void Close()
        {
            OnPreClose();
            UiManager.CloseUiElement(_key);
            OnClosed?.Invoke();
        }
        
        protected virtual void OnPreClose() { }
        
        public event Action OnClosed;
    }
}