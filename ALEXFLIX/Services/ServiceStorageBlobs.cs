using ALEXFLIX.Helpers;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using NuGetAlexflixModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ALEXFLIX.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient serviceBlobs;
        String containername;


        public ServiceStorageBlobs(String keys)
        {
            this.serviceBlobs = new BlobServiceClient(keys);
            this.containername = "alexflix";
        }

        public async Task UploadBlobAsync(Folders carpeta, string fileName, Stream stream)
        {
            BlobContainerClient container = this.serviceBlobs.GetBlobContainerClient(this.containername);
            BlobClient blobClient = container.GetBlobClient(carpeta.ToString().ToLower() + "/" + fileName);
            if (carpeta == Folders.Images)
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = "image/jpeg" });

            }
            else if (carpeta == Folders.Videos)
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = "video/mp4" });
            }
        }
    }
}
