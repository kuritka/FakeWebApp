using ExecutiveOffice.EDT.GlobalNotesService.Infrsatructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExecutiveOffice.EDT.GlobalNotesService.Tests
{
    public class CsvImportTest
    {
        public void ImportCsv()
        {
            CsvInputFormatter formatter = new CsvInputFormatter(new CsvFormatterOptions() { CsvDelimiter = ";" });

        } 

    }
}
