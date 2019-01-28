using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Azure.Documents.ChangeFeedProcessor.PartitionManagement;
using Microsoft.Azure.Documents.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeFeedConsole
{
    public class SampleListener : IDisposable
    {
        private List<Tuple<IObserver<IEnumerable<string>>, IChangeFeedProcessor>> Subscriptions { get; }
        private Uri CosmosUri { get; }
        private string CosmosKey { get; }
        private string DatabaseName { get; }
        private string MonitoredCollection { get; }
        private string LeaseCollection { get; }

        public SampleListener(Uri cosmosUri, string cosmosKey, string databaseName = "Sample", string monitoredCollection = "_Monitored", string leaseCollection = "_Leases")
        {
            CosmosUri = cosmosUri;
            CosmosKey = cosmosKey;
            DatabaseName = databaseName;
            MonitoredCollection = monitoredCollection;
            LeaseCollection = leaseCollection;
            Subscriptions = new List<Tuple<IObserver<IEnumerable<string>>, IChangeFeedProcessor>>();
        }

        public IObservable<IEnumerable<string>> Listen()
        {
            CreateStoresIfNotExistsAsync().Wait();

            var stream = Observable.Create<IEnumerable<string>>(obs =>
            {
                var factory = new SampleObserverFactory(obs);
                var processor = new ChangeFeedProcessorBuilder()
                                        .WithFeedCollection(new DocumentCollectionInfo { Uri = CosmosUri, MasterKey = CosmosKey, DatabaseName = DatabaseName, CollectionName = MonitoredCollection })
                                        .WithLeaseCollection(new DocumentCollectionInfo { Uri = CosmosUri, MasterKey = CosmosKey, DatabaseName = DatabaseName, CollectionName = LeaseCollection })
                                        .WithHostName("SampleHost")
                                        .WithObserverFactory(factory)
                                        /* the explicit checkpoint is important because otherwise when the stream blows, it still checkpoints, and we 'lose' the message */
                                        .WithProcessorOptions(new ChangeFeedProcessorOptions { MaxItemCount = 2, CheckpointFrequency = new CheckpointFrequency { ExplicitCheckpoint = true } })
                                        .BuildAsync().Result;

                processor.StartAsync().Wait();

                var sub = Tuple.Create(obs, processor);
                Subscriptions.Add(sub);

                return Disposable.Create(() =>
                {
                    Subscriptions.Remove(sub);
                    processor.StopAsync().Wait();
                });
            });

            Log.Information("Subscription created");
            return stream;
        }


        private async Task CreateStoresIfNotExistsAsync()
        {
            using (var client = new DocumentClient(CosmosUri, CosmosKey))
            {
                await client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });

                await CreateCollectionIfNotExistsAsync(client, MonitoredCollection);
                await CreateCollectionIfNotExistsAsync(client, LeaseCollection);
            }
        }
        private async Task CreateCollectionIfNotExistsAsync(DocumentClient client, string collectionName)
        {
            var col = new DocumentCollection
            {
                Id = collectionName,
            };

            var partKey = "/id";
            if (!partKey.StartsWith("/")) partKey = "/" + partKey;
            col.PartitionKey.Paths.Add(partKey);

            await client.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseName),
                col,
                new RequestOptions { OfferThroughput = 400 });
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Subscriptions.ToArray().ForAll(s =>
                    {
                        Log.Warning("Disposing subscription");
                        s.Item1?.OnCompleted();
                        s.Item2?.StopAsync().Wait();
                        return true;
                    });
                    Log.Warning("All subscriptions disposed");
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
