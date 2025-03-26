using Soko.Unity.Game.Level.Grid;
using Soko.Unity.Game.Level.Grid.Objects;

namespace Soko.Core.Events.Impl.Args
{
    public class ObjectMoveArgs : IGameEventArgs
    {
        public LevelGridCell StartCell { get; set; }
        public LevelObjectBase MovedObject { get; set; }

        public ObjectMoveArgs(LevelGridCell startCell, LevelObjectBase movedObject)
        {
            StartCell = startCell;
            MovedObject = movedObject;
        }
    }
}