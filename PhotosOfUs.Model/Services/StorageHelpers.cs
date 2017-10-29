using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Configuration;

namespace PhotosOfUs.Model.Services
{
    public static class StorageHelpers
    {
        public static string SafeKey(string key)
        {
            char[] disallowedChars = { '/', '\\', '?', '#', '\t', '\n', '\r' };
            return key.Replace(disallowedChars, "-").ToString();
        }

        // Load from the 
        private static readonly CloudStorageAccount StorageAccount =
            CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=photosofus;AccountKey=;EndpointSuffix=core.windows.net");

        private static readonly Dictionary<string, CloudTable> TableReferences = new Dictionary<string, CloudTable>();

        private static readonly Dictionary<string, CloudBlobContainer> BlobReferences =
            new Dictionary<string, CloudBlobContainer>();

        private static string _accountName;

        public static string AccountName
            => _accountName ?? (_accountName = StorageAccount.BlobEndpoint.Host.Split('.').First());

        public static CloudTable Table(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) return null;
            if (TableReferences.ContainsKey(tableName)) return TableReferences[tableName];

            var context = StorageAccount.CreateCloudTableClient();
            TableReferences[tableName] = context.GetTableReference(tableName);

            return TableReferences[tableName];
        }

        public static CloudBlobContainer Container(string containerName)
        {
            if (BlobReferences.ContainsKey(containerName)) return BlobReferences[containerName];

            var context = StorageAccount.CreateCloudBlobClient();
            BlobReferences[containerName] = context.GetContainerReference(containerName);

            return BlobReferences[containerName];
        }
        
    }

  
}