using NUnit.Framework;
using currency_converter;

namespace CurrencyCalculatorTests
{
    public class Tests
    {
        CurrencyCalculator calc;
        [SetUp]
        public void Setup()
        {
            calc = new CurrencyCalculator();
        }

        [Test]
        public void CheckUrlWrong()
        {
            Assert.IsFalse(calc.Connect("wrongAddress"));
        }
        [Test]
        public void CheckUrlCorrect()
        {
            Assert.IsTrue(calc.Connect("http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"));
        }
        [Test]
        public void CheckPathWrong()
        {
            Assert.IsFalse(calc.GetFromFile("wrongPath"));
        }
        [Test]
        public void CheckPathCorrect()
        {
            //if cant load because theres no such file, user needs to copy file to the tests folder
            Assert.IsTrue(calc.GetFromFile("eurofxref-daily.xml"));
        }
        [Test]
        public void CurrencyCalculationCorrect()
        {
            Currency curr = new Currency("test", 100);
            Assert.AreEqual(curr.ToEurFactor, 0,01f);
        }
        [Test]
        public void CurrencyCalculationWrong()
        {
            Currency curr = new Currency("test", 100);
            Assert.AreNotEqual(curr.ToEurFactor, 100f);
        }
        [Test]
        public void TwoWayConvert()
        {
            Currency curr = new Currency("test", 1.234f);
            float ammount = 1556.78f;
            Assert.IsTrue(0.01f > ammount - (ammount * curr.FromEurFactor * curr.ToEurFactor));
        }
    }
}