using System.Collections.Generic;
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
        
        private readonly Dictionary<int, UiContainer> _containers = new ();
        private readonly Dictionary<UiElements, UiElement> _activeUiElements = new ();
        private readonly Dictionary<UiElements, UiElement> _inactiveUiElements = new ();

        [Inject] private void Construct()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(true);
        }

        public UiElement OpenUiElement(UiElements element, int order = UseDefaultOrder)
        {
            var elementData = _uiDataSo.UiElements[element];
            var elementOrder = order == UseDefaultOrder ? elementData.DefaultSortingOrder : order;
            
            var uiContainer = GetOrCreateUiContainer(elementOrder);
            var elementState = GetUiElementState(element);
            if (elementState == UiElementState.NotInstantiated) CreateUiElement(elementData);
            ActivateUiElement(element, uiContainer);
            return GetUiElement(element);
        }

        public void CloseUiElement(UiElements element)
        {
            var elementState = GetUiElementState(element);
            if (elementState != UiElementState.Active) return;
            DeactivateUiElement(element);
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
            _inactiveUiElements.Add(data.Prefab.Key, newUiElement);
        }

        private void ActivateUiElement(UiElements element, UiContainer container)
        {
            if (!_inactiveUiElements.TryGetValue(element, out var uiElement)) return;
            
            CurrentScopeProvider.Instance.CurrentScope.InjectGameObject(uiElement.gameObject);
            uiElement.transform.SetParent(container.transform, false);
            uiElement.Initialize(container);
            uiElement.gameObject.SetActive(true);
            _activeUiElements.Add(element, uiElement);
            _inactiveUiElements.Remove(element);
        }

        private void DeactivateUiElement(UiElements element)
        {
            if (!_activeUiElements.TryGetValue(element, out var uiElement)) return;
            
            uiElement.transform.SetParent(_inactiveUiRoot, false);
            uiElement.Initialize(null);
            uiElement.gameObject.SetActive(false);
            _inactiveUiElements.Add(element, uiElement);
            _activeUiElements.Remove(element);
        }

        public UiElementState GetUiElementState(UiElements element)
        {
            if (_activeUiElements.ContainsKey(element)) return UiElementState.Active;
            if (_inactiveUiElements.ContainsKey(element)) return UiElementState.Inactive;
            return UiElementState.NotInstantiated;
        }
        
        public UiElement GetUiElement(UiElements element)
        {
            var elementState = GetUiElementState(element);
            if (elementState == UiElementState.NotInstantiated) return null;
            return elementState == UiElementState.Active
                ? _activeUiElements[element]
                : _inactiveUiElements[element];
        }
    }
}