using System.Collections.Generic;
using System.Linq;
using Soko.Core.Models.Levels;
using Soko.Service.Extensions;
using Soko.Unity.DataLayer.So;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Building
{
    public class LevelGridBuilder
    {
        private const float CellSize = 0.64f;
        private const char OpenSeparator = '[';
        private const char CloseSeparator = ']';
        
        [Inject] private LevelObjectsSo _levelObjectsSo;
        
        public LevelGrid BuildLevelGrid(Transform root, LevelData levelData)
        {
            var keys = GetLevelObjectKeys(levelData);
            var levelGrid = SpawnLevelGridObject(root, keys);
            SpawnLevelGridCells(levelGrid);
            SpawnLevelObjects(levelGrid, keys);
            return levelGrid;
        }

        private LevelGrid SpawnLevelGridObject(Transform root, List<List<string>> keys)
        {
            var levelGridGo = new GameObject("LevelGrid");
            var levelGrid = levelGridGo.AddComponent<LevelGrid>();
            levelGridGo.transform.SetParent(root);
            var dimensions = GetLevelDimensions(keys);
            levelGrid.Initialize(dimensions.Rows, dimensions.Columns);
            root.transform.position = new Vector3(-dimensions.Columns * CellSize / 2, 
                dimensions.Rows * CellSize / 2, 
                0);
            return levelGrid;
        }

        private void SpawnLevelGridCells(LevelGrid grid)
        {
            for (int y = 0; y < grid.Rows; y++)
            {
                for (int x = 0; x < grid.Columns; x++)
                {
                    SpawnGridCell(grid, y, x);
                }
            }
        }

        private (int Rows, int Columns) GetLevelDimensions(List<List<string>> keys)
        {
            var columns = keys.Max(list => list.Count); // longest row
            var rows = keys.Count;
            return (rows, columns);
        }

        private List<List<string>> GetLevelObjectKeys(LevelData levelData)
        {
            return levelData.LevelMap.Split('\n')
                .Select(rowString => rowString.SplitByTwoSeparators(OpenSeparator, CloseSeparator))
                .ToList();
        }

        private void SpawnGridCell(LevelGrid grid, int row, int col)
        {
            // TODO: pool here
            var cellGo = Object.Instantiate(_levelObjectsSo.Empty, grid.transform, true);
            cellGo.name = $"Cell[{row}, {col}]";
            cellGo.transform.localScale = Vector3.one;
            cellGo.transform.localPosition = new Vector3(col * CellSize, -row * CellSize, 0);
            cellGo.transform.localRotation = Quaternion.identity;
            
            var cell = cellGo.AddComponent<LevelGridCell>();
            cell.Initialize(new (row, col));
            grid.SetCell(row, col, cell);
        }

        private void SpawnLevelObjects(LevelGrid grid, List<List<string>> keys)
        {
            for (int y = 0; y < grid.Rows; y++)
            {
                for (int x = 0; x < grid.Columns; x++)
                {
                    var key = keys[y][x];
                    if (string.IsNullOrWhiteSpace(key)) continue;

                    var gridObjectPrefab = _levelObjectsSo.LevelObjects[key];
                    var cellTransform = grid[y, x].transform;
                    var gridObject = Object.Instantiate(gridObjectPrefab, cellTransform, true);
                    gridObject.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}