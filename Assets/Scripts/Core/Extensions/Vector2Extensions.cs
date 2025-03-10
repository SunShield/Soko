using Soko.Unity.Game.Level.Grid.Enums;
using UnityEngine;

namespace Soko.Core.Extensions
{
    public static class Vector2Extensions
    {
        public static Direction ToDirection(this Vector2 vector)
        {
            if (vector is { x: 0f, y: 1f }) return Direction.Up;
            if (vector is { x: 0f, y: -1f }) return Direction.Down;
            if (vector is { x: -1f, y: 0f }) return Direction.Left;
            if (vector is { x: 1f, y: 0f }) return Direction.Right;
            return Direction.None;
        }
    }
}