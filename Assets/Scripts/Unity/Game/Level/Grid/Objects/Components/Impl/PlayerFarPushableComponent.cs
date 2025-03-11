using System.Collections.Generic;
using System.Linq;
using Soko.Core.Extensions;
using Soko.Unity.Game.Level.Grid.Enums;
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
                ? SortObjectsByDistance(groupComponent.GroupObjects, parentMoveAction.Direction)
                : new () { Object } ;
            var pushAction = new MovementAction(parentMoveAction.Direction);

            var pushPaths = new Dictionary<LevelObjectBase, List<LevelGridCell>>();
            var destinations = new HashSet<LevelGridCell>();
            foreach (var objectToMove in objectsToMove)
            {
                pushPaths.Add(objectToMove, new List<LevelGridCell>());
                var pushCell = GetPushCell(objectToMove, pushAction, pushPaths[objectToMove], destinations);
                if (objectToMove == Object && pushCell == null)
                {
                    parentMoveAction.Active = false;
                    return;
                }

                destinations.Add(pushCell);
                pushPaths[objectToMove].Add(pushCell);
            }

            foreach (var objectToMove in objectsToMove)
            {
                var pushPath = pushPaths[objectToMove];
                ExecuteMovement(objectToMove, pushPath);
            }
        }

        private List<LevelObjectBase> SortObjectsByDistance(List<LevelObjectBase> objects, Direction direction)
            => direction switch
            {
                Direction.Up    => objects.OrderByDescending(o => o.Cell.Coords.Rows).ToList(),
                Direction.Down  => objects.OrderByDescending(o => -o.Cell.Coords.Rows).ToList(),
                Direction.Left  => objects.OrderByDescending(o => -o.Cell.Coords.Columns).ToList(),
                Direction.Right => objects.OrderByDescending(o => o.Cell.Coords.Columns).ToList(),
            };

        private LevelGridCell GetPushCell(LevelObjectBase movingObject, MovementAction pushAction,
                    List<LevelGridCell> pushPath, HashSet<LevelGridCell> destinations)
        {
            var secondaryPushAction = new MovementAction(pushAction.Direction);
            var pushCell = movingObject.Cell.GetNeighbour(pushAction.Direction);
            if (pushCell == null) return null;
            pushCell.OnObjectAboutToEnter(movingObject, secondaryPushAction);
            if (!secondaryPushAction.Active) return null;
            if(destinations.Contains(pushCell)) return null;

            while (pushCell != null)
            {
                pushPath.Add(pushCell);
                var pushCellNeighbour = pushCell.GetNeighbour(pushAction.Direction);
                if (pushCellNeighbour == null) break;
                if (destinations.Contains(pushCellNeighbour)) break;
                pushCellNeighbour.OnObjectAboutToEnter(movingObject, secondaryPushAction);
                if (!secondaryPushAction.Active) break;
                pushCell = pushCellNeighbour;
            }
            
            return pushCell;
        }
        
        private void ExecuteMovement(LevelObjectBase objectToMove, List<LevelGridCell> path) => 
            _levelObjectMover.MoveObject(objectToMove, path);
    }
}