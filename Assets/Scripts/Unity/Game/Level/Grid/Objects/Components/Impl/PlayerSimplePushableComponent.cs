using System.Collections.Generic;
using System.Linq;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using Soko.Unity.Game.Sounds;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    /// <summary>
    /// Everything player is capable of pushing 1 space
    /// </summary>
    public class PlayerSimplePushableComponent : MovementRulesComponent
    {
        [Inject] private LevelObjectMover _levelObjectMover;
        [Inject] private SoundsManager _soundsManager;

        protected override bool CheckCanMoveInternal(Direction direction, MoveAction moveAction)
            => moveAction.Path.Count < 2;

        public override bool CheckBoundObjectsAllowMove(Dictionary<LevelObjectBase, MoveAction> bindingGroup)
        {
            return bindingGroup
                .Where(entry => entry.Key.HasComponent<PlayerSimplePushableComponent>())
                .All(entry => !entry.Value.Interrupted);
        }

        public override void OnPreMoved() => _soundsManager.PlaySfx(GameSfx.BoxPush);
    }
}