
namespace currency_converter
{
    public struct Currency
    {
        public string Symbol { get; }
        public float ToEurFactor { get; }
        public float FromEurFactor { get; }
        public Currency(string Symbol, float ToEurFactor)
        {
            this.Symbol = Symbol;
            this.ToEurFactor = ToEurFactor;
            this.FromEurFactor = 1 / ToEurFactor;
        }

    }
}
