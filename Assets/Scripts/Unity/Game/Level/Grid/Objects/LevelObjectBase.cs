using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects
{
    public abstract class LevelObjectBase : MonoBehaviour
    {
        private LevelGridCell _cell;
        public Vector2Int Position => _cell.Coords;
    }
}