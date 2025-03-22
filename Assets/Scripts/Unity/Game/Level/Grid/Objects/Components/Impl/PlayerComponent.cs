using System.Collections.Generic;
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
            
            var hasPossibleObject = targetPlayerCell.Objects.TryGetValue(ObjectLayer.Solid, out var possibleObject);
            if (!hasPossibleObject) return null;
            
            var isPLayerMovable = possibleObject.HasComponent<PlayerMovableComponent>();
            return isPLayerMovable ? new List<LevelObjectBase> { possibleObject } : null;
        }
    }
}