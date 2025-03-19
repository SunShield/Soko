using System.Collections.Generic;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Movement;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    /// <summary>
    /// Only ONE of those is allowed on object simultaneously
    /// </summary>
    public class MovementRulesComponent : LevelObjectComponent
    {
        public bool CanMove { get; private set; } = true;
        
        public void SetCanMove(bool canMove) => CanMove = canMove;

        public virtual LevelGridCell GetTargetCell(Direction direction, MoveAction moveAction) 
            => Object.Cell.GetNeighbour(direction);
        
        public bool CheckCanMove(Direction direction, MoveAction moveAction)
            => CheckCanMoveInternal(direction, moveAction) && CanMove;
        protected virtual bool CheckCanMoveInternal(Direction direction, MoveAction moveAction) => true;
        public virtual bool CheckBoundObjectsAllowMovement(List<LevelObjectBase> boundObjects, Direction direction) 
            => true;
    }
}