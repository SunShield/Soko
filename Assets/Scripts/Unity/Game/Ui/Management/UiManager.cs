using System;
using System.Collections.Generic;
using System.Linq;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Args;
using Soko.Core.Events.Impl.Events;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.DI.Scopes.Base;
using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.Management.Elements;
using Soko.Unity.Game.Ui.Management.Wrapper;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [Inject] private EventBus _eventBus;
        
        private Dictionary<Type, UiElementData> _uiElementDatas = new ();
        private readonly Dictionary<string, HashSet<UiElement>> _elementsPerScene = new();
        private readonly Dictionary<int, UiContainer> _containers = new ();
        private readonly Dictionary<Type, UiElement> _activeUiElements = new ();
        private readonly Dictionary<Type, UiElement> _inactiveUiElements = new ();

        [Inject] private void Construct()
        {
            DontDestroyOnLoad(gameObject);
            
            gameObject.SetActive(true);
            CreateElementsDatasDictionaryIfNeeded();
            
            _eventBus.GetEvent<PreSceneLoadedEvent>().SubscribeForGlobal(OnScenePreLoad);
        }

        private void CreateElementsDatasDictionaryIfNeeded()
        {
            if (_uiElementDatas.Count != 0) return;
            
            _uiElementDatas = _uiDataSo.UiElements.ToDictionary(e => e.Prefab.GetType(), e => e);
        }

        private void OnScenePreLoad(EmptyArgs args)
        {
            var activeSceneName = SceneManager.GetActiveScene().name;
            if (!_elementsPerScene.TryGetValue(activeSceneName, out var activeSceneElements)) return;

            var elementsToRemove = activeSceneElements.ToList();
            foreach (var activeSceneElement in elementsToRemove)
                CloseUiElement(activeSceneElement);
        }

        public TElement SimpleOpenUiElement<TElement>(int order = UseDefaultOrder)
            where TElement : UiElement
        {
            var process = StartUiElementOpenProcess<TElement>();
            return process.FinishOpeningProcess();
        }
        
        public OpeningUiElementWrapper<TElement> StartUiElementOpenProcess<TElement>(int order = UseDefaultOrder)
            where TElement : UiElement
        {
            var type = typeof(TElement);
            var elementData =_uiElementDatas[type];
            var elementOrder = GetElementOrder(order, elementData);
            var uiContainer = GetOrCreateUiContainer(elementOrder);
            var elementState = GetUiElementState<TElement>();
            CreateUiElementIfNeeded(elementState, elementData);
            var element = GetUiElement<TElement>();
            SetElementContainer(element, uiContainer);
            var wrapper = new OpeningUiElementWrapper<TElement>(this, element);
            return wrapper;
        }

        private int GetElementOrder(int order, UiElementData elementData)
            => order == UseDefaultOrder ? elementData.DefaultSortingOrder : order;

        private void CreateUiElementIfNeeded(UiElementState elementState, UiElementData elementData)
        {
            if (elementState != UiElementState.NotInstantiated) return;
            CreateUiElement(elementData);
        }

        private void SetElementContainer<TElement>(TElement element, UiContainer uiContainer) 
            where TElement : UiElement
        {
            element.transform.SetParent(uiContainer.transform, false);
            element.SetContainer(uiContainer);
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

        public void ActivateUiElement<TElement>()
            where TElement : UiElement
        {
            var type = typeof(TElement);
            if (!_inactiveUiElements.TryGetValue(type, out var uiElement)) return;
            
            CurrentScopeProvider.Instance.CurrentScope.InjectGameObject(uiElement.gameObject);
            uiElement.gameObject.SetActive(true);
            _activeUiElements.Add(type, uiElement);
            _inactiveUiElements.Remove(type);
            AddElementToActiveScene(uiElement);
        }

        private void AddElementToActiveScene(UiElement uiElement)
        {
            _elementsPerScene.TryAdd(SceneManager.GetActiveScene().name, new());
            _elementsPerScene[SceneManager.GetActiveScene().name].Add(uiElement);
        }
        
        private void DeactivateUiElement(Type type)
        {
            if (!_activeUiElements.TryGetValue(type, out var uiElement)) return;
            
            uiElement.transform.SetParent(_inactiveUiRoot, false);
            uiElement.SetContainer(null);
            uiElement.gameObject.SetActive(false);
            _inactiveUiElements.Add(type, uiElement);
            _activeUiElements.Remove(type);
            RemoveElementFromActiveScene(uiElement);
        }
        
        private void RemoveElementFromActiveScene(UiElement uiElement)
            => _elementsPerScene[SceneManager.GetActiveScene().name].Remove(uiElement);

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