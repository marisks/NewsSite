using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Contracts;
using EPiServer.BaseLibrary.Scheduling;
using EPiServer.PlugIn;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace NewsSite.Business
{
    [ScheduledPlugIn(DisplayName = "Init import", SortIndex = 2000)]
    public class ImportInitializationJob : JobBase
    {
        public override string Execute()
        {
            var container = CreateStorageContainer();

            foreach (var item in container.ListBlobs()
                                            .OfType<CloudBlockBlob>())
            {
                var importFile = new ImportFile
                {
                    Name = item.Name, Uri = item.Uri
                };
                var message = new BrokeredMessage(importFile);
                QueueConnector.Client.Send(message);
            }

            return "Success";
        }

        private const string ContainerName = "epiimportdata";

        private static CloudBlobContainer CreateStorageContainer()
        {
            var cn = ConfigurationManager
                            .ConnectionStrings["EPiServerAzureBlobs"]
                            .ConnectionString;
            var storageAccount = CloudStorageAccount.Parse(cn);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(ContainerName);
            container.CreateIfNotExists();
            return container;
        }
    }
}