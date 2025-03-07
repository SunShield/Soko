using UnityEngine;

namespace Soko.Unity.Game.Level.Grid
{
    public class LevelGrid : MonoBehaviour
    {
        private LevelGridCell[, ] _cells;
        
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public void Initialize(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            _cells = new LevelGridCell[Rows, Columns];
        }

        public void SetCell(int row, int column, LevelGridCell cell) => _cells[row, column] = cell;
        public LevelGridCell this[int row, int column] => _cells[row, column];
    }
}