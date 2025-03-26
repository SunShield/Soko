using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Events;
using Soko.Core.Extensions;
using Soko.Unity.Game.Level.Grid.Enums;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Movement
{
    public class MoveManager
    {
        [Inject] private LevelObjectMover _mover;
        [Inject] private EventBus _eventBus;
        
        private readonly Dictionary<LevelObjectBase, MoveAction> _moveActions = new ();
        private readonly Dictionary<int, List<LevelObjectBase>> _bindingGroups = new ();
        private readonly Dictionary<LevelObjectBase, (List<LevelObjectBase> objects, bool moved)> 
            _subsequentObjectsSets  = new ();
        private readonly List<Task> _objectMovementSequences = new();
        
        private readonly Dictionary<LevelObjectBase, MoveAction> _teleportActions = new ();
        private readonly Dictionary<LevelObjectBase, Action> _onTeleportActions = new ();
        private readonly List<Task> _objectTeleportSequences = new ();
        
        private readonly HashSet<LevelObjectBase> _delayedMoveObjects = new();
        private Direction _delayedMoveDirection = Direction.None;

        private bool HasDelayedObjects => _delayedMoveObjects.Count > 0;
        public bool IsExecuting { get; private set; }
        
        public void RegisterObjectToMove(LevelObjectBase objectToMove, Direction direction)
        {
            if (!IsExecuting)
            {
                RegisterObjectToMoveInternal(objectToMove, direction);
                RegisterSubsequentObjectsIfNeeded(objectToMove, direction);
            }
            else
            {
                if (_delayedMoveDirection == Direction.None) _delayedMoveDirection = direction;
                // should never happen; just double-check that delayed moved objects always have one direction
                else if (_delayedMoveDirection != direction) return;
                
                _delayedMoveDirection = direction;
                _delayedMoveObjects.Add(objectToMove);
            }
        }

        private void RegisterSubsequentObjectsIfNeeded(LevelObjectBase objectToMove, Direction direction)
        {
            var subsequentObjects = objectToMove.GetSubsequentObjects(direction, _moveActions[objectToMove]);
            if (subsequentObjects == null) return;
            
            foreach (var subsequentObject in subsequentObjects)
                RegisterObjectToMoveInternal(subsequentObject, direction, objectToMove);
        }

        private void RegisterObjectToMoveInternal(LevelObjectBase objectToMove, Direction direction, 
            LevelObjectBase mainObject = null)
        {
            var objectsToMove = objectToMove.GetObjectBindingGroup();
            objectsToMove.Add(objectToMove);
            objectsToMove.ForEach(obj => AddMovementAction(direction, obj));
            
            RegisterBindingGroupIfNeeded(objectToMove);
            if (mainObject != null) RegisterSubsequentObjectsSet(mainObject, objectsToMove);
        }

        private void AddMovementAction(Direction direction, LevelObjectBase obj)
            => _moveActions.AddOrReplace(obj, CreateMoveAction(obj, direction));

        private MoveAction CreateMoveAction(LevelObjectBase objectToMove, Direction direction)
        {
            var moveAction = new MoveAction() { StartingDirection = direction };
            moveAction.Path.Add(objectToMove.Cell);
            return moveAction;
        }

        private void RegisterBindingGroupIfNeeded(LevelObjectBase objectToMove)
        {
            var group = objectToMove.Group; 
            if (group == UnityConstants.Level.NoBindingGroup) return;
            
            _bindingGroups.AddOrReplace(group, new () { objectToMove });
            _bindingGroups[group].AddRange(objectToMove.GetObjectBindingGroup());
        }

        private void RegisterSubsequentObjectsSet(LevelObjectBase mainObject, List<LevelObjectBase> subsequentObjects)
            => _subsequentObjectsSets.Add(mainObject, (subsequentObjects, false));
        
        public void RegisterObjectToTeleport(LevelObjectBase objectToTeleport, LevelGridCell target, Action onTeleport)
        {
            _teleportActions.AddOrReplace(objectToTeleport, CreateTeleportAction(objectToTeleport, target));
            _onTeleportActions.AddOrReplace(objectToTeleport, onTeleport);
        }
        
        private MoveAction CreateTeleportAction(LevelObjectBase objectToTeleport, LevelGridCell destination)
        {
            var teleportMoveAction = CreateMoveAction(objectToTeleport, Direction.None);
            teleportMoveAction.Path.Add(destination);
            teleportMoveAction.IsTeleport = true;
            return teleportMoveAction;
        }
        
        public async void ExecuteObjectsMovement(Direction direction, bool isSecondary = false)
        {
            if (IsExecuting) return;
            IsExecuting = true;
            
            if (!isSecondary) _eventBus.GetEvent<MovementStatedEvent>().InvokeForGlobal(new());
            
            var movedObjects = GetSortedMoveObjects(direction);
            do
            {
                CheckObjectsMovement(direction, movedObjects);
                await PerformMovement(direction, movedObjects);
                
            } while (!CheckContinueMovement());

            await ExecuteObjectsTeleportation();
            ClearMovementState();
            
            IsExecuting = false;
            
            if (HasDelayedObjects) ExecuteDelayedObjectsMovement();
            
            if (!isSecondary) _eventBus.GetEvent<MovementFinishedEvent>().InvokeForGlobal(new());
        }

        private List<LevelObjectBase> GetSortedMoveObjects(Direction direction)
            => SortObjects(_moveActions.Keys.ToList(), direction);
        
        private List<LevelObjectBase> SortObjects(List<LevelObjectBase> objects, Direction direction)
        => direction switch
        {
            Direction.Up    => objects.OrderByDescending(o => -o.Position.Rows).ThenBy(o => o.Position.Columns).ToList(),
            Direction.Down  => objects.OrderByDescending(o =>  o.Position.Rows).ThenBy(o => o.Position.Columns).ToList(),
            Direction.Left  => objects.OrderByDescending(o => -o.Position.Columns).ThenBy(o => -o.Position.Rows).ToList(),
            Direction.Right => objects.OrderByDescending(o =>  o.Position.Columns).ThenBy(o => -o.Position.Rows).ToList(),
        };

        /// <summary>
        /// A lot of repetitive checks inside this are required to create a consistent movement 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="movedObjects"></param>
        private void CheckObjectsMovement(Direction direction, List<LevelObjectBase> movedObjects)
        {
            foreach (var movedObject in movedObjects)
            {
                var moveAction = _moveActions[movedObject];
                    
                if (!CheckObjectCanMove(direction, movedObject, moveAction)) continue;
                if (!CheckObjectHasTargetCell(direction, movedObject, moveAction, out var targetCell)) continue;
                CheckObjectCanEnterCell(targetCell, movedObject, moveAction, _moveActions);
            }

            foreach (var movedObject in movedObjects)
            {
                var moveAction = _moveActions[movedObject];

                if (!CheckBoundObjectsAllowObjectToMove(movedObject, moveAction)) continue;
                if (!CheckObjectHasTargetCell(direction, movedObject, moveAction, out var targetCell)) continue;
                CheckObjectCanEnterCell(targetCell, movedObject, moveAction, _moveActions);
            }
                
            foreach (var movedObject in movedObjects)
            {
                var moveAction = _moveActions[movedObject];

                if (!CheckBoundObjectsAllowObjectToMove(movedObject, moveAction)) continue;
                if (!CheckObjectHasTargetCell(direction, movedObject, moveAction, out var targetCell)) continue;
                CheckObjectCanEnterCell(targetCell, movedObject, moveAction, _moveActions);
            }

            foreach (var movedObject in movedObjects)
            {
                var moveAction = _moveActions[movedObject];
                    
                if (!CheckObjectHasTargetCell(direction, movedObject, moveAction, out var targetCell)) continue;
                CheckObjectCanEnterCell(targetCell, movedObject, moveAction, _moveActions);
            }
            
            InterruptSubsequentObjectMovementIfNeeded();
        }

        private bool CheckObjectCanMove(Direction direction, LevelObjectBase movedObject, MoveAction moveAction)
        {
            var canMove = movedObject.CanMove(direction, moveAction);
            if (!canMove) moveAction.Interrupted = true;

            return !moveAction.Interrupted;
        }

        private bool CheckObjectHasTargetCell(Direction direction, LevelObjectBase movedObject, MoveAction moveAction,
            out LevelGridCell targetCell)
        {
            targetCell = movedObject.GetTargetCell(direction, moveAction);
            if (!targetCell) moveAction.Interrupted = true;

            return !moveAction.Interrupted;
        }

        private void CheckObjectCanEnterCell(LevelGridCell targetCell, LevelObjectBase movedObject,
            MoveAction moveAction, Dictionary<LevelObjectBase, MoveAction> moveActions)
        {
            if (targetCell.CheckObjectEnter(movedObject, moveActions)) return;
            
            moveAction.Interrupted = true;
        }

        private bool CheckBoundObjectsAllowObjectToMove(LevelObjectBase levelObject, MoveAction moveAction)
        {
            if (levelObject.Group == UnityConstants.Level.NoBindingGroup) return true;
            
            var boundObjects = _bindingGroups[levelObject.Group];
            var moveActions = boundObjects.ToDictionary(obj => obj, obj => _moveActions[obj]);

            if (!levelObject.CheckBoundObjectsAllowMove(moveActions)) moveAction.Interrupted = true;
            return !moveAction.Interrupted;
        }

        private void InterruptSubsequentObjectMovementIfNeeded()
        {
            foreach (var mainObject in _subsequentObjectsSets.Keys)
            {
                var subsequentObjectSetData = _subsequentObjectsSets[mainObject];
                if (subsequentObjectSetData.moved) continue;
                    
                var moveAction = _moveActions[mainObject];
                if (!moveAction.Interrupted) continue;
                    
                subsequentObjectSetData.objects.ForEach(o => _moveActions[o].Interrupted = true);    
            }

            _subsequentObjectsSets.Clear();
        }

        private async Task PerformMovement(Direction direction, List<LevelObjectBase> movedObjects)
        {
            OnMoveStarted(movedObjects);
            OnMoveFinishedForPreviouslyMovedObjects(movedObjects); // for already moved objects

            MoveObjectsIfNeeded(direction, movedObjects);

            await WaitForMovementToFinish();
        }

        private void OnMoveStarted(List<LevelObjectBase> movedObjects)
        {
            foreach (var movedObject in movedObjects)
            {
                var moveAction = _moveActions[movedObject];
                if (!moveAction.Started && !moveAction.Interrupted)
                {
                    moveAction.Started = true;
                    movedObject.OnMoveStarted();
                }
            }
        }

        private void OnMoveFinishedForPreviouslyMovedObjects(List<LevelObjectBase> movedObjects)
        {
            foreach (var movedObject in movedObjects)
            {
                var moveAction = _moveActions[movedObject];
                if (!moveAction.Finished && moveAction.Interrupted)
                {
                    moveAction.Finished = true;
                    if (moveAction.Path.Count > 1) movedObject.OnMoveFinished();
                }
            }
        }

        private void MoveObjectsIfNeeded(Direction direction, List<LevelObjectBase> movedObjects)
        {
            foreach (var movedObject in movedObjects)
            {
                var moveAction = _moveActions[movedObject];
                if (moveAction.Finished) continue;

                var startCell = movedObject.Cell;
                var targetCell = movedObject.GetTargetCell(direction, moveAction);
                CreateObjectMoveSequence(movedObject, targetCell);
                moveAction.Path.Add(targetCell);

                _eventBus.GetEvent<ObjectMovedEvent>().InvokeForGlobal(new (startCell, movedObject));
            }
        }

        private void CreateObjectMoveSequence(LevelObjectBase movedObject, LevelGridCell targetCell)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(_mover.MoveObject(movedObject, targetCell));
            sequence.AppendCallback(() => targetCell.AddObject(movedObject));
            
            _objectMovementSequences.Add(sequence.Play().AsyncWaitForCompletion());
        }

        private async Task WaitForMovementToFinish()
        {
            await Task.WhenAll(_objectMovementSequences);
            _objectMovementSequences.Clear();
        }

        private bool CheckContinueMovement() => _moveActions.Values.All(v => v.Interrupted);

        private void ClearMovementState()
        {
            _bindingGroups.Clear();
            _moveActions.Clear();
            _subsequentObjectsSets.Clear();
        }

        private async Task ExecuteObjectsTeleportation()
        {
            var teleportedObjects = _teleportActions.Keys.ToList();
            
            foreach (var teleportedObject in teleportedObjects)
            {
                var teleportAction = _teleportActions[teleportedObject];
                var teleportTarget = teleportAction.Destination;
                
                CheckObjectCanEnterCell(teleportTarget, teleportedObject, teleportAction, _teleportActions);
            }

            TeleportObjectsIfNeeded(teleportedObjects);

            await FinishTeleportation();
            ClearTeleportationState();
        }

        private void TeleportObjectsIfNeeded(List<LevelObjectBase> teleportedObjects)
        {
            foreach (var teleportedObject in teleportedObjects)
            {
                var teleportAction = _teleportActions[teleportedObject];
                if (teleportAction.Interrupted) continue;
                
                CreateTeleportSequence(teleportedObject, teleportAction.Destination);
            }
        }

        private void CreateTeleportSequence(LevelObjectBase teleportedObject, LevelGridCell destination)
        {
            var sequence = _mover.TeleportObject(teleportedObject, destination);
            sequence.AppendCallback(() => destination.AddObject(teleportedObject, true));
            sequence.OnComplete(() => _onTeleportActions[teleportedObject].Invoke());
            _objectTeleportSequences.Add(sequence.Play().AsyncWaitForCompletion());
        }

        private async Task FinishTeleportation()
        {
            await Task.WhenAll(_objectTeleportSequences);
            _objectTeleportSequences.Clear();
        }

        private void ClearTeleportationState()
        {
            _onTeleportActions.Clear();
            _teleportActions.Clear();
        }

        private void ExecuteDelayedObjectsMovement()
        {
            foreach (var delayedObject in _delayedMoveObjects)
            {
                RegisterObjectToMoveInternal(delayedObject, _delayedMoveDirection);
                RegisterSubsequentObjectsIfNeeded(delayedObject, _delayedMoveDirection);
            }

            var dir = ClearDelayedObjectsState();
            ExecuteObjectsMovement(dir);
        }

        private Direction ClearDelayedObjectsState()
        {
            var dir = _delayedMoveDirection;
            _delayedMoveDirection = Direction.None;
            _delayedMoveObjects.Clear();
            return dir;
        }
    }
}