using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Unity.Game.Level.Grid.Objects.Components;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects
{
    public class LevelObjectBase : SerializedMonoBehaviour
    {
        [field: OdinSerialize] public HashSet<LevelObjectComponent> Components { get; private set; }
        
        private List<LevelObjectComponent> _componentsList = new ();
        private LevelGridCell _cell;
        public Vector2Int Position => _cell.Coords;

        public void Initialize(LevelGridCell cell)
        {
            _cell = cell;
            _componentsList = Components?.ToList();
        }

        public async Task OnObjectAboutToEnter(LevelObjectBase enteringObject)
        {
            foreach (var component in _componentsList)
            {
                await component.OnObjectAboutToEnter(enteringObject);
            }
        }
    }
}