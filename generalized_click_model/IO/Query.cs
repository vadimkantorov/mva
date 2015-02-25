namespace Clik
{
    public struct Query
    {
        public int QueryId;
        public int RegionId;
        public int QueryTimePassed;
        public int[] URLs;
        public bool[] IsClicked;
        public int[] ClickTimePassed;
    }
}