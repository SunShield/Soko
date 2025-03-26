using System.Collections.Generic;
using Soko.Unity.Game.Level.Grid;
using Soko.Unity.Game.Level.History.Interfaces;

namespace Soko.Unity.Game.Level.History.Imprints
{
    public class ObjectImprint
    {
        public LevelGridCell Cell { get; set; }
        public Dictionary<IImprintableComponent, ComponentImprint> ComponentImprints = new ();
    }
}