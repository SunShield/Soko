using Soko.Unity.Game.Level.Grid.Enums;

namespace Soko.Service.Extensions
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
    }
}