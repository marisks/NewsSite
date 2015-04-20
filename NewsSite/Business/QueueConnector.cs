using System.Configuration;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace NewsSite.Business
{
    public static class QueueConnector
    {
        public static QueueClient Client;

        public const string Namespace = "epinewssite";
        public const string QueueName = "ImportQueue";

        public static NamespaceManager CreateNamespaceManager()
        {
            var cn = ConfigurationManager
                    .ConnectionStrings["EPiServerAzureEvents"]
                    .ConnectionString;
            return NamespaceManager.CreateFromConnectionString(cn);
        }

        public static void Initialize()
        {
            // Using Http to be friendly with outbound firewalls
            ServiceBusEnvironment.SystemConnectivity.Mode =
                ConnectivityMode.Http;

            var namespaceManager = CreateNamespaceManager();

            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            var messagingFactory = MessagingFactory.Create(
                namespaceManager.Address,
                namespaceManager.Settings.TokenProvider);
            Client = messagingFactory.CreateQueueClient(QueueName);
        }
    }
}