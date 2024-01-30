using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace AlHersey.ViewComponents
{
    public class Exchange : ViewComponent
    {
        public string Invoke()
        {
            string url = "http://www.tcmb.gov.tr/kurlar/today.xml";

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(url);

            // XmlNode ile USD'nin satış fiyatını al
            XmlNode usdNode = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='USD']/BanknoteSelling");

            if (usdNode != null)
            {
                // InnerText ile node'un değerini al
                string usdSatis = usdNode.InnerText;

                // İlk 5 karakteri al
                usdSatis = usdSatis.Substring(0, 5);

                return usdSatis;
            }
            else
            {
                return "Hata: XML belgesinde USD düğümü bulunamadı.";
            }
        }
    }
}
