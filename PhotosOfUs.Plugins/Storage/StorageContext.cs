using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using PhotosOfUs.Connectors.Storage;

namespace PhotosOfUs.Connectors.Storage
{
    public class StorageContext
    {
        private readonly CloudStorageAccount _storageAccount;

        public StorageContext(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        private readonly Dictionary<string, CloudBlobContainer> BlobReferences =
            new Dictionary<string, CloudBlobContainer>();

        public CloudBlobContainer Container(string containerName)
        {
            if (BlobReferences.ContainsKey(containerName)) return BlobReferences[containerName];

            var context = _storageAccount.CreateCloudBlobClient();
            BlobReferences[containerName] = context.GetContainerReference(containerName);

            return BlobReferences[containerName];
        }
    }
}