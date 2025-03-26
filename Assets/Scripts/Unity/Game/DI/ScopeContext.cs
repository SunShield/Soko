using VContainer;

namespace Soko.Unity.Game.DI
{
    public class ScopeContext {
        public static IObjectResolver CurrentScope { get; set; }
    }
}