using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kuvio.Kernel.Architecture;
using Microsoft.WindowsAzure.Storage.Blob;
using PhotosOfUs.Model.Models;

namespace PhotosOfUs.Connectors.Storage
{
    public class MediaRepository<T> : IMediaRepository<T> where T : IFile
    {
        private readonly StorageContext context;

        public MediaRepository(StorageContext context)
        {
            this.context = context;
        }

        public async Task<Uri> UploadAsync(T item)
        {
            item.Stream.Position = 0;
            var containerBlob = Get(item);
            await containerBlob.UploadFromStreamAsync(item.Stream);
            return containerBlob.Uri;
        }

        private CloudBlockBlob Get(T item)
        {
            return context.Container(item.FolderName).GetBlockBlobReference(item.Filename);
        }
    }
}