using System.Threading.Tasks;
using DG.Tweening;

namespace Soko.Unity.Game.Level.Grid.Objects.Helpers
{
    /// <summary>
    /// Helper encapsulating a common action - moving exaclty 1 cell in certain direction
    /// </summary>
    public class LevelObjectMover
    {
        private const float MoveTime = 0.25f;
        
        public async Task MoveObject(LevelObjectBase objectToMove, LevelGridCell targetCell)
        {
            await objectToMove.transform
                .DOMove(targetCell.transform.position, MoveTime)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion();
            targetCell.AddObject(objectToMove);
        }
    }
}