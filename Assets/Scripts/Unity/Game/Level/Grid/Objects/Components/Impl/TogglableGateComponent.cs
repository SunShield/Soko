using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class TogglableGateComponent : LevelObjectComponent
    {
        [SerializeField] private GameObject _lockedGraphics;
        [field: SerializeField] public bool Locked { get; set; }

        private bool _reallyLocked;
        private bool _hasObjectOn;

        public override void OnLevelCreated()
        {
            if (Object.Cell.Objects.Count > 1) _hasObjectOn = true;
            
            if (!_hasObjectOn) _reallyLocked = Locked;
        }

        public void ToggleLockedState()
        {
            _reallyLocked = !_reallyLocked;
            if (_hasObjectOn) return;
            
            Locked = !Locked;
            _lockedGraphics.SetActive(Locked);
        }

        public override bool CheckObjectEnter(LevelObjectBase enteringObject) => !_reallyLocked;
        public override void OnObjectEntered(LevelObjectBase enteringObject) => _hasObjectOn = true;

        public override void OnObjectLeft(LevelObjectBase enteringObject)
        {
            _hasObjectOn = false;
            if (_reallyLocked && !Locked) ToggleLockedState();
        }
    }
}