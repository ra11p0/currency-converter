
namespace currency_converter
{
    public struct Currency
    {
        public string Symbol { get; }
        public float ToEurFactor { get; }
        public float FromEurFactor { get; }
        public Currency(string Symbol, float FomEurRate)
        {
            this.Symbol = Symbol;
            this.ToEurFactor = 1 / FomEurRate;
            this.FromEurFactor = FomEurRate;
        }

    }
}
