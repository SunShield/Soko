using Soko.Unity.Game.Level.Grid.Enums;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement
{
    public class MovementAction
    {
        public bool Active = true;
        public Direction Direction;
        
        public MovementAction() { }
        public MovementAction(Direction direction) => Direction = direction;
    }
}