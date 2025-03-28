using System.Collections.Generic;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid
{
    public class LevelGridCell : MonoBehaviour
    {
        public LevelGrid Grid { get; private set; }
        public Dictionary<ObjectLayer, LevelObjectBase> Objects { get; private set; } = new();
        public GridCoords Coords { get; private set; }
        
        public void Initialize(LevelGrid grid, GridCoords coords)
        {
            Grid = grid;
            Coords = coords;
        }

        public void BaseAddObject(LevelObjectBase objectBase)
        {
            objectBase.SetCell(this);
            Objects.Add(objectBase.Layer, objectBase);
            objectBase.transform.parent = transform;
            objectBase.transform.localPosition = Vector3.zero;
        }

        public void AddObject(LevelObjectBase objectBase, bool suppressEnterEvent = false)
        {
            if (objectBase.Cell != null) objectBase.Cell.RemoveObject(objectBase);
            objectBase.SetCell(this);
            Objects.Add(objectBase.Layer, objectBase);
            if (!suppressEnterEvent) objectBase.Cell.OnObjectEntered(objectBase);
            objectBase.transform.parent = transform;
        }

        public void RemoveObject(LevelObjectBase objectBase)
        {
            OnObjectLeft(objectBase);
            BaseRemoveObject(objectBase);
        }

        public void BaseRemoveObject(LevelObjectBase objectBase)
        {
            Objects.Remove(objectBase.Layer);
            objectBase.transform.parent = null;
        }

        public void OnCellObjectsSpawned()
        {
            if (Objects.Count < 2) return;
            
            Objects[ObjectLayer.Ground].OnStartWithObject(Objects[ObjectLayer.Solid]);
            Objects[ObjectLayer.Solid].OnStartWithObject(Objects[ObjectLayer.Ground]);
        }

        public void OnObjectEntered(LevelObjectBase objectBase)
        {
            var objects = new List<LevelObjectBase>(Objects.Values);
            foreach (var levelObject in objects)
                levelObject.OnObjectEntered(objectBase);
        }

        public void OnObjectLeft(LevelObjectBase leftObject)
        {
            var objects = new List<LevelObjectBase>(Objects.Values);
            foreach (var levelObject in objects)
                levelObject.OnObjectLeft(leftObject);
        }
        
        // todo: move to MoveManager, at least partially
        public bool CheckObjectEnter(LevelObjectBase enteringObject, Dictionary<LevelObjectBase, MoveAction> moveActions)
        {
            var objects = new List<LevelObjectBase>(Objects.Values);
            foreach (var cellObject in objects)
            {
                if (moveActions.TryGetValue(cellObject, out var cellObjectMoveAction))
                {
                    // object will leave cell this movement so we don't look at interactions with it
                    if (!cellObjectMoveAction.Interrupted || cellObjectMoveAction.Destination != this)
                    {
                        continue;
                    } 
                }
                
                var canEnter = cellObject.CheckObjectEnter(enteringObject);
                if (!canEnter) return false;
            }

            return true;
        }

        public LevelGridCell GetNeighbour(Direction direction) => direction switch
        {
            Direction.Up    when Coords.Rows - 1 >= 0              => Grid[Coords.Rows - 1, Coords.Columns],
            Direction.Down  when Coords.Rows + 1 < Grid.Rows       => Grid[Coords.Rows + 1, Coords.Columns],
            Direction.Left  when Coords.Columns - 1 >= 0           => Grid[Coords.Rows, Coords.Columns - 1],
            Direction.Right when Coords.Columns + 1 < Grid.Columns => Grid[Coords.Rows, Coords.Columns + 1],
            _ => null
        };
    }
}