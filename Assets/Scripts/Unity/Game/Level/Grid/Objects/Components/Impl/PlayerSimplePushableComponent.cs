using System.Collections.Generic;
using System.Linq;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using Soko.Unity.Game.Level.Grid.Objects.Helpers;
using Soko.Unity.Game.Sounds;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    /// <summary>
    /// Everything player is capable of pushing 1 space
    /// </summary>
    public class PlayerSimplePushableComponent : PlayerInteractableComponent
    {
        [Inject] private LevelObjectMover _levelObjectMover;
        [Inject] private SoundsManager _soundsManager;
        
        protected override void OnPlayerAboutToEnter(LevelObjectBase enteringObject, MovementAction parentMoveAction)
        {
            var objectsToMove = GetObjectsToMove(parentMoveAction.Direction);
            var pushAction = new MovementAction(parentMoveAction.Direction);
            if (!EnsureObjectsMovable(parentMoveAction, objectsToMove, pushAction)) return;

            MoveObjects(objectsToMove, pushAction);
        }

        private List<LevelObjectBase> GetObjectsToMove(Direction direction)
        {
            var hasGroup = Object.TryGetComponent<GroupComponent>(out var groupComponent)
                           && groupComponent.Group != GroupComponent.NoGroup;
            var objectsToMove = hasGroup
                ? SortObjectsByDistance(groupComponent.GroupObjects, direction)
                : new () { Object } ;
            return objectsToMove;
        }
        
        private List<LevelObjectBase> SortObjectsByDistance(List<LevelObjectBase> objects, Direction direction)
            => direction switch
            {
                Direction.Up    => objects.OrderByDescending(o => -o.Cell.Coords.Rows).ToList(),
                Direction.Down  => objects.OrderByDescending(o => o.Cell.Coords.Rows).ToList(),
                Direction.Left  => objects.OrderByDescending(o => -o.Cell.Coords.Columns).ToList(),
                Direction.Right => objects.OrderByDescending(o => o.Cell.Coords.Columns).ToList(),
            };

        private bool EnsureObjectsMovable(MovementAction parentMoveAction, List<LevelObjectBase> objectsToMove, MovementAction pushAction)
        {
            foreach (var objectToMove in objectsToMove)
            {
                var result = ProcessMovingObject(objectToMove, pushAction);
                if (result) continue;
                
                parentMoveAction.Active = false;
                return false;
            }

            return true;
        }

        private LevelGridCell ProcessMovingObject(LevelObjectBase movingObject, MovementAction pushAction)
        {
            var pushCell = movingObject.Cell.GetNeighbour(pushAction.Direction);
            if (pushCell == null) return null;
            
            pushCell.OnObjectAboutToEnter(movingObject, pushAction);
            if (!pushAction.Active) return null;
            
            return pushCell;
        }

        private void MoveObjects(List<LevelObjectBase> objectsToMove, MovementAction pushAction)
        {
            foreach (var objectToMove in objectsToMove)
            {
                var pushCell = objectToMove.Cell.GetNeighbour(pushAction.Direction);
                ExecuteMovement(objectToMove, pushCell);
            }
        }
        
        private void ExecuteMovement(LevelObjectBase objectToMove, LevelGridCell targetCell)
        {
            _soundsManager.PlaySfx(GameSfx.BoxPush);
            _levelObjectMover.MoveObject(objectToMove, targetCell);
        }
    }
}