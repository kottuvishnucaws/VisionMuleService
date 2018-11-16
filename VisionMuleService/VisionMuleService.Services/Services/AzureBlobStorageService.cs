using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using VisionMuleService.Services.Interfaces;

namespace VisionMuleService.Services.Services
{
    public class AzureBlobStorageService : IFileStorageService
    {
        string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=visionmuleservicestorage;AccountKey=bUK/SzWXjpD1yzRmT3332bJF7xjBXoVL/+oooknVKfFaxu5tn76pJJBKgcrILZzGNlQolMWiwIxCdewKXqFfsg==;EndpointSuffix=core.windows.net";

        CloudStorageAccount cloudStorageAccount;
        CloudBlobClient cloudBlobClient;
        CloudBlobContainer cloudBlobContainer;
        CloudBlobDirectory serviceDirectory;

        public AzureBlobStorageService(string serviceName)
        {
           cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);

           cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

           cloudBlobContainer = cloudBlobClient.GetContainerReference("scannedimages");

           serviceDirectory = cloudBlobContainer.GetDirectoryReference(serviceName);

        }

        public async Task<string> StoreImageAsync(byte[] frontImage, byte[] backImage = null)
        {
            try
            {
                if (frontImage == null)
                    return null;

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }


                Guid guid = Guid.NewGuid();
                string imageName = guid.ToString();

                CloudBlockBlob cloudBlockBlob = serviceDirectory.GetBlockBlobReference(imageName + "Front.jpeg");
                cloudBlockBlob.Properties.ContentType = "image/jpeg";
                await cloudBlockBlob.UploadFromByteArrayAsync(frontImage, 0, frontImage.Length);

                if (backImage != null)
                {
                    cloudBlockBlob = serviceDirectory.GetBlockBlobReference(imageName + "Back.jpeg");
                    cloudBlockBlob.Properties.ContentType = "image/jpeg";
                    await cloudBlockBlob.UploadFromByteArrayAsync(backImage, 0, backImage.Length);
                }

                return imageName;
            }
            catch (Exception)
            {
                Console.WriteLine("Error : Storing The Images.");
                return null;
            }
        }

        public async Task<bool> MoveToSubFolderAsync(string id, string subFolderName)
        {
            try
            {
                CloudBlobDirectory subDirectory = serviceDirectory.GetDirectoryReference(subFolderName);

                try
                {
                    CloudBlockBlob frontSource = serviceDirectory.GetBlockBlobReference( id + "Front.jpeg");
                    CloudBlockBlob frontTarget = subDirectory.GetBlockBlobReference(id + "Front.jpeg");

                    await frontTarget.StartCopyAsync(frontSource);
                    await frontSource.DeleteAsync();
                }
                catch (Exception) { }

                try
                {
                    CloudBlockBlob backSource = serviceDirectory.GetBlockBlobReference(id + "Back.jpeg");
                    CloudBlockBlob backTarget = subDirectory.GetBlockBlobReference(id + "Back.jpeg");

                    await backTarget.StartCopyAsync(backSource);
                    await backSource.DeleteAsync();
                }
                catch (Exception) { }

                return true;
            }
            catch(Exception)
            {
                Console.WriteLine("Error : Migrating the Images.");
                return false;
            }
        }
    }
}
