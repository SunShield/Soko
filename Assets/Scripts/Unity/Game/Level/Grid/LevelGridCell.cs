﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid
{
    public class LevelGridCell : MonoBehaviour
    {
        public LevelGrid Grid { get; private set; }
        public List<LevelObjectBase> Objects { get; private set; } = new();
        public GridCoords Coords { get; private set; }
        
        public void Initialize(LevelGrid grid, GridCoords coords)
        {
            Grid = grid;
            Coords = coords;
        }

        public void AddObject(LevelObjectBase objectBase, bool suppressEnterEvent = false)
        {
            if (objectBase.Cell != null) objectBase.Cell.RemoveObject(objectBase);
            objectBase.SetCell(this);
            Objects.Add(objectBase);
            if (!suppressEnterEvent) objectBase.Cell.OnObjectEntered(objectBase);
        }

        public void RemoveObject(LevelObjectBase objectBase)
        {
            OnObjectLeft(objectBase);
            Objects.Remove(objectBase);
        }

        public void OnObjectAboutToEnter(LevelObjectBase enteringObject, MovementAction movementAction)
        {
            var objects = new List<LevelObjectBase>(Objects);
            foreach (var levelObject in objects)
                levelObject.OnObjectAboutToEnter(enteringObject, movementAction);
        }

        public void OnObjectEntered(LevelObjectBase objectBase)
        {
            var objects = new List<LevelObjectBase>(Objects);
            foreach (var levelObject in objects)
                levelObject.OnObjectEntered(objectBase);
        }

        public void OnObjectLeft(LevelObjectBase leftObject)
        {
            var objects = new List<LevelObjectBase>(Objects);
            foreach (var levelObject in objects)
                levelObject.OnObjectLeft(leftObject);
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