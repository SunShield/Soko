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
        public LevelGridCell Cell { get; private set; }
        public GridCoords Position => Cell.Coords;

        public void Initialize(LevelGridCell cell)
        {
            Cell = cell;
            _componentsList = Components.ToList();
            _componentsList.ForEach(c => c.Initialize(this));
        }

        public void SetCell(LevelGridCell cell) => Cell = cell;

        public async Task OnObjectAboutToEnter(LevelObjectBase enteringObject, MovementAction movementAction)
        {
            foreach (var component in _componentsList)
            {
                await component.OnObjectAboutToEnter(enteringObject, movementAction);
                if (!movementAction.Active) break;
            }
        }
        
        public async Task OnObjectEntered(LevelObjectBase enteringObject, MovementAction movementAction)
        {
            foreach (var component in _componentsList)
            {
                await component.OnObjectAboutToEnter(enteringObject, movementAction);
            }
        }
    }
}