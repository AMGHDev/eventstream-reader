using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor.FeedProcessing;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeFeedConsole
{
    public class SampleObserver : IChangeFeedObserver
    {
        private SampleProcessor Processor { get; }
        public SampleObserver(IObserver<IEnumerable<string>> stream)
        {
            Processor = new SampleProcessor(stream);
        }
        public Task CloseAsync(IChangeFeedObserverContext context, ChangeFeedObserverCloseReason reason)
        {
            Log.Information($"SampleObserver::CloseAsync for {context.PartitionKeyRangeId} with {reason}");
            return Task.CompletedTask;
        }

        public Task OpenAsync(IChangeFeedObserverContext context)
        {            
            Log.Information($"SampleObserver::OpenAsync for {context.PartitionKeyRangeId}");
            return Task.CompletedTask;
        }

        public Task ProcessChangesAsync(IChangeFeedObserverContext context, IReadOnlyList<Document> docs, CancellationToken cancellationToken)
        {
            Log.Information("SampleObserver::ProcessChangesAsync: partition {0}, {1} docs", context.PartitionKeyRangeId, docs.Count);
            return Processor.ProcessBatchAsync(docs, cancellationToken, context.CheckpointAsync);
        }
    }
}
