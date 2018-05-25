using FakerTest.Infrsatructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace FakerTest.Tests
{
    public class CsvImportTest
    {
        public void ImportCsv()
        {
            CsvInputFormatter formatter = new CsvInputFormatter(new CsvFormatterOptions() { CsvDelimiter = ";" });

        } 

    }
}
