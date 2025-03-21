using System.Linq;
using Soko.Unity.Game.Level.Grid.Objects.Helpers;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using Soko.Unity.Game.Sounds;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class PlayerTeleporterComponent : LevelObjectComponent
    {
        [Inject] private LevelObjectMover _levelObjectMover;
        [Inject] private SoundsManager _soundsManager;

        public override void OnObjectEntered(LevelObjectBase enteringObject)
        {
            if (!enteringObject.HasComponent<PlayerComponent>()) return;
            
            var group = Object.GetComponent<ColorComponent>();
            var boundTeleporter = LevelPlayCycleManager.LevelGrid.LevelObjects
                .Except(new[] { Object })
                .Where(o => o.HasComponent<PlayerTeleporterComponent>())
                .Where(o =>
                {
                    var hasGroup = o.TryGetComponent<ColorComponent>(out var colorComponent);
                    if (!hasGroup) return false;

                    return colorComponent.Color == group.Color;
                })
                .First();

            if (boundTeleporter == null) return;
            if (!boundTeleporter.CheckObjectEnter(enteringObject)) return;
            
            ExecuteTeleportation(enteringObject, boundTeleporter);
        }

        private void ExecuteTeleportation(LevelObjectBase playerObject, LevelObjectBase boundTeleporter)
        {
            _soundsManager.PlaySfx(GameSfx.Teleporter);
            _levelObjectMover.TeleportObject(playerObject, boundTeleporter.Cell);
        }
    }
}