using System.Collections.Generic;
using System.Linq;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl;
using Soko.Unity.Game.Level.Grid.Objects.Helpers;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Movement
{
    /// <summary>
    /// Some movement rules:
    ///
    /// 1. Movement is executed first by the distance from the edge object is moved towards
    /// (right edge if right direction)
    /// and them from top to bottom or from left to right
    ///
    /// 2. Movement executed per-cell per-object. So if group of objects is moving, each object, following the order
    /// from (1), is moved 1 space, and again, and again.
    ///
    /// 3. Movement of any non-player object should not cause any subsequent movements to appear. 
    /// </summary>
    public class MoveManager
    {
        [Inject] private LevelObjectMover _mover;

        private MoveAction _playerMoveAction;
        private readonly Dictionary<LevelObjectBase, MoveAction> _moveActions = new ();

        public async void ExecutePlayerMovement(LevelObjectBase player, Direction direction)
        {
            _moveActions.Clear();
            
            _playerMoveAction = CreateMoveAction(player, direction);
            var targetCell = player.Cell.GetNeighbour(direction);
            if (targetCell == null) return;

            if (!targetCell.CheckObjectEnter(player, _playerMoveAction)) return;

            var objectToMove = targetCell.Objects.FirstOrDefault(o => o.HasComponent<PlayerMovableComponent>());
            if (objectToMove != null)
            {
                ExecuteObjectMovement(player, objectToMove, direction);
                if (_moveActions[objectToMove].Destination != targetCell)
                    _mover.MoveObject(player, targetCell);
            }
            else
            {
                _mover.MoveObject(player, targetCell);
            }
            
            _moveActions.Clear();
            _playerMoveAction = null;
        }
        
        public async void ExecuteObjectMovement(LevelObjectBase player, LevelObjectBase objectToMove, Direction direction)
        {
            var boundObjects = objectToMove.GetBoundObjects();
            boundObjects.ForEach(obj => _moveActions.Add(obj, CreateMoveAction(obj, direction)));
            boundObjects = SortBoundObjects(boundObjects, direction);

            var continueMovement = true;
            var playerMoved = false;
            do
            {
                foreach (var levelObject in boundObjects)
                {
                    var moveAction = _moveActions[levelObject];
                    var canMove = levelObject.CanMove(direction, moveAction);
                    if (!canMove)
                    {
                        moveAction.Interrupted = true;
                        continue;
                    }

                    var targetCell = levelObject.GetTargetCell(direction, moveAction);
                    if (!targetCell)
                    {
                        moveAction.Interrupted = true;
                        continue;
                    }

                    var canEnterCell = targetCell.CheckObjectEnter(levelObject, moveAction);
                    if (!canEnterCell)
                    {
                        moveAction.Interrupted = true;
                        continue;
                    }
                    
                    moveAction.Path.Add(targetCell);
                }

                foreach (var levelObject in boundObjects)
                {
                    var moveAction = _moveActions[levelObject];
                    if (levelObject.Cell == moveAction.Destination) continue;
                    
                    await _mover.MoveObject(levelObject, moveAction.Destination);
                }

                continueMovement = _moveActions.Values.All(v => v.Interrupted);
                
            } while (!continueMovement);
        }

        private MoveAction CreateMoveAction(LevelObjectBase player, Direction direction)
        {
            var moveAction = new MoveAction() { StartingDirection = direction };
            moveAction.Path.Add(player.Cell);
            return moveAction;
        }
        
        private List<LevelObjectBase> SortBoundObjects(List<LevelObjectBase> objects, Direction direction)
            => direction switch
            {
                Direction.Up    => objects.OrderByDescending(o => -o.Position.Rows).ThenBy(o => o.Position.Columns).ToList(),
                Direction.Down  => objects.OrderByDescending(o =>  o.Position.Rows).ThenBy(o => o.Position.Columns).ToList(),
                Direction.Left  => objects.OrderByDescending(o => -o.Position.Columns).ThenBy(o => -o.Position.Rows).ToList(),
                Direction.Right => objects.OrderByDescending(o =>  o.Position.Columns).ThenBy(o => -o.Position.Rows).ToList(),
            };
    }
}