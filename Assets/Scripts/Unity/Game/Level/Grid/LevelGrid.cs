using UnityEngine;

namespace Soko.Unity.Game.Level.Grid
{
    public class LevelGrid : MonoBehaviour
    {
        private LevelGridCell[, ] _cells;
        
        public Vector2Int Dimensions { get; private set; }
        public int Rows => Dimensions.x;
        public int Columns => Dimensions.y;
        
        public void Initialize(int rows, int columns)
        {
            Dimensions = new Vector2Int(rows, columns);
            _cells = new LevelGridCell[rows, columns];
        }

        public void SetCell(int row, int column, LevelGridCell cell) => _cells[row, column] = cell;
        public LevelGridCell this[int row, int column] => _cells[row, column];
    }
}