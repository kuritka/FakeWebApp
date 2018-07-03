using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using ExecutiveOffice.EDT.GlobalNotesService.Extensions;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using ExecutiveOffice.EDT.GlobalNotesService.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using ExecutiveOffice.EDT.GlobalNotesService.Infrsatructure.Azure;

namespace ExecutiveOffice.EDT.GlobalNotesService.Controllers
{
    [Route("[action]")]
    public partial class GlobalNotesController : Controller
    {
        private IConfiguration _configuration;
        private IGlobalNotesProcessor _globalNotesProcessor;

        private ILogger _logger;

        public GlobalNotesController(IConfiguration configuration, ILogger<GlobalNotesController> logger, IGlobalNotesProcessor globalNotesProcessor)
        {

            _globalNotesProcessor = globalNotesProcessor ?? throw new ArgumentNullException(nameof(globalNotesProcessor));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }




        [HttpGet("UploadFile")]
        public async Task<IActionResult> ReadFromTableStorage()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_configuration.ReadKeyFromFilePath(Constants.ConnectionStringPathKey));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable etdTable = tableClient.GetTableReference("EdtTable");

            var partitionKey = "P001-7554-VL344E-2.csv";

            TableQuery<GlobalNoteAzureEntity> query
                = new TableQuery<GlobalNoteAzureEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            var queryResponse = Task.Run(async () => await etdTable.ExecuteQuerySegmentedAsync(query, null));

            var tableContinuationToken = queryResponse.Result.ContinuationToken;

            var results = queryResponse.Result.Results;

            var globalNotes = results.Select(d => JsonConvert.DeserializeObject<GlobalNote>(d.GlobalNoteRow)).ToList() ;

            string htmlContent = _globalNotesProcessor.GetGlobalNotes(globalNotes);

            var base64Content = htmlContent.ToBase64Encode();

            var uri = new Uri(_configuration["WkHtmlToPdf:url"]);

            var content = new StringContent(JsonConvert.SerializeObject(new LocalContent { contents = base64Content }).ToString());

            var client = new HttpClient();

            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await client.PostAsync(uri.AbsoluteUri, content);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"returned code: {response.StatusCode};");
                _logger.LogError($"content: {response.Content};");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();

            _logger.LogInformation($"Sucessfully received, creating file {Path.Combine(Directory.GetCurrentDirectory(), "file.pdf")} ");



            Stream stream = new MemoryStream(bytes);

            new AzureRepository().UploadFileStream(stream, _configuration["ConnectionString"], new FileInfo(Path.Combine("HyperArchive", $"GlobalNotes_{DateTime.UtcNow.ToString("yyyyMMdd_hhmmss")}.pdf")));

            var fileResponse = new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = "GlobalNotes.pdf"
            };

            return fileResponse;


        }


        [HttpGet("UploadFile")]
        public async Task<IActionResult> PostGlobalNotes([FromBody]List<GlobalNote> file)
        {
            try
            {
                string htmlContent = _globalNotesProcessor.GetGlobalNotes(new List<GlobalNote>() {
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
                    Isin = string.Format("EN00000054000212AA1"),
                    NumberOfShares = "24.000.000",
                    DistributionCurrency = Enum.GetName(typeof(Currency), Currency.EUR),
                    Issuer = "Landesbank Baden-Württemberg",
                    ProductName = "2,5 % LBBW Commerzbank Aktien-Anleihe",
                    Language= "EN",
                },
                });                

                var base64Content = htmlContent.ToBase64Encode();

                var uri = new Uri(_configuration["WkHtmlToPdf:url"]);

                var content = new StringContent(JsonConvert.SerializeObject(new LocalContent { contents = base64Content }).ToString());

                var client = new HttpClient();

                content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                var response = await client.PostAsync(uri.AbsoluteUri, content);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _logger.LogError($"returned code: {response.StatusCode};");
                    _logger.LogError($"content: {response.Content};");
                    return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                }

                var bytes = await response.Content.ReadAsByteArrayAsync();

                _logger.LogInformation($"Sucessfully received, creating file {Path.Combine(Directory.GetCurrentDirectory(), "file.pdf")} ");


                Stream stream = new MemoryStream(bytes);

                var fileResponse = new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = "GlobalNotes.pdf"
                };
                
                return fileResponse;
                //System.IO.File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "file.pdf"), bytes);

                //return new StatusCodeResult((int)HttpStatusCode.OK);
            }catch(Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                _logger.LogError($"Stack Trace : {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException.Message);
                }
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }


        private sealed class LocalContent
        {

            public string contents { get; set; }
        }

    }
}
