using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Unity.Game.Level.Grid.Objects.Components;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;

namespace Soko.Unity.Game.Level.Grid.Objects
{
    public class LevelObjectBase : SerializedMonoBehaviour
    {
        [field: OdinSerialize] public HashSet<LevelObjectComponent> Components { get; private set; }
        
        private List<LevelObjectComponent> _componentsList = new ();
        private HashSet<Type> _componentTypes = new ();
        public LevelGridCell Cell { get; private set; }
        public GridCoords Position => Cell.Coords;

        public void Initialize(LevelGridCell cell)
        {
            Cell = cell;
            _componentTypes = Components.Select(component => component.GetType()).ToHashSet();
            _componentsList = Components.ToList();
            _componentsList.ForEach(c => c.Initialize(this));
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
        
        public void OnObjectEntered(LevelObjectBase enteringObject, MovementAction movementAction)
        {
            foreach (var component in _componentsList)
            {
                component.OnObjectAboutToEnter(enteringObject, movementAction);
            }
        }
        
        public bool HasComponent<TComponent>()
            where TComponent : LevelObjectComponent
            => _componentTypes.Contains(typeof(TComponent));

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