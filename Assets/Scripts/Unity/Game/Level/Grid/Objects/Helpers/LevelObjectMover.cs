using System.Collections.Generic;
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

        public async Task MoveObject(LevelObjectBase objectToMove, List<LevelGridCell> path)
        {
            var sequence = DOTween.Sequence();
            foreach (var cell in path)
            {
                var moveTween = objectToMove.transform
                    .DOMove(cell.transform.position, MoveTime / path.Count)
                    .OnComplete(() => cell.AddObject(objectToMove));
                sequence.Append(moveTween);
            }

            await sequence.Play().AsyncWaitForCompletion();
        }

        public void TeleportObject(LevelObjectBase objectToMove, LevelGridCell targetCell)
        {
            objectToMove.transform.position = targetCell.transform.position;
            targetCell.AddObject(objectToMove, true);
        }
    }
}