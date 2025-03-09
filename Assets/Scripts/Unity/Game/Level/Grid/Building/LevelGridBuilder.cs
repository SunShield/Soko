using System.Collections.Generic;
using System.Linq;
using Soko.Core.Models.Levels;
using Soko.Service.Extensions;
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
            cell.Initialize(grid, new (row, col));
            grid.SetCell(row, col, cell);
        }

        private void SpawnLevelObjects(LevelGrid grid, List<List<string>> keys)
        {
            for (int y = 0; y < grid.Rows; y++)
            {
                for (int x = 0; x < grid.Columns; x++)
                {
                    var key = keys[y][x];
                    var data = new Dictionary<string, string>();
                    if (string.IsNullOrWhiteSpace(key)) continue;
                    var dataSeparatorPosition = key.IndexOf(DataSeparator);
                    if (dataSeparatorPosition > 0) // has special data
                    {
                        var keyData = key.Split(DataSeparator);
                        key = keyData[0];

                        for (int i = 1; i < keyData.Length; i++)
                        {
                            var dataElement = keyData[i];
                            var dataSplit = dataElement.Split(DataElementSeparator);
                            data.Add(dataSplit[0], dataSplit[1]);
                        }
                    }
                    
                    var gridObjectPrefab = _levelObjectsSo.LevelObjects[key];
                    var cell = grid[y, x];
                    var gridObject = Object.Instantiate(gridObjectPrefab, cell.transform, true);
                    _objectResolver.InjectGameObject(gridObject.gameObject);
                    gridObject.transform.localPosition = Vector3.zero;
                    gridObject.Initialize(cell);
                    cell.AddObject(gridObject);
                    grid.RegisterObject(gridObject);
                    
                    ProcessColoredObject(gridObject, data);
                    ProcessGroupedObject(gridObject, data);
                }
            }
            
            ConnectGroupedObjects(grid);
        }

        private void ProcessColoredObject(LevelObjectBase levelObject, Dictionary<string, string> specialData)
        {
            if (!levelObject.TryGetComponent<ColorComponent>(out var colorComponent)) return;

            var color = ObjectColor.None;
            if (specialData.TryGetValue(ColorDataKey, out var colorData))
                color = _colorDataSo.Colors[colorData].Color;
           
            colorComponent.SetColor(color);
        }

        private void ProcessGroupedObject(LevelObjectBase levelObject, Dictionary<string, string> specialData)
        {
            if (!levelObject.TryGetComponent<GroupComponent>(out var groupComponent)) return;

            var group = -1;
            if (specialData.TryGetValue(GroupDataKey, out var _))
                group = int.Parse(specialData[GroupDataKey]);
            
            groupComponent.SetGroup(group);
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
            {
                foreach (var groupComponent in groupComponents)
                {
                    foreach (var groupComponent2 in groupComponents)
                    {
                        groupComponent.AddObject(groupComponent2.Object);
                    }
                }
            }
        }
    }
}