using System.Collections.Generic;
using System.Linq;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Movement;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    /// <summary>
    /// Defines if object is a player
    /// </summary>
    public class PlayerComponent : MovementRulesComponent
    {
        protected override bool CheckCanMoveInternal(Direction direction, MoveAction moveAction)
            => moveAction.Path.Count < 2;

        public override List<LevelObjectBase> GetSubsequentObjects(Direction direction, MoveAction moveAction)
        {
            var targetPlayerCell = Object.GetTargetCell(direction, null);
            if (targetPlayerCell == null) return null;
            
            var targetObject = targetPlayerCell.Objects
                .FirstOrDefault(obj => obj.HasComponent<PlayerMovableComponent>());
            return targetObject == null ? null : new List<LevelObjectBase> { targetObject };
        }
    }
}