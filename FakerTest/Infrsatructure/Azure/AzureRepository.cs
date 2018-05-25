using FakerTest.Extensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FakerTest.Infrsatructure.Azure
{


    public class AzureRepository 
        :  IAzureRepository
    {


        public void UploadFileStream(Stream stream, string connectionString, FileInfo outputFile)
        {
            connectionString.ThrowExceptionIfNullOrEmpty();
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

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
                    CloudFileDirectory customDirectory = rootDirectory.GetDirectoryReference("HyperArchive");
                    if (Task.Run(async () => await customDirectory.ExistsAsync()).Result)
                    {
                        CloudFile file = customDirectory.GetFileReference(outputFile.Name);
                        string sasToken = file.GetSharedAccessSignature(null, policyName);

                        //generate URL of file with SAS token  
                        Uri fileSASUrl = new Uri(file.StorageUri.PrimaryUri.ToString() + sasToken);
                        CloudFile newFile = new CloudFile(fileSASUrl);
                       
                        var taskResult = Task.Run(async () => await newFile.UploadFromStreamAsync(stream));
                    }
                }
            }


        }


        //public void UploadFile(FileInfo inputFile, string connectionString, string sharedReference, string directory)
        //{

        //    connectionString.ThrowExceptionIfNullOrEmpty();
        //    inputFile.ThrowExceptionIfNullOrDoesntExists()
        //        .ThrowExceptionIfFileSizeExceedsMB(10);

        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

        //    CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

        //    CloudFileShare fileShare = fileClient.GetShareReference(sharedReference);

        //    if (Task.Run(async () => await fileShare.ExistsAsync()).Result)
        //    {
        //        string policyName = "DemoPolicy" + new Random().Next(50);

        //        FileSharePermissions fileSharePermissions = Task.Run(async () => await fileShare.GetPermissionsAsync()).Result;

        //        // define policy   
        //        SharedAccessFilePolicy sharedAccessFilePolicy = new SharedAccessFilePolicy()
        //        {
        //            SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1),
        //            //Permissions = SharedAccessFilePermissions.Read
        //            Permissions = SharedAccessFilePermissions.Write
        //        };

        //        fileSharePermissions.SharedAccessPolicies.Add(policyName, sharedAccessFilePolicy);

        //        // set permissions of file share  
        //        Task.Run(async () => await fileShare.SetPermissionsAsync(fileSharePermissions));

        //        // generate SAS token based on policy and use to create a new file  
        //        CloudFileDirectory rootDirectory = fileShare.GetRootDirectoryReference();

        //        if (Task.Run(async () => await rootDirectory.ExistsAsync()).Result)
        //        {
        //            CloudFileDirectory customDirectory = rootDirectory.GetDirectoryReference(directory);
        //            if (Task.Run(async () => await customDirectory.ExistsAsync()).Result)
        //            {
        //                CloudFile file = customDirectory.GetFileReference(inputFile.Name);
        //                string sasToken = file.GetSharedAccessSignature(null, policyName);

        //                //generate URL of file with SAS token  
        //                Uri fileSASUrl = new Uri(file.StorageUri.PrimaryUri.ToString() + sasToken);
        //                CloudFile newFile = new CloudFile(fileSASUrl);

        //                Task.Run(async () => await newFile.UploadFromFileAsync(inputFile.FullName));
        //            }
        //        }
        //    }
        //}

       
    }
}
