using Soko.Unity.Game.Level.History.Imprints;
using Soko.Unity.Game.Level.History.Imprints.Impl;
using Soko.Unity.Game.Level.History.Interfaces;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class TogglableGateComponent : LevelObjectComponent, IImprintableComponent
    {
        [SerializeField] private GameObject _lockedGraphics;
        [field: SerializeField] public bool Locked { get; set; } // Internal Locked state

        // In-level locked state. Can differ from the internal state if there's an object on gate
        private bool _lockedInLevel; 
        private bool _hasObjectOn;

        public override void OnLevelCreated()
        {
            if (Object.Cell.Objects.Count > 1) _hasObjectOn = true;
            if (!_hasObjectOn) _lockedInLevel = Locked;
        }

        public void ToggleLockedState()
        {
            Locked = !Locked;
            _lockedInLevel = Locked;
            if (_hasObjectOn) _lockedInLevel = false;
            
            _lockedGraphics.SetActive(_lockedInLevel);
        }

        public override bool CheckObjectEnter(LevelObjectBase enteringObject) => !_lockedInLevel;
        
        public override void OnObjectEntered(LevelObjectBase enteringObject)
        {
            _hasObjectOn = true;
            _lockedInLevel = false;
            _lockedGraphics.SetActive(_lockedInLevel);
        }

        public override void OnObjectLeft(LevelObjectBase enteringObject)
        {
            _hasObjectOn = false;
            _lockedInLevel = Locked;
            _lockedGraphics.SetActive(_lockedInLevel);
        }

        public ComponentImprint CreateComponentImprint()
        {
            var imprint = new TogglableGateComponentImprint
            {
                Locked = Locked,
                HasObjectOn = _hasObjectOn
            };
            
            return imprint;
        }

        public void RestoreFromImprint(ComponentImprint imprint)
        {
            var imprintTyped = imprint as TogglableGateComponentImprint;
            Locked = imprintTyped.Locked;
            _hasObjectOn = imprintTyped.HasObjectOn;
            
            _lockedInLevel = Locked;
            if (_hasObjectOn) _lockedInLevel = false;
            _lockedGraphics.SetActive(_lockedInLevel);
        }
    }
}