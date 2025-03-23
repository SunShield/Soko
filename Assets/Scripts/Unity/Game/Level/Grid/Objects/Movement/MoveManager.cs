using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Soko.Unity.Game.Level.Grid.Enums;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Movement
{
    public class MoveManager
    {
        [Inject] private LevelObjectMover _mover;
        
        // todo: after movements triggered while other movements are active will be introduced,
        // a need for a separate binding groups list can appear
        private readonly Dictionary<LevelObjectBase, MoveAction> _moveActions = new ();
        private readonly Dictionary<int, List<LevelObjectBase>> _bindingGroups = new ();
        private readonly Dictionary<LevelObjectBase, (List<LevelObjectBase> objects, bool moved)> 
            _subsequentObjectsSets  = new ();
        private readonly List<Task> _objectMovementSequences = new();
        
        private readonly Dictionary<LevelObjectBase, MoveAction> _teleportActions = new ();
        private readonly Dictionary<LevelObjectBase, Action> _onTeleportActions = new ();
        private readonly List<Task> _objectTeleportSequences = new ();
        
        private readonly HashSet<LevelObjectBase> _delayedMoveObjects = new();
        
        public bool IsExecuting { get; private set; }
        
        public void RegisterObjectToMove(LevelObjectBase objectToMove, Direction direction)
        {
            if (!IsExecuting)
            {
                RegisterObjectToMoveInternal(objectToMove, direction);
                var subsequentObjects = objectToMove.GetSubsequentObjects(direction, _moveActions[objectToMove]);
                if (subsequentObjects == null) return;
            
                foreach (var subsequentObject in subsequentObjects)
                    RegisterObjectToMoveInternal(subsequentObject, direction, objectToMove);
            }
            else
            {
                _delayedMoveObjects.Add(objectToMove);
            }
        }

        public void RegisterObjectToTeleport(LevelObjectBase objectToTeleport, LevelGridCell target, Action onTeleport)
        {
            if (_teleportActions.ContainsKey(objectToTeleport)) _teleportActions.Remove(objectToTeleport);
            
            var teleportMoveAction = CreateMoveAction(objectToTeleport, Direction.None);
            teleportMoveAction.Path.Add(target);
            teleportMoveAction.IsTeleport = true;
            _teleportActions.Add(objectToTeleport, teleportMoveAction);
            _onTeleportActions.Add(objectToTeleport, onTeleport);
        }

        private void RegisterObjectToMoveInternal(LevelObjectBase objectToMove, Direction direction, 
            LevelObjectBase mainObject = null)
        {
            var objectsToMove = objectToMove.GetObjectBindingGroup();
            objectsToMove.Add(objectToMove);
            objectsToMove.ForEach(obj =>
            {
                if (_moveActions.ContainsKey(obj)) _moveActions.Remove(obj);
                
                _moveActions.Add(obj, CreateMoveAction(obj, direction));
            });
            
            RegisterBindingGroupIfNeeded(objectToMove);
            if (mainObject != null) RegisterSubsequentObjectsSet(mainObject, objectsToMove);
        }

        private void RegisterBindingGroupIfNeeded(LevelObjectBase objectToMove)
        {
            var group = objectToMove.Group; 
            if (group == -1) return;
            if (_bindingGroups.ContainsKey(group))
                _bindingGroups.Remove(group);
            
            _bindingGroups.Add(group, new () { objectToMove });
            _bindingGroups[group].AddRange(objectToMove.GetObjectBindingGroup());
        }

        private void RegisterSubsequentObjectsSet(LevelObjectBase mainObject, List<LevelObjectBase> subsequentObjects)
            => _subsequentObjectsSets.Add(mainObject, (subsequentObjects, false));
        
        public async void ExecuteObjectsMovement(Direction direction)
        {
            if (IsExecuting) return;
            IsExecuting = true;
            
            var movedObjects = _moveActions.Keys.ToList();
            movedObjects = SortBoundObjects(movedObjects, direction);

            var continueMovement = true;
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

                    if (levelObject.Group != -1)
                    {
                        var boundObjects = _bindingGroups[levelObject.Group];
                        var moveActions = boundObjects.ToDictionary(obj => obj, obj => _moveActions[obj]);
                        
                        if (!levelObject.CheckBoundObjectsAllowMove(moveActions))
                        {
                            moveAction.Interrupted = true;
                            continue;
                        }
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
                
                foreach (var levelObject in movedObjects)
                {
                    var moveAction = _moveActions[levelObject];

                    if (levelObject.Group != -1)
                    {
                        var boundObjects = _bindingGroups[levelObject.Group];
                        var moveActions = boundObjects.ToDictionary(obj => obj, obj => _moveActions[obj]);
                        
                        if (!levelObject.CheckBoundObjectsAllowMove(moveActions))
                        {
                            moveAction.Interrupted = true;
                            continue;
                        }
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

                foreach (var levelObject in movedObjects)
                {
                    var moveAction = _moveActions[levelObject];
                    
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

                // Subsequent objects movement is stopped in their main object was failed to move once
                // The most typical example of this logic is a situation when player fails to move and all subsequent
                // objects movement is interrupted
                foreach (var mainObject in _subsequentObjectsSets.Keys)
                {
                    var subsequentObjectSetData = _subsequentObjectsSets[mainObject];
                    if (subsequentObjectSetData.moved) continue;
                    
                    var moveAction = _moveActions[mainObject];
                    if (!moveAction.Interrupted) continue;
                    
                    subsequentObjectSetData.objects.ForEach(o => _moveActions[o].Interrupted = true);    
                }
                _subsequentObjectsSets.Clear();
                
                foreach (var movedObject in movedObjects)
                {
                    var moveAction = _moveActions[movedObject];
                    if (!moveAction.Started && !moveAction.Interrupted)
                    {
                        moveAction.Started = true;
                        movedObject.OnMoveStarted();
                    }
                }

                foreach (var movedObject in movedObjects)
                {
                    var moveAction = _moveActions[movedObject];
                    if (!moveAction.Finished && moveAction.Interrupted)
                    {
                        moveAction.Finished = true;
                        if (moveAction.Path.Count > 1) movedObject.OnMoveFinished();
                    }
                }

                foreach (var movedObject in movedObjects)
                {
                    var moveAction = _moveActions[movedObject];
                    if (moveAction.Finished) continue;
                    
                    var targetCell = movedObject.GetTargetCell(direction, moveAction);
                    CreateObjectMoveSequence(movedObject, targetCell);
                    moveAction.Path.Add(targetCell);
                }

                await Task.WhenAll(_objectMovementSequences);
                _objectMovementSequences.Clear();
                
                continueMovement = _moveActions.Values.All(v => v.Interrupted);
                
            } while (!continueMovement);

            await ExecuteObjectsTeleportation();
            
            _bindingGroups.Clear();
            _moveActions.Clear();
            _subsequentObjectsSets.Clear();
            
            IsExecuting = false;
        }

        private void CreateObjectMoveSequence(LevelObjectBase movedObject, LevelGridCell targetCell)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(_mover.MoveObject(movedObject, targetCell));
            sequence.AppendCallback(() => targetCell.AddObject(movedObject));
            
            _objectMovementSequences.Add(sequence.Play().AsyncWaitForCompletion());
        }

        private MoveAction CreateMoveAction(LevelObjectBase objectToMove, Direction direction)
        {
            var moveAction = new MoveAction() { StartingDirection = direction };
            moveAction.Path.Add(objectToMove.Cell);
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

        /// <summary>
        /// Teleportations occur STRICTLY after movement caused them. So after all object's final positions are
        /// determined, we can check if teleportation is possible
        /// </summary>
        private async Task ExecuteObjectsTeleportation()
        {
            var teleportedObjects = _teleportActions.Keys.ToList();
            
            foreach (var teleportedObject in teleportedObjects)
            {
                var teleportAction = _teleportActions[teleportedObject];

                var teleportTarget = teleportAction.Destination;
                if (!teleportTarget.CheckObjectEnter(teleportedObject, _teleportActions))
                {
                    teleportAction.Interrupted = true;
                    continue;
                }
            }

            foreach (var teleportedObject in teleportedObjects)
            {
                var teleportAction = _teleportActions[teleportedObject];
                if (teleportAction.Interrupted) continue;
                
                CreateTeleportSequence(teleportedObject, teleportAction.Destination);
            }

            await Task.WhenAll(_objectTeleportSequences);
            _objectTeleportSequences.Clear();
            
            _onTeleportActions.Clear();
            _teleportActions.Clear();
        }

        private void CreateTeleportSequence(LevelObjectBase teleportedObject, LevelGridCell destination)
        {
            var sequence = _mover.TeleportObject(teleportedObject, destination);
            sequence.AppendCallback(() => destination.AddObject(teleportedObject, true));
            sequence.OnComplete(() => _onTeleportActions[teleportedObject].Invoke());
            _objectTeleportSequences.Add(sequence.Play().AsyncWaitForCompletion());
        }
    }
}