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
    }
}