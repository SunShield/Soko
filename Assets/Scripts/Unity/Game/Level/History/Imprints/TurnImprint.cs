using System.Collections.Generic;
using Soko.Unity.Game.Level.Grid;

namespace Soko.Unity.Game.Level.History.Imprints
{
    public class TurnImprint
    {
        public List<ObjectImprint> ObjectImprints { get; private set; } = new();
    }
}