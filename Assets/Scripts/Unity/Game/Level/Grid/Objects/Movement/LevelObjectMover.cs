using DG.Tweening;

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

        public void TeleportObject(LevelObjectBase objectToMove, LevelGridCell targetCell)
        {
            objectToMove.transform.position = targetCell.transform.position;
        }
    }
}