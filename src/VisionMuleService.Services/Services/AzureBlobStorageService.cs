using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisionMuleService.Services.Interfaces;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace VisionMuleService.Services.Services
{
    public class AzureBlobStorageService : IFileStorageService
    {
        string storageConnection = "DefaultEndpointsProtocol=https;AccountName=visionmuleservicestorage;AccountKey=bUK/SzWXjpD1yzRmT3332bJF7xjBXoVL/+oooknVKfFaxu5tn76pJJBKgcrILZzGNlQolMWiwIxCdewKXqFfsg==;EndpointSuffix=core.windows.net";
        CloudStorageAccount cloudStorageAccount;

        //create a block blob 
        CloudBlobClient cloudBlobClient;
        public AzureBlobStorageService()
        {
           cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);

           cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }

        public async Task<string> Store(Stream image)
        {
            try
            {


                //create a container
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("scannedimages");

                //create a container if it is not already exists

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }
                Guid guid = Guid.NewGuid();
                string imageName = guid.ToString();

                //get Blob reference

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
                //cloudBlockBlob.Properties.ContentType = "jpeg";

                await cloudBlockBlob.UploadFromStreamAsync(image);

                return imageName;
            }
            catch (Exception ex)
            {
                return "Error..!";
            }
        }

        public async void MoveToSubFolder(string id, string subFolderName)
        {
            CloudBlobContainer sourceBlobContainer = cloudBlobClient.GetContainerReference("scannedimages");
            CloudBlockBlob srcBlob = sourceBlobContainer.GetBlockBlobReference(id);

            CloudBlobContainer destBlobContainer = cloudBlobClient.GetContainerReference(subFolderName.ToString());
            CloudBlockBlob destBlob;


            if (srcBlob == null)
            {
                throw new Exception("Source blob cannot be null.");
            }

            //if (!destBlobContainer.Exists())
            //{
            //    throw new Exception("Destination container does not exist.");
            //}

            //Copy source blob to destination container
            string name = srcBlob.Uri.Segments.Last();
            destBlob = destBlobContainer.GetBlockBlobReference(name);
            await destBlob.StartCopyAsync(srcBlob);
            //remove source blob after copy is done.
            await srcBlob.DeleteAsync();
        }
    }
}
