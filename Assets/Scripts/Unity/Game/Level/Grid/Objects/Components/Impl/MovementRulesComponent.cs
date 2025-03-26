using System.Collections.Generic;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using Soko.Unity.Game.Level.History.Imprints;
using Soko.Unity.Game.Level.History.Imprints.Impl;
using Soko.Unity.Game.Level.History.Interfaces;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    /// <summary>
    /// Only ONE of those is allowed on object simultaneously
    /// </summary>
    public class MovementRulesComponent : LevelObjectComponent, IImprintableComponent
    {
        public bool CanMove { get; private set; } = true;
        
        public void SetCanMove(bool canMove) => CanMove = canMove;

        public virtual LevelGridCell GetTargetCell(Direction direction, MoveAction moveAction) 
            => Object.Cell.GetNeighbour(direction);
        
        public bool CheckCanMove(Direction direction, MoveAction moveAction)
            => CheckCanMoveInternal(direction, moveAction) && CanMove;
        protected virtual bool CheckCanMoveInternal(Direction direction, MoveAction moveAction) => true;
        public virtual bool CheckBoundObjectsAllowMove(Dictionary<LevelObjectBase, MoveAction> bindingGroup) => true;
        public virtual List<LevelObjectBase> GetSubsequentObjects(Direction direction, MoveAction moveAction) => null;
        public virtual void OnMoveStarted() { }
        public virtual void OnMoveFinished() { }
        
        public ComponentImprint CreateComponentImprint()
        {
            var imprint = CreateComponentImprintInternal();
            imprint.CanMove = CanMove;
            return imprint;
        }

        protected virtual MovementRulesComponentImprint CreateComponentImprintInternal() => new();

        public void RestoreFromImprint(ComponentImprint imprint)
        {
            var imprintTyped = imprint as MovementRulesComponentImprint;
            CanMove = imprintTyped.CanMove;
            RestoreFromImprintInternal(imprintTyped);
        }

        protected virtual void RestoreFromImprintInternal(MovementRulesComponentImprint imprint) { }
    }
}