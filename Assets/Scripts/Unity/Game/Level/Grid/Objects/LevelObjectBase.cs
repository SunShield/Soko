using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Components;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects
{
    public class LevelObjectBase : SerializedMonoBehaviour
    {
        [field: SerializeField] public string PrefabKey { get; private set; } // todo: perhaps change to enum
        [field: OdinSerialize] public HashSet<LevelObjectComponent> Components { get; private set; }
        
        private List<LevelObjectComponent> _componentsList = new ();
        private HashSet<Type> _componentTypes = new ();
        private MovementRulesComponent _movementRulesComponent;
        public LevelGridCell Cell { get; private set; }
        public GridCoords Position => Cell.Coords;

        public void Initialize(LevelGridCell cell)
        {
            Cell = cell;
            FetchComponentDatas();
            GetMovementRulesComponent();
        }

        private void FetchComponentDatas()
        {
            _componentTypes = Components.Select(component => component.GetType()).ToHashSet();
            _componentsList = Components.ToList();
            _componentsList.ForEach(c => c.Initialize(this));
        }

        private void GetMovementRulesComponent()
        {
            var moveComponents = Components.OfType<MovementRulesComponent>();
            if (moveComponents.Count() > 1)
                throw new Exception("More than one movement rules component found");
            _movementRulesComponent = Components.OfType<MovementRulesComponent>().FirstOrDefault();
        }

        public void SetCell(LevelGridCell cell) => Cell = cell;

        public void OnObjectAboutToEnter(LevelObjectBase enteringObject, MovementAction movementAction)
        {
            foreach (var component in _componentsList)
            {
                component.OnObjectAboutToEnter(enteringObject, movementAction);
                if (!movementAction.Active) break;
            }
        }
        
        public void OnObjectEntered(LevelObjectBase enteringObject)
        {
            foreach (var component in _componentsList)
                component.OnObjectEntered(enteringObject);
        }
        
        public void OnObjectLeft(LevelObjectBase leftObject)
        {
            foreach (var component in _componentsList)
                component.OnObjectLeft(leftObject);
        }
        
        public bool HasComponent<TComponent>()
            where TComponent : LevelObjectComponent
            => _componentTypes.Contains(typeof(TComponent));

        public List<LevelObjectBase> GetBoundObjects()
        {
            var boundObjects = new HashSet<LevelObjectBase> { this };

            foreach (var component in _componentsList)
            {
                var componentBoundObjects = component.GetBoundObjects();
                boundObjects = boundObjects.Union(componentBoundObjects).ToHashSet();
            }

            return boundObjects.ToList();
        }

        public bool CanMove(Direction direction, MoveAction moveAction) 
            => _movementRulesComponent.CheckCanMove(direction, moveAction); 
        public LevelGridCell GetTargetCell(Direction direction, MoveAction moveAction)
            => _movementRulesComponent.GetTargetCell(direction, moveAction);

        public bool OnObjectAboutToEnter(LevelObjectBase enteringObject, MoveAction moveAction)
        {
            foreach (var component in _componentsList)
            {
                var canEnter = component.CheckObjectEnter(enteringObject, moveAction);
                if (!canEnter) return false;
            }

            return true;
        }
        
        public bool TryGetComponent<TComponent>(out TComponent component)
            where TComponent : LevelObjectComponent
        {
            component = null;
            if (!HasComponent<TComponent>()) return false;
            component = (TComponent)_componentsList.Find(component => component.GetType() == typeof(TComponent));
            return true;
        }
    }
}