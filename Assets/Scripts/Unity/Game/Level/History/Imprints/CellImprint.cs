namespace Soko.Unity.Game.Level.History.Imprints
{
    public class CellImprint
    {
        public CellImprint PreviousImprint { get; set; }
        public ObjectImprint GroundObjectImprint { get; set; }
        public ObjectImprint SolidObjectImprint { get; set; }
    }
}