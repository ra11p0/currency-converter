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
        public Dictionary<string, decimal> Currencies { get; } = new Dictionary<string, decimal>();
        public void Connect(string source)
        {
            XDocument document;
            //get data from server, if exception, return false
            try
            {
                document = XDocument.Parse(new WebClient().DownloadString(source));
            }
            catch (Exception)
            {
                throw;
            }
            parse(document);
        }
        public void GetFromFile(String path)
        {
            XDocument document;
            //get data from file, if exception, return false
            try
            {
                document = XDocument.Parse(File.ReadAllText(path));
            }
            catch (Exception)
            {
                throw;
            }
            parse(document);
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
            //if there are no currency records, return
            if (curr.Count() == 0) return;
            foreach (XElement element in curr)
                Currencies.Add(element.Attribute("currency").Value, decimal.Parse(element.Attribute("rate").Value.Replace('.', ',')));
        }
        public decimal ConvertToEur(string symbol, decimal ammount)
        {
            return ammount / Currencies[symbol];
        }
        public decimal ConvertFromEur(string symbol, decimal ammount)
        {
            return ammount * Currencies[symbol];
        }
    }
}
