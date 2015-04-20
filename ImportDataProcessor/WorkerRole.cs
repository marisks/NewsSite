using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImportDataProcessor
{
    public class WorkerRole : RoleEntryPoint
    {

        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            InClient.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        Trace.WriteLine("Processing Service Bus message: " + 
                                         receivedMessage.SequenceNumber.ToString());

                        var importFile = receivedMessage.GetBody<ImportFile>();
                        var container = CreateStorageContainer();
                        var blob = container.GetBlockBlobReference(importFile.Name);

                        var articles = ReadArticles(blob).ToList();

                        articles.ForEach(article =>
                        {
                            var message = new BrokeredMessage(article);
                            OutClient.Send(message);
                        });

                        receivedMessage.Complete();
                    }
                    catch(Exception ex)
                    {
                        Trace.TraceError("Exception: {0} \n Stack Trace: {1}",
                                            ex.Message, ex.StackTrace);
                    }
                });

            CompletedEvent.WaitOne();
        }

        private static IEnumerable<Article> ReadArticles(CloudBlockBlob blob)
        {
            var text = blob.DownloadText();
            
            using (var sr = new StringReader(text))
            {
                string line;
                var row = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    row++;
                    if (row == 1) continue;

                    var fields = line.Split(',');
                    yield return new Article
                    {
                        Name = fields[0],
                        Intro = fields[1],
                        Content = fields[2],
                        ImageUrl = fields[3]
                    };
                }
            }
        }

        QueueClient InClient;
        QueueClient OutClient;

        const string InQueueName = "ImportQueue";
        const string OutQueueName = "ImportArticleQueue";

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var cn = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(cn);
            if (!namespaceManager.QueueExists(InQueueName))
            {
                namespaceManager.CreateQueue(InQueueName);
            }

            if (!namespaceManager.QueueExists(OutQueueName))
            {
                namespaceManager.CreateQueue(OutQueueName);
            }

            InClient = QueueClient.CreateFromConnectionString(cn, InQueueName);
            OutClient = QueueClient.CreateFromConnectionString(cn, OutQueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            InClient.Close();
            CompletedEvent.Set();
            base.OnStop();
        }

        private const string ContainerName = "epiimportdata";
        private static CloudBlobContainer CreateStorageContainer()
        {
            var connectionString = CloudConfigurationManager.GetSetting("EPiServerAzureBlobs");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(ContainerName);
            container.CreateIfNotExists();
            return container;
        }
    }
}
