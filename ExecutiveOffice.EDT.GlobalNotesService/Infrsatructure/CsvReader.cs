using ExecutiveOffice.EDT.GlobalNotesService.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ExecutiveOffice.EDT.GlobalNotesService.Infrsatructure
{

    public class CsvReaderOptions
    {
        public enum ParsingStrategy
        {
            MappingNamesWithHeaders,

            MappingByOrder
        }

        public bool HasHeader { get; set; } = true;

        public char CsvDelimiter { get; set; } = ',';

        public string Encoding { get; set; } = "ISO-8859-1";

        public ParsingStrategy ParsingType { get; set; } = ParsingStrategy.MappingNamesWithHeaders;

    }

    public class CsvReader<T> where T : new()
    {
        private  CsvReaderOptions _options;


        public IEnumerable<T> Read(FileInfo fileInfo)
        {
            CsvReaderOptions options = new CsvReaderOptions();
            return Read(fileInfo, options);
        }

        public IEnumerable<T> Read(FileInfo fileInfo, CsvReaderOptions options)
        {
            fileInfo
                .ThrowExceptionIfNullOrDoesntExists()
                .ThrowExceptionIfExtensionIsDifferentFrom(Constants.FileExtensions.Csv);

            _options = options;

            var objects = new List<T>();
            var headers = new List<string>();
            using (var sr = new StreamReader(fileInfo.FullName, Encoding.GetEncoding(options.Encoding)))
            {
                var headersRead = !_options.HasHeader;
                string line;
                do
                {
                    line = sr.ReadLine();
                    if (line != null && headersRead)
                    {
                        if (line.ToCharArray().Count(d => d == '"') % 2 == 1)
                        {
                            throw new FormatException($"Odd number of double quotes in line ${line}");
                        }
                        var propertyValues = Regex.Split(line, _options.CsvDelimiter +"(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")
                            .Select(d => d.Replace("\"", string.Empty)).ToArray();
                        var obj = AssignValuesFromCsv(propertyValues, headers);
                        objects.Add(obj);
                    }
                    if (!headersRead)
                    {
                        headersRead = true;
                        if (line == null) throw new FileLoadException($"{fileInfo.Name} is invalid. Probably it is empty...");
                        headers = line.Split(_options.CsvDelimiter).ToList();
                    }
                } while (line != null);
            }
            return objects;
        }



        private T AssignValuesFromCsv(string[] propertyValues, List<string> headers)
        {
            var obj = new T();
            var properties = obj.GetType().GetProperties().Where(d => d.Name.ToUpperInvariant() != "ID" && d.CanWrite).ToArray();
            var upperHeaders = headers.Select(d => d.ToUpperInvariant()).ToList();
            for (var i = 0; i < properties.Length; i++)
            {
                var type = properties[i].PropertyType.Name;
                int index;
                if (_options.ParsingType == CsvReaderOptions.ParsingStrategy.MappingNamesWithHeaders)
                {
                    index = !upperHeaders.IsNullOrEmpty() ? upperHeaders.IndexOf(properties[i].Name.ToUpperInvariant()) : i;
                    if (index == -1) break;
                }
                else
                {
                    index = i;
                }
                switch (type)
                {
                    case "Int32":
                        properties[i].SetValue(obj,
                            int.Parse(propertyValues[index]));
                        break;
                    default:
                        properties[i].SetValue(obj, propertyValues[index]?.EncodeTo(Encoding.UTF8));
                        break;
                }
            }
            return obj;
        }
    }
}
