using UnityEngine;

namespace Soko.Unity.Game.Level.Grid
{
    public class LevelGridCell : MonoBehaviour
    {
        public Vector2Int Coords { get; private set; }
        
        public void Initialize(Vector2Int coords) => Coords = coords;
    }
}