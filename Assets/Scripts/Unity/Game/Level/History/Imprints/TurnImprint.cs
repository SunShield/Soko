using System.Collections.Generic;
using Soko.Unity.Game.Level.Grid;

namespace Soko.Unity.Game.Level.History.Imprints
{
    public class TurnImprint
    {
        public Dictionary<GridCoords, CellImprint> CellImprints { get; private set; } = new();
    }
}