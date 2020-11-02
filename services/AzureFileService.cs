using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Mmt.Api.DTO.Curiosity;
using Mmt.Api.DTO.Photo;
using Mmt.Api.DTO.Tours;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.services
{
    public interface IAzureFileService
    {
        Task<string> ProcessFotoAsync(CuriositiesPostDTO model);
        Task<string> DeleteCuriosityImageAsync(string curiosityImg);
        Task<string> ProcessFotoAsync(CuriositiesPutDTO model);

        Task<string> ProcessPhotoGalleryForCuriosityAsync(PhotosPostDTO model);
        Task<string> DeleteCuriosityPhotoGalleryAsync(string curiosityImg);

        //tours
        Task<string> ProcessTourImgAsync(TourPostDTO model);
        Task<string> DeleteTourImageAsync(string tourImg);
        Task<string> ProcessTourImgAsync(TourPutDTO model);

    }
    public class AzureFileService : IAzureFileService
    {
        private readonly IConfiguration _Configuration;
        public string StorageConnectionString { get; private set; }
        public BlobServiceClient BlobServiceClient { get; set; }

        public AzureFileService(IConfiguration configuration)
        {
            _Configuration = configuration;
            StorageConnectionString = _Configuration["ConnectionStrings:AzureStorage"];
            BlobServiceClient = new BlobServiceClient(StorageConnectionString);
        }

        public async Task<string> ProcessFotoAsync(CuriositiesPostDTO model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;

                // Retrieve storage account from connection string.
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("images");
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(uniqueFileName);
                await using (var data = model.Image.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(data);
                }
            }

            return uniqueFileName;
        }

        public async Task<string> ProcessFotoAsync(CuriositiesPutDTO model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;

                // Retrieve storage account from connection string.
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("images");
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(uniqueFileName);
                await using (var data = model.Image.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(data);
                }
            }

            return uniqueFileName;
        }

        public async Task<string> DeleteCuriosityImageAsync(string curiosityImg)
        {
            if (curiosityImg != null)
            {

                // Retrieve storage account from connection string.
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("images");
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(curiosityImg);
                await blockBlob.DeleteIfExistsAsync();
            }

            return curiosityImg;
        }



        //photo gallery
        public async Task<string> ProcessPhotoGalleryForCuriosityAsync(PhotosPostDTO model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;

                // Retrieve storage account from connection string.
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("gallery");
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(uniqueFileName);
                await using (var data = model.Photo.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(data);
                }
            }

            return uniqueFileName;
        }

        public async Task<string> DeleteCuriosityPhotoGalleryAsync(string curiosityImg)
        {
            if (curiosityImg != null)
            {

                // Retrieve storage account from connection string.
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("gallery");
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(curiosityImg);
                await blockBlob.DeleteIfExistsAsync();
            }

            return curiosityImg;
        }


        //tours images
        public async Task<string> ProcessTourImgAsync(TourPostDTO model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;

                // Retrieve storage account from connection string.
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("tourimages");
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(uniqueFileName);
                await using (var data = model.Image.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(data);
                }
            }

            return uniqueFileName;
        }

        public async Task<string> ProcessTourImgAsync(TourPutDTO model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;

                // Retrieve storage account from connection string.
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("tourimages");
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(uniqueFileName);
                await using (var data = model.Image.OpenReadStream())
                {
                    await blockBlob.UploadFromStreamAsync(data);
                }
            }

            return uniqueFileName;
        }

        public async Task<string> DeleteTourImageAsync(string tourImg)
        {
            if (tourImg != null)
            {

                // Retrieve storage account from connection string.
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                // Create the blob client.
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("tourimages");
                // This also does not make a service call; it only creates a local object.
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(tourImg);
                await blockBlob.DeleteIfExistsAsync();
            }

            return tourImg;
        }


    }
}
