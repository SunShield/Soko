using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Management;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    /// <summary>
    /// If spot has no color or no color component, any spot activator activates it.
    /// White spot can be activated with any colored spot activator.
    /// Colored spot cat be activated with spot activators of the same color.
    /// </summary>
    public class SpotComponent : LevelObjectComponent
    {
        [SerializeField] private GameObject _activeGraphics;
        [SerializeField] private GameObject _inactiveGraphics;
        
        [Inject] private LevelManager _levelManager;
        
        public bool Activated { get; private set; }

        public override void OnObjectEntered(LevelObjectBase enteringObject)
        {
            if (!enteringObject.HasComponent<SpotActivatorComponent>()) return;

            if (Object.TryGetComponent<ColorComponent>(out var currentColorComponent))
            {
                if (currentColorComponent.Color == ObjectColor.None)
                {
                    SetActivated(true);
                    return;
                }
                
                var checkedColor = ObjectColor.None;
                if (enteringObject.TryGetComponent<ColorComponent>(out var colorComponent))
                    checkedColor = colorComponent.Color;
                
                if (currentColorComponent.Color == ObjectColor.White && checkedColor != ObjectColor.None ||
                    currentColorComponent.Color != ObjectColor.White && checkedColor == currentColorComponent.Color)
                    SetActivated(true);
            }
            else
                SetActivated(true);
        }

        public override void OnObjectLeft(LevelObjectBase enteringObject)
        {
            if (!enteringObject.HasComponent<SpotActivatorComponent>()) return;
            
            SetActivated(false);
        }
        
        private void SetActivated(bool activated)
        {
            Activated = activated;
            _activeGraphics.SetActive(activated);
            _inactiveGraphics.SetActive(!activated);
            
            if (Activated) _levelManager.CheckWin();
        }
    }
}