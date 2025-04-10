using System;
using System.Collections.Generic;
using System.Linq;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.DI.Scopes.Base;
using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Ui.Management
{
    public class UiManager : MonoBehaviour
    {
        private const int UseDefaultOrder = -1;
        
        [SerializeField] private UiContainer _containerPrefab;
        [SerializeField] private Transform _activeUiRoot;
        [SerializeField] private Transform _inactiveUiRoot;
        
        [Inject] private IObjectResolver _objectResolver;
        [Inject] private UiDataSo _uiDataSo;
        
        private Dictionary<Type, UiElementData> _uiElementDatas = new ();
        private readonly Dictionary<int, UiContainer> _containers = new ();
        private readonly Dictionary<Type, UiElement> _activeUiElements = new ();
        private readonly Dictionary<Type, UiElement> _inactiveUiElements = new ();

        [Inject] private void Construct()
        {
            DontDestroyOnLoad(gameObject);
            
            gameObject.SetActive(true);
            CreateElementsDatasDictionaryIfNeeded();
        }

        private void CreateElementsDatasDictionaryIfNeeded()
        {
            if (_uiElementDatas.Count != 0) return;
            
            _uiElementDatas = _uiDataSo.UiElements.ToDictionary(e => e.Prefab.GetType(), e => e);
        }

        /*public async UniTask<TResult> OpenUiElementWithResult<TElement, TResult>(int order = UseDefaultOrder)
            where TElement : AwaitableUiElement<TResult>
        {
            var uiElement = OpenUiElement(element, order);
            if (uiElement is not AwaitableUiElement<TResult> awaitableUiElement) return default;

            return await awaitableUiElement.AwaitForResult();
        }*/

        public TElement OpenUiElement<TElement>(int order = UseDefaultOrder)
            where TElement : UiElement
        {
            var type = typeof(TElement);
            var elementData =_uiElementDatas[type];
            var elementOrder = order == UseDefaultOrder ? elementData.DefaultSortingOrder : order;
            
            var uiContainer = GetOrCreateUiContainer(elementOrder);
            var elementState = GetUiElementState<TElement>();
            if (elementState == UiElementState.NotInstantiated) CreateUiElement(elementData);
            ActivateUiElement<TElement>(uiContainer);
            return GetUiElement<TElement>();
        }

        public void CloseUiElement<TElement>()
            where TElement : UiElement
            => CloseUiElement(typeof(TElement));

        public void CloseUiElement(UiElement uiElement) => CloseUiElement(uiElement.GetType());
        public void CloseUiElement(Type type)
        {
            var elementState = GetUiElementState(type);
            if (elementState != UiElementState.Active) return;
            DeactivateUiElement(type);
        }

        private UiContainer GetOrCreateUiContainer(int order)
        {
            if (_containers.TryGetValue(order, out var uiContainer)) return uiContainer;
            
            var container = Instantiate(_containerPrefab, _activeUiRoot);
            _containers.Add(order, container);
            return _containers[order];
        }

        private void CreateUiElement(UiElementData data)
        {
            var newUiElement = Instantiate(data.Prefab, _activeUiRoot);
            newUiElement.gameObject.SetActive(false);
            _inactiveUiElements.Add(data.Prefab.GetType(), newUiElement);
        }

        private void ActivateUiElement<TElement>(UiContainer container)
            where TElement : UiElement
        {
            var type = typeof(TElement);
            if (!_inactiveUiElements.TryGetValue(type, out var uiElement)) return;
            
            CurrentScopeProvider.Instance.CurrentScope.InjectGameObject(uiElement.gameObject);
            uiElement.transform.SetParent(container.transform, false);
            uiElement.SetContainer(container);
            uiElement.gameObject.SetActive(true);
            _activeUiElements.Add(type, uiElement);
            _inactiveUiElements.Remove(type);
        }

        private void DeactivateUiElement<TElement>()
            where TElement : UiElement
        {
            var type = typeof(TElement);
            DeactivateUiElement(type);
        }
        
        private void DeactivateUiElement(Type type)
        {
            if (!_activeUiElements.TryGetValue(type, out var uiElement)) return;
            
            uiElement.transform.SetParent(_inactiveUiRoot, false);
            uiElement.SetContainer(null);
            uiElement.gameObject.SetActive(false);
            _inactiveUiElements.Add(type, uiElement);
            _activeUiElements.Remove(type);
        }

        public UiElementState GetUiElementState<TElement>()
            where TElement : UiElement
            => GetUiElementState(typeof(TElement));
        
        public UiElementState GetUiElementState(Type type)
        {
            if (_activeUiElements.ContainsKey(type)) return UiElementState.Active;
            if (_inactiveUiElements.ContainsKey(type)) return UiElementState.Inactive;
            return UiElementState.NotInstantiated;
        } 
        
        public TElement GetUiElement<TElement>()
            where TElement : UiElement
        {
            var type = typeof(TElement);
            var elementState = GetUiElementState<TElement>();
            if (elementState == UiElementState.NotInstantiated) return null;
            return elementState == UiElementState.Active
                ? _activeUiElements[type] as TElement
                : _inactiveUiElements[type] as TElement;
        }
    }
}