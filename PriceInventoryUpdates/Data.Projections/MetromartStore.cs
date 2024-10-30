namespace Data.Projections
{
    public record MetromartStore(string Code)
    {
        public override string ToString()
        {
            return Code;
        }
    }
}