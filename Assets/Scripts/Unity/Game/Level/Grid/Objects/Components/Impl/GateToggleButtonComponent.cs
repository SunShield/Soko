using System.Collections.Generic;
using Soko.Unity.Game.Sounds;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class GateToggleButtonComponent : LevelObjectComponent
    {
        [Inject] private SoundsManager _soundsManager;
        
        private readonly List<TogglableGateComponent> _gates = new();

        public override void OnLevelCreated()
        {
            Object.TryGetObjectComponent<ColorComponent>(out var colorComponent);
            var color = colorComponent.Color;
            foreach (var levelObject in Object.Cell.Grid.LevelObjects)
            {
                if (!levelObject.TryGetObjectComponent<TogglableGateComponent>(out var togglableGate)) continue;
                levelObject.TryGetObjectComponent<ColorComponent>(out var levelObjectColor);
                if (colorComponent.Color != levelObjectColor.Color) continue;
                
                _gates.Add(togglableGate);
            }
        }
        
        public override void OnObjectEntered(LevelObjectBase enteringObject)
        {
            if (!enteringObject.HasComponent<PlayerComponent>()) return;
            
            _soundsManager.PlaySfx(GameSfx.ClickLevelButton);
            _gates.ForEach(g => g.ToggleLockedState());
        }
    }
}