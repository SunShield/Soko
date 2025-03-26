using Soko.Unity.Game.Level.History.Imprints;

namespace Soko.Unity.Game.Level.History.Interfaces
{
    public interface IImprintableComponent
    {
        ComponentImprint CreateComponentImprint();
        void RestoreFromImprint(ComponentImprint imprint);
    }
}