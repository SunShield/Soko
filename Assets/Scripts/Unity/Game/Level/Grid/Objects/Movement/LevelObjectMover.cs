using System.Threading.Tasks;

namespace Soko.Unity.Game.Level.Grid.Objects.Movement
{
    /// <summary>
    /// Helper encapsulating a common action - moving exaclty 1 cell in certain direction
    /// </summary>
    public class LevelObjectMover
    {
        private const float MoveTime = 0.1f;
        
        public async Task MoveObject(LevelObjectBase objectToMove, LevelGridCell targetCell)
        {
            objectToMove.transform.position = targetCell.transform.position;
            targetCell.AddObject(objectToMove);
        }

        public void TeleportObject(LevelObjectBase objectToMove, LevelGridCell targetCell)
        {
            objectToMove.transform.position = targetCell.transform.position;
            targetCell.AddObject(objectToMove, true);
        }
    }
}