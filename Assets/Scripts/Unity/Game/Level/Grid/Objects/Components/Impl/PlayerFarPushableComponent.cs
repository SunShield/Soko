using Soko.Unity.Game.Sounds;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class PlayerFarPushableComponent : MovementRulesComponent
    {
        [Inject] private SoundsManager _soundsManager;
        
        public override void OnMoveStarted() => _soundsManager.PlaySfx(GameSfx.SlideableBoxPush);
        public override void OnMoveFinished() => _soundsManager.PlaySfx(GameSfx.SlideableBoxPushEnd);
    }
}