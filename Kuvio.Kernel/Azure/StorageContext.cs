using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Kuvio.Kernel.Azure
{
    public class StorageContext
    {
        private readonly CloudStorageAccount _storageAccount;

        public StorageContext(string connectionString)
        {
            _storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        private readonly Dictionary<string, CloudTable> TableReferences = new Dictionary<string, CloudTable>();

        private readonly Dictionary<string, CloudBlobContainer> BlobReferences =
            new Dictionary<string, CloudBlobContainer>();

        private string _accountName;

        public string AccountName
            => _accountName ?? (_accountName = _storageAccount.BlobEndpoint.Host.Split('.').First());

        public CloudTable Table(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) return null;
            if (TableReferences.ContainsKey(tableName)) return TableReferences[tableName];

            var context = _storageAccount.CreateCloudTableClient();
            TableReferences[tableName] = context.GetTableReference(tableName);

            return TableReferences[tableName];
        }

        public CloudBlobContainer Container(string containerName)
        {
            if (BlobReferences.ContainsKey(containerName)) return BlobReferences[containerName];

            var context = _storageAccount.CreateCloudBlobClient();
            BlobReferences[containerName] = context.GetContainerReference(containerName);

            return BlobReferences[containerName];
        }
        
        public static string SafeKey(string key)
        {
            char[] disallowedChars = { '/', '\\', '?', '#', '\t', '\n', '\r' };
            disallowedChars.ToList().ForEach(x => key = key.Replace(x, '-').ToString());
            return key;
        }
    }
}