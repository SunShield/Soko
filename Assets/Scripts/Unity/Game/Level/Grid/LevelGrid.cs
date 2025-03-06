using System.Collections.Generic;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid
{
    public class LevelGrid : MonoBehaviour
    {
        private List<List<LevelGridCell>> _cells;
        
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public void Initialize(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            _cells = new List<List<LevelGridCell>>();
            for (int i = 0; i < Rows; i++)
            {
                _cells.Add(new List<LevelGridCell>());
            }
        }

        public void SetCell(int row, int column, LevelGridCell cell)
        {
            
        }
        
        public LevelGridCell this[int row, int column] => _cells[row][column];
    }
}