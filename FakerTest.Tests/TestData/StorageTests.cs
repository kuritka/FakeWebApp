using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using FakerTest.Infrsatructure;
using FakerTest.Entities;
using System.IO;
using FakerTest.Extensions;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.Extensions.Configuration;

namespace FakerTest.Tests.TestData
{
    [TestClass]
    public class StorageTests
    {

        private readonly FileInfo _edtTableCsv = new FileInfo(Path.Combine("TestData", "P001-7554-VL344E.csv"));
        private readonly FileInfo _globalNotesPdf = new FileInfo(Path.Combine("TestData", "GlobalNotes.pdf"));
        private readonly FileInfo _edtTableCsvWithMultipleLines = new FileInfo(Path.Combine("TestData", "P001-7554-VL344E-2.csv"));
        private readonly DirectoryInfo _workingDirectory = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "CsvTest"));
        private IConfiguration _configuration ;


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
        public void AddOrUpdateGlobalNoteInTableStorage()
        {

            var table = _edtTableCsvWithMultipleLines;

            CsvReader<GlobalNote> reader = new CsvReader<GlobalNote>();

            var globalNotes = reader.Read(table, new CsvReaderOptions() { CsvDelimiter = ';', ParsingType = CsvReaderOptions.ParsingStrategy.MappingByOrder });

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_configuration.ReadKeyFromFilePath(Constants.ConnectionStringPathKey));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable etdTable = tableClient.GetTableReference("EdtTable");
            var createResult = Task.Run(async () => await etdTable.CreateIfNotExistsAsync()).Result;


            var results = new List<TableResult>();

            var insertedRecords = globalNotes.AsGlobalNoteAzureEntities();
            foreach (var insertedRecord in insertedRecords)
            {
                insertedRecord.State = ProcessedState.Archived;
                insertedRecord.PartitionKey = table.Name;
                insertedRecord.RowKey = $"{results.Count}";
                TableOperation insertOperation = TableOperation.InsertOrReplace(insertedRecord);
                var insertResult = Task.Run(async () => await etdTable.ExecuteAsync(insertOperation)).Result;
                results.Add(insertResult);
            }    
        }



        [TestMethod]
        public void ReadFromTableStorage()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_configuration.ReadKeyFromFilePath(Constants.ConnectionStringPathKey));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable etdTable = tableClient.GetTableReference("EdtTable");

            var partitionKey = _edtTableCsv.Name;

            TableQuery<GlobalNoteAzureEntity> query
                = new TableQuery<GlobalNoteAzureEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            var queryResponse = Task.Run(async () => await etdTable.ExecuteQuerySegmentedAsync(query, null));

            var tableContinuationToken = queryResponse.Result.ContinuationToken;

            var result = queryResponse.Result.Results.First();

            var globalNote = JsonConvert.DeserializeObject<GlobalNote>(result.GlobalNoteRow);
        }




        [TestMethod]
        public void ReadAllFromTableStorage()
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_configuration.ReadKeyFromFilePath(Constants.ConnectionStringPathKey));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable etdTable = tableClient.GetTableReference("EdtTable");

            var partitionKey = _edtTableCsvWithMultipleLines.Name;

            TableQuery<GlobalNoteAzureEntity> query
                = new TableQuery<GlobalNoteAzureEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            var queryResponse = Task.Run(async () => await etdTable.ExecuteQuerySegmentedAsync(query, null));

            var tableContinuationToken = queryResponse.Result.ContinuationToken;

            var results = queryResponse.Result.Results;

            var globalNotes   =  results.Select(d => JsonConvert.DeserializeObject<GlobalNote>(d.GlobalNoteRow)).ToList();
        }


        [TestMethod]
        public void WriteFilesIntoFileService()
        {
           CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_configuration.ReadKeyFromFilePath(Constants.ConnectionStringPathKey));

            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            CloudFileShare fileShare = fileClient.GetShareReference("h2h");

            if (Task.Run(async () => await fileShare.ExistsAsync()).Result)
            {
                string policyName = "DemoPolicy" + new Random().Next(50);

                FileSharePermissions fileSharePermissions = Task.Run(async () => await fileShare.GetPermissionsAsync()).Result;

                // define policy   
                SharedAccessFilePolicy sharedAccessFilePolicy = new SharedAccessFilePolicy()
                {
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
                    //Permissions = SharedAccessFilePermissions.Read
                    Permissions = SharedAccessFilePermissions.Write
                };

                fileSharePermissions.SharedAccessPolicies.Add(policyName, sharedAccessFilePolicy);

                // set permissions of file share  
                Task.Run(async () => await fileShare.SetPermissionsAsync(fileSharePermissions));

                // generate SAS token based on policy and use to create a new file  
                CloudFileDirectory rootDirectory = fileShare.GetRootDirectoryReference();

                if (Task.Run(async () => await rootDirectory.ExistsAsync()).Result)
                {
                    CloudFileDirectory customDirectory = rootDirectory.GetDirectoryReference("Output");
                    if (Task.Run(async () => await customDirectory.ExistsAsync()).Result)
                    {
                        CloudFile file = customDirectory.GetFileReference(_globalNotesPdf.Name);
                        string sasToken = file.GetSharedAccessSignature(null, policyName);

                        //generate URL of file with SAS token  
                        Uri fileSASUrl = new Uri(file.StorageUri.PrimaryUri.ToString() + sasToken);
                        CloudFile newFile = new CloudFile(fileSASUrl);

                        Task.Run(async () => await newFile.UploadFromFileAsync(_globalNotesPdf.FullName));
                    }
                }
            }
        }

        

    }
}
