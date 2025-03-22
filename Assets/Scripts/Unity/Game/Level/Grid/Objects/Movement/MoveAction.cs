using System.Collections.Generic;
using Soko.Unity.Game.Level.Grid.Enums;

namespace Soko.Unity.Game.Level.Grid.Objects.Movement
{
    public class MoveAction
    {
        public Direction StartingDirection { get; set; }
        public readonly List<LevelGridCell> Path = new();
        public bool Interrupted { get; set; }
        public bool IsTeleport { get; set; }
        
        public LevelGridCell Start => Path[0];
        public LevelGridCell Destination => Path[^1];
    }
}