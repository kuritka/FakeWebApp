using ExecutiveOffice.EDT.GlobalNotesService.Entities;
using ExecutiveOffice.EDT.GlobalNotesService.Extensions;
using ExecutiveOffice.EDT.GlobalNotesService.Infrsatructure;
using ExecutiveOffice.EDT.GlobalNotesService.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace ExecutiveOffice.EDT.GlobalNotesService.Tests
{
    [TestClass]
    public class CsvReaderTests
    {
        private readonly DirectoryInfo _workingDirectory = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "CsvTest"));

        private readonly FileInfo _leiMapCsv = new FileInfo(Path.Combine("TestData", "LeiMap.csv"));
        private readonly FileInfo _leiMapCsvOddQuotes = new FileInfo(Path.Combine("TestData", "LeiMapOddQuotes.csv"));
        private readonly FileInfo _leiMapCsvLowerCaseHeader = new FileInfo(Path.Combine("TestData", "LeiMapLowerCase.csv"));
        private readonly FileInfo _leiMapCsvWithoutHeader = new FileInfo(Path.Combine( "TestData", "LeiMapWithoutHeader.csv"));
        private readonly FileInfo _leiMapEmptyCsvHeader = new FileInfo(Path.Combine( "TestData", "LeiMapOnlyHeader.csv"));
        private readonly FileInfo _leiMapEmpty = new FileInfo(Path.Combine("TestData", "LeiMapEmpty.csv"));
        private readonly FileInfo _leiMapCsvXml = new FileInfo(Path.Combine("TestData", "LeiMap.csv.xml"));
        private readonly FileInfo _edtTableCsv = new FileInfo(Path.Combine("TestData", "P001-7554-VL344E.csv"));
        private readonly FileInfo _edtTableCsvWithgMultipleLines = new FileInfo(Path.Combine("TestData", "P001-7554-VL344E-2.csv"));



        [TestInitialize]
        public void SetUp()
        {
            _workingDirectory.DeleteWithContentIfExists().CreateIfNotExists();
        }

        [TestCleanup]
        public void TearDown()
        {
            _workingDirectory.DeleteWithContentIfExists();
        }


        [TestMethod]
        public void ReadEdtTable()
        {
            //Arrange
            CsvReader<GlobalNote> reader = new CsvReader<GlobalNote>();
            //Act
            var data = reader.Read(_edtTableCsv, new CsvReaderOptions() { CsvDelimiter = ';', ParsingType = CsvReaderOptions.ParsingStrategy.MappingByOrder }).ToList();

            //Assert
            Assert.AreEqual(data.Count, 1);
            //Assert.AreEqual(FormatLeiMap(data[0]), "582566-22, abcd0001LEI, CSOB, Testing LEI , Česká Písmena");

        }


        [TestMethod]
        public void ReadEdtTableWithMultipleLines()
        {
            //Arrange
            CsvReader<GlobalNote> reader = new CsvReader<GlobalNote>();
            //Act
            var data = reader.Read(_edtTableCsvWithgMultipleLines, new CsvReaderOptions() { CsvDelimiter = ';', ParsingType = CsvReaderOptions.ParsingStrategy.MappingByOrder }).ToList();

            //Assert
            Assert.AreEqual(data.Count, 2);
            //Assert.AreEqual(FormatLeiMap(data[0]), "582566-22, abcd0001LEI, CSOB, Testing LEI , Česká Písmena");

        }


        [TestMethod]
        public void ReadCsvWithHeader()
        {
            //Arrange
            CsvReader<LeiMapping> reader = new CsvReader<LeiMapping>();

            //Act
            var data = reader.Read(_leiMapCsv).ToList();

            //Assert
            Assert.AreEqual(data.Count, 2);
            Assert.AreEqual(FormatLeiMap(data[0]), "582566-22, abcd0001LEI, CSOB, Testing LEI , Česká Písmena");
            Assert.AreEqual(FormatLeiMap(data[1]), "3656 3232, zzz0002LEI, CSSF, '汉语' 漢語");
        }



        [TestMethod]
        public void ReadCsvWithMultipleLinesWithHeader()
        {
            //Arrange
            CsvReader<LeiMapping> reader = new CsvReader<LeiMapping>();

            //Act
            var data = reader.Read(_leiMapCsv).ToList();

            //Assert
            Assert.AreEqual(data.Count, 2);
            Assert.AreEqual(FormatLeiMap(data[0]), "582566-22, abcd0001LEI, CSOB, Testing LEI , Česká Písmena");
            Assert.AreEqual(FormatLeiMap(data[1]), "3656 3232, zzz0002LEI, CSSF, '汉语' 漢語");
        }



        [TestMethod]
        public void ReadCsvWithLowerCaseHeader()
        {
            //Arrange
            CsvReader<LeiMapping> reader = new CsvReader<LeiMapping>();

            //Act
            var data = reader.Read(_leiMapCsvLowerCaseHeader).ToList();

            //Assert
            Assert.AreEqual(data.Count, 2);
            Assert.AreEqual(FormatLeiMap(data[0]), "582566-22, abcd0001LEI, CSOB, Testing LEI , Česká Písmena");
        }




        [TestMethod]
        public void ReadEmptyCsvWithHeader()
        {
            //Arrange
            CsvReader<LeiMapping> reader = new CsvReader<LeiMapping>();

            //Act
            var data = reader.Read(_leiMapEmptyCsvHeader).ToList();

            //Assert
            Assert.IsFalse(data.Any());
        }



        [TestMethod]
        [ExpectedException(typeof(FileLoadException))]
        public void ReadEmptyCsv()
        {
            //Arrange
            CsvReader<LeiMapping> reader = new CsvReader<LeiMapping>();

            //Act
            reader.Read(_leiMapEmpty).ToList();
        }


        [TestMethod]
        public void ReadCsvWithoutHeader()
        {
            //Arrange
            CsvReader<LeiMapping> reader = new CsvReader<LeiMapping>();

            //Act
            var data = reader.Read(_leiMapCsvWithoutHeader, new CsvReaderOptions() { HasHeader = false}).ToList();

            //Assert
            Assert.AreEqual(data.Count, 2);
            Assert.AreEqual(FormatLeiMap(data[0]), "582566-22, abcd0001LEI, Testing LEI , Česká Písmena, CSOB");
            Assert.AreEqual(FormatLeiMap(data[1]), "3656 3232, zzz0002LEI, '汉语' 漢語, CSSF");
        }



        [TestMethod]
        [ExtendedExpectedException(typeof(FormatException))]
        public void ReadWithOddNumberOfDoublequotes()
        {
            //Arrange
            CsvReader<LeiMapping> reader = new CsvReader<LeiMapping>();

            //Act
            reader.Read(_leiMapCsvOddQuotes).ToList();
        }


        [TestMethod]
        [ExtendedExpectedException(typeof(FileNotFoundException))]
        public void ReadFromCsvWhichDoesntExists()
        {
            //Arrange
            CsvReader<LeiMapping> reader = new CsvReader<LeiMapping>();

            //Act
            reader.Read(new FileInfo("Dummy.csv")).ToList();
        }


        [TestMethod]
        [ExtendedExpectedException(typeof(ArgumentException))]
        public void ReadFromFileWithWrongExtension()
        {
            //Arrange
            CsvReader<LeiMapping> reader = new CsvReader<LeiMapping>();

            //Act
            reader.Read(_leiMapCsvXml).ToList();
        }



        private string FormatLeiMap(LeiMapping leiMap)
        {
            return $"{leiMap.Code}, {leiMap.Lei}, {leiMap.NcaName}, {leiMap.FullName}";
        }




    }
}
