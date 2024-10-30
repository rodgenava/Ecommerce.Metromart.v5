namespace Data.Projections
{
    public record PandamartStore(string Code)
    {
        public override string ToString()
        {
            return Code;
        }
    }
}
