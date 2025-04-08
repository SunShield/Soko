using Cysharp.Threading.Tasks;

namespace Soko.Unity.Game.Tutorials.Actions
{
    public abstract class TutorialAction
    {
        public abstract UniTask Execute();
    }
}