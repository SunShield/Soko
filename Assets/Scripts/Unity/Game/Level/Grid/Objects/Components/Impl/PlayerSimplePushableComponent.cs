using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using Soko.Unity.Game.Level.Grid.Objects.Helpers;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    /// <summary>
    /// Everything player is capable of pushing 1 space
    /// </summary>
    public class PlayerSimplePushableComponent : PlayerInteractableComponent
    {
        [Inject] private LevelObjectMover _levelObjectMover;
        
        protected override void OnPlayerAboutToEnter(LevelObjectBase enteringObject, MovementAction parentMoveAction)
        {
            var hasGroup = Object.TryGetComponent<GroupComponent>(out var groupComponent)
                           && groupComponent.Group != GroupComponent.NoGroup;
            var objectsToMove = hasGroup
                ? groupComponent.GroupObjects
                : new () { Object } ;
            var pushAction = new MovementAction(parentMoveAction.Direction);

            foreach (var objectToMove in objectsToMove)
            {
                var result = ProcessMovingObject(objectToMove, pushAction);
                if (result) continue;
                
                parentMoveAction.Active = false;
                return;
            }

            foreach (var objectToMove in objectsToMove)
            {
                var pushCell = objectToMove.Cell.GetNeighbour(pushAction.Direction);
                ExecuteMovement(objectToMove, pushCell);
            }
        }

        private LevelGridCell ProcessMovingObject(LevelObjectBase movingObject, MovementAction pushAction)
        {
            var pushCell = movingObject.Cell.GetNeighbour(pushAction.Direction);
            if (pushCell == null) return null;
            
            pushCell.OnObjectAboutToEnter(movingObject, pushAction);
            if (!pushAction.Active) return null;
            
            return pushCell;
        }
        
        private void ExecuteMovement(LevelObjectBase objectToMove, LevelGridCell targetCell) => 
            _levelObjectMover.MoveObject(objectToMove, targetCell);
    }
}