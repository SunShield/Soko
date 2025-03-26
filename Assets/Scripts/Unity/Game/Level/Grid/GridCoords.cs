namespace Soko.Unity.Game.Level.Grid
{
    public struct GridCoords
    {
        public int Rows;
        public int Columns;

        public GridCoords(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }

        public override bool Equals(object obj)
        {
            if (obj is not GridCoords gc) return false;
            return Rows.Equals(gc.Rows) && Columns.Equals(gc.Columns);
        }
    }
}