using ExecutiveOffice.EDT.GlobalNotesService.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using ExecutiveOffice.EDT.GlobalNotesService.Entities;
using System;
using ExecutiveOffice.EDT.GlobalNotesService.Extensions;

namespace ExecutiveOffice.EDT.GlobalNotesService.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var globalNotes = new List<GlobalNote>()
            {
                new GlobalNote
                {
                    Isin = string.Format("DE00000054000112AA1"),
                    NumberOfShares = "24.000.000",
                    DistributionCurrency = Enum.GetName(typeof(Currency), Currency.EUR),
                    Issuer = "Landesbank Baden-Württemberg",
                    ProductName = "2,5 % LBBW Commerzbank Aktien-Anleihe",
                    Language= "DE",
                },
                new GlobalNote
                {
                    Isin = string.Format("DE000UW83179"),
                    NumberOfShares = "1.000.000",
                    DistributionCurrency = Enum.GetName(typeof(Currency), Currency.EUR),
                    Issuer = "UBS AG, London Branch",
                    ProductName = "BONUS_CAPPED_CERTIFICATE on Peugeot",
                    Language= "EN",
                },
            };

            string htmlContent = new GlobalNotesAsHtmlProcessor().GetGlobalNotes(globalNotes);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "file.html");

            File.WriteAllText(path, htmlContent);

            Console.WriteLine(path);
        }


      
    }
}
