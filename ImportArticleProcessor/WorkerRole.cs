using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using Contracts;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json;

namespace ImportArticleProcessor
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            InClient.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        // Process the message
                        Trace.WriteLine("Processing Service Bus message: " 
                                        + receivedMessage.SequenceNumber.ToString());

                        var article = receivedMessage.GetBody<Article>();

                        using (var client = CreateClient())
                        {
                            var str = JsonConvert.SerializeObject(article);
                            var content = new StringContent(str, Encoding.UTF8, "text/json");
                            var result = client.PostAsync("api/article", content).Result;
                            result.EnsureSuccessStatusCode();
                        }
                    }
                    catch
                    {
                        // Handle any message processing specific exceptions here
                    }
                });

            CompletedEvent.WaitOne();
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient {BaseAddress = new Uri("http://epinewssite.azurewebsites.net/")};
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        const string InQueueName = "ImportArticleQueue";

        QueueClient InClient;

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var cn = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(cn);
            if (!namespaceManager.QueueExists(InQueueName))
            {
                namespaceManager.CreateQueue(InQueueName);
            }

            InClient = QueueClient.CreateFromConnectionString(cn, InQueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            InClient.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}
