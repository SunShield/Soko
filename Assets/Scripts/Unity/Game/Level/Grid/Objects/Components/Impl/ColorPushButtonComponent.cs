using System.Collections.Generic;
using System.Linq;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class ColorPushButtonComponent : LevelObjectComponent
    {
        [field: SerializeField] public Direction Direction { get; private set; }
        
        [Inject] private MoveManager _moveManager;

        public override void OnObjectEntered(LevelObjectBase enteringObject)
        {
            if (!enteringObject.HasComponent<PlayerComponent>()) return;
            
            var hasColor = Object.TryGetObjectComponent<ColorComponent>(out var colorComponent);
            if (!hasColor) return;

            var coloredMovableObjects = new List<LevelObjectBase>();
            foreach (var levelObject in Object.Cell.Grid.LevelObjects)
            {
                var loHasColor = levelObject.TryGetObjectComponent<ColorComponent>(out var loColorComponent);
                if (!loHasColor) continue;
                if (loColorComponent.Color != colorComponent.Color) continue;
                if (levelObject.MovementRulesComponent == null) continue;
                
                coloredMovableObjects.Add(levelObject);
            }
            
            coloredMovableObjects.ForEach(co => _moveManager.RegisterObjectToMove(co, Direction));
            _moveManager.ExecuteObjectsMovement(Direction);
        }
    }
}