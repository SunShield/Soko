using System.Collections.Generic;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using Soko.Unity.Game.Level.Grid.Objects.Helpers;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class PlayerFarPushableComponent : PlayerInteractableComponent
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

            var pushCells = new Dictionary<LevelObjectBase, LevelGridCell>();
            foreach (var objectToMove in objectsToMove)
            {
                var pushCell = GetPushCell(objectToMove, pushAction);
                if (objectToMove == Object && pushCell == null)
                {
                    parentMoveAction.Active = false;
                    return;
                }
                pushCells.Add(objectToMove, pushCell);
            }

            foreach (var objectToMove in objectsToMove)
            {
                var pushCell = pushCells[objectToMove];
                ExecuteMovement(objectToMove, pushCell);
            }
        }

        private LevelGridCell GetPushCell(LevelObjectBase movingObject, MovementAction pushAction)
        {
            var secondaryPushAction = new MovementAction(pushAction.Direction);
            var pushCell = movingObject.Cell.GetNeighbour(pushAction.Direction);
            if (pushCell == null) return null;
            pushCell.OnObjectAboutToEnter(movingObject, secondaryPushAction);
            if (!secondaryPushAction.Active) return null;

            while (pushCell != null)
            {
                var pushCellNeighbour = pushCell.GetNeighbour(pushAction.Direction);
                if (pushCellNeighbour == null) break;
                pushCellNeighbour.OnObjectAboutToEnter(movingObject, secondaryPushAction);
                if (!secondaryPushAction.Active) break;
                pushCell = pushCellNeighbour;
            }
            
            return pushCell;
        }
        
        private void ExecuteMovement(LevelObjectBase objectToMove, LevelGridCell targetCell) => 
            _levelObjectMover.MoveObject(objectToMove, targetCell);
    }
}