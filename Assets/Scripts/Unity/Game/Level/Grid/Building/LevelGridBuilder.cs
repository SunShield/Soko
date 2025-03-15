using System.Collections.Generic;
using System.Linq;
using Soko.Core.Extensions;
using Soko.Core.Models.Levels;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Level.Grid.Building
{
    public class LevelGridBuilder
    {
        private const float CellSize = 0.64f;
        private const char OpenSeparator = '[';
        private const char CloseSeparator = ']';
        private const char DataSeparator = '|';
        private const char DataElementSeparator = ':';
        
        private const string ColorDataKey = "c";
        private const string GroupDataKey = "g";

        [Inject] private IObjectResolver _objectResolver;
        [Inject] private LevelObjectsSo _levelObjectsSo;
        [Inject] private ColorDataSo _colorDataSo;
        
        public LevelGrid BuildLevelGrid(Transform root, LevelData2 levelData)
        {
            var levelGrid = SpawnLevelGridObject(root, levelData);
            SpawnLevelGridCells(levelGrid);
            SpawnLevelObjects(levelGrid, levelData);
            return levelGrid;
        }

        private LevelGrid SpawnLevelGridObject(Transform root, LevelData2 levelData)
        {
            var levelGridGo = new GameObject("LevelGrid");
            var levelGrid = levelGridGo.AddComponent<LevelGrid>();
            levelGridGo.transform.SetParent(root);
            var dimensions = GetLevelDimensions(levelData);
            levelGrid.Initialize(dimensions.Rows, dimensions.Columns);
            CenterGrid(root, dimensions);
            return levelGrid;
        }

        private void CenterGrid(Transform grid, (int Rows, int Columns) dimensions)
        {
            grid.transform.position = new Vector3(-dimensions.Columns * CellSize / 2f + CellSize / 2, 
                dimensions.Rows * CellSize / 2f - CellSize / 2, 
                0);
        }

        private void SpawnLevelGridCells(LevelGrid grid)
        {
            for (int y = 0; y < grid.Rows; y++)
                for (int x = 0; x < grid.Columns; x++)
                    SpawnGridCell(grid, y, x);
        }

        private (int Rows, int Columns) GetLevelDimensions(LevelData2 levelData)
        {
            var columns = levelData.Cells.GetLength(0);
            var rows = levelData.Cells.GetLength(1);
            return (rows, columns);
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
            cell.Initialize(grid, new (row, col));
            grid.SetCell(row, col, cell);
        }

        private void SpawnLevelObjects(LevelGrid grid, LevelData2 levelData)
        {
            for (int y = 0; y < grid.Rows; y++)
            {
                for (int x = 0; x < grid.Columns; x++)
                {
                    var cellData = levelData.Cells[x, y];
                    if (string.IsNullOrWhiteSpace(cellData.ObjectKey)) continue;

                    var gridObject = CreateGridObject(grid, cellData.ObjectKey, y, x);

                    ProcessColoredObject(gridObject, cellData);
                    ProcessGroupedObject(gridObject, cellData);
                }
            }
            
            ConnectGroupedObjects(grid);
        }

        private LevelObjectBase CreateGridObject(LevelGrid grid, string key, int y, int x)
        {
            var gridObjectPrefab = _levelObjectsSo.LevelObjects[key];
            var cell = grid[y, x];
            var gridObject = Object.Instantiate(gridObjectPrefab, cell.transform, true);
            _objectResolver.InjectGameObject(gridObject.gameObject);
            gridObject.transform.localPosition = Vector3.zero;
            gridObject.Initialize(cell);
            cell.AddObject(gridObject);
            grid.RegisterObject(gridObject);
            return gridObject;
        }

        private void ProcessColoredObject(LevelObjectBase levelObject, CellData cellData)
        {
            if (!levelObject.TryGetComponent<ColorComponent>(out var colorComponent)) return;
           
            colorComponent.SetColor(cellData.Color);
        }

        private void ProcessGroupedObject(LevelObjectBase levelObject, CellData cellData)
        {
            if (!levelObject.TryGetComponent<GroupComponent>(out var groupComponent)) return;
            
            groupComponent.SetGroup(cellData.Group);
        }

        private void ConnectGroupedObjects(LevelGrid grid)
        {
            var groupedObjects = new Dictionary<int, List<GroupComponent>>();
            foreach (var levelObject in grid.LevelObjects)
            {
                if (!levelObject.TryGetComponent<GroupComponent>(out var groupComponent)) continue;
                if (groupComponent.Group == -1) continue;
                
                if (!groupedObjects.ContainsKey(groupComponent.Group))
                    groupedObjects.Add(groupComponent.Group, new());
                groupedObjects[groupComponent.Group].Add(groupComponent);
            }

            foreach (var groupComponents in groupedObjects.Values)
                foreach (var groupComponent in groupComponents)
                    foreach (var groupComponent2 in groupComponents)
                        groupComponent.AddObject(groupComponent2.Object);
        }
    }
}