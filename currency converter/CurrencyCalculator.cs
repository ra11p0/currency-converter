using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace currency_converter
{
    public class CurrencyCalculator
    {
        public List<Currency> Currencies { get; } = new List<Currency>();
        public bool Connect(string source)
        {
            XDocument document;
            //get data from server
            try
            {
                document = XDocument.Parse(new WebClient().DownloadString(source));
            }
            catch (Exception)
            {
                return false;
            }
            parse(document);
            return true;
        }
        public bool GetFromFile(String path)
        {
            XDocument document;
            //get data from server
            try
            {
                document = XDocument.Parse(File.ReadAllText(path));
            }
            catch (Exception)
            {
                return false;
            }
            parse(document);
            return true;
        }
        private void parse(XDocument document)
        {
            //find currencies list (in the second cube)
            List<XElement> curr = document.
                Root.
                Elements().
                Where(x => x.Name.LocalName.Contains("Cube")).
                Elements().
                Where(x => x.Name.LocalName.Contains("Cube")).
                Elements().
                ToList();
            //generate currencies list
            foreach (XElement element in curr)
                Currencies.Add(new Currency(element.Attribute("currency").Value, float.Parse(element.Attribute("rate").Value.Replace('.', ','))));
        }
        public float ConvertToEur(Currency from, float ammount)
        {
            return ammount * from.FromEurFactor;
        }
        public float ConvertFromEur(Currency to, float ammount)
        {
            return ammount * to.ToEurFactor;
        }
    }
}
