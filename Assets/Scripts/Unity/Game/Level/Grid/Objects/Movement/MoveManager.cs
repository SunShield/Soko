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
        
        // todo: after movements triggered while other movements are active will be introduced,
        // a need for a separate binding groups list can appear
        private LevelObjectBase _player;
        private readonly Dictionary<LevelObjectBase, MoveAction> _moveActions = new ();
        private readonly Dictionary<int, List<LevelObjectBase>> _bindingGroups = new ();

        public async void ExecutePlayerMovement(LevelObjectBase player, Direction direction)
        {
            _player = player;
            _moveActions.Clear();

            RegisterObjectToMove(player, direction);
            var targetPlayerCell = player.GetTargetCell(direction, null);
            if (targetPlayerCell == null) return;
            
            var targetObject = targetPlayerCell.Objects.FirstOrDefault(obj => obj.HasComponent<PlayerMovableComponent>());
            if (targetObject != null)
                RegisterObjectToMove(targetObject, direction);
            ExecuteObjectMovement(direction);
            
            _moveActions.Clear();
        }

        public void RegisterObjectToMove(LevelObjectBase objectToMove, Direction direction)
        {
            //var bindingGroup = objectToMove.GetObjectBindingGroup(); // later
            
            var objectsToMove = objectToMove.GetObjectBindingGroup();
            objectsToMove.Add(objectToMove);
            objectsToMove.ForEach(obj => _moveActions.Add(obj, CreateMoveAction(obj, direction)));
        }
        
        public async void ExecuteObjectMovement(Direction direction)
        {
            var movedObjects = _moveActions.Keys.ToList();
            movedObjects = SortBoundObjects(movedObjects, direction);

            var continueMovement = true;
            var playerMoved = false;
            do
            {
                foreach (var levelObject in movedObjects)
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

                    var canEnterCell = targetCell.CheckObjectEnter(levelObject, _moveActions);
                    if (!canEnterCell)
                    {
                        moveAction.Interrupted = true;
                        continue;
                    }
                }

                foreach (var levelObject in movedObjects)
                {
                    var moveAction = _moveActions[levelObject];
                    
                    if (!levelObject.CheckBoundObjectsAllowMove(_moveActions))
                    {
                        moveAction.Interrupted = true;
                        continue;
                    }
                    
                    var targetCell = levelObject.GetTargetCell(direction, moveAction);
                    if (targetCell != null)
                    {
                        var canEnterCell = targetCell.CheckObjectEnter(levelObject, _moveActions);
                        if (!canEnterCell)
                        {
                            moveAction.Interrupted = true;
                            continue;
                        }
                    }
                }

                // todo: later unbound movement from player. player is not always presented 
                if (!playerMoved && _moveActions[_player].Interrupted) return;

                foreach (var movedObject in movedObjects)
                {
                    var moveAction = _moveActions[movedObject];
                    if (moveAction.Interrupted) continue;
                    
                    var targetCell = movedObject.GetTargetCell(direction, moveAction);
                    _mover.MoveObject(movedObject, targetCell);
                    moveAction.Path.Add(targetCell);
                }
                
                playerMoved = true;
                continueMovement = _moveActions.Values.All(v => v.Interrupted);
                
            } while (!continueMovement);
            
            // todo: redo end criterion to _moveActions.Count == 0;
            // todo: make a moving player "just" an object with the same "move end" criterion as anything else
                // todo: consider adding player to a group with boxes. it can be cool 
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