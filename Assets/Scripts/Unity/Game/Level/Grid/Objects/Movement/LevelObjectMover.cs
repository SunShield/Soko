using DG.Tweening;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects.Movement
{
    /// <summary>
    /// Helper encapsulating a common action - moving exaclty 1 cell in certain direction
    /// </summary>
    public class LevelObjectMover
    {
        private const float MoveTime = 0.1f;
        
        public Tween MoveObject(LevelObjectBase objectToMove, LevelGridCell targetCell)
            => objectToMove.transform.DOMove(targetCell.transform.position, MoveTime).SetEase(Ease.Linear);

        public Sequence TeleportObject(LevelObjectBase objectToMove, LevelGridCell targetCell)
        {
            var teleportSequence = DOTween.Sequence();
            var scale = objectToMove.transform.localScale;
            teleportSequence.Append(objectToMove.transform.DOScale(Vector3.zero, MoveTime));
            teleportSequence.AppendCallback(() => objectToMove.transform.position = targetCell.transform.position);
            teleportSequence.Append(objectToMove.transform.DOScale(scale, MoveTime));

            return teleportSequence;
        }
    }
}