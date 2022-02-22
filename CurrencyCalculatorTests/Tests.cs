using NUnit.Framework;
using currency_converter;
using System;

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
            try
            {
                calc.Connect("wrongAddress");
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }
        [Test]
        public void CheckUrlCorrect()
        {
            try
            {
                calc.Connect("http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");
                Assert.Pass();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        [Test]
        public void CheckPathWrong()
        {
            try
            {
                calc.GetFromFile("wrongPath");
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.Pass();
            }
            
        }
        [Test]
        public void CheckPathCorrect()
        {
            //if cant load because theres no such file, user needs to copy file to the tests folder
            try
            {
                calc.GetFromFile("eurofxref-daily.xml");
                Assert.Pass();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            
        }
    }
}