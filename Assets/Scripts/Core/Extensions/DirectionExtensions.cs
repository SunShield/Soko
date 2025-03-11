using Soko.Unity.Game.Level.Grid.Enums;

namespace Soko.Core.Extensions
{
    public static class DirectionExtensions
    {
        public static Direction Reverse(this Direction direction) => direction switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            Direction.None => Direction.None,
        };

        public static bool IsHorizontal(this Direction direction) => direction is Direction.Left or Direction.Right;
        public static bool IsVertical(this Direction direction) => direction is Direction.Left or Direction.Right;
    }
}