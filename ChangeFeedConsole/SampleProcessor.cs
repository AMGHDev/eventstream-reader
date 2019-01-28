using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor.PartitionManagement;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeFeedConsole
{
    public class SampleProcessor
    {
        private IObserver<IEnumerable<string>> Stream { get; }

        public SampleProcessor(IObserver<IEnumerable<string>> stream)
        {
            Stream = stream;
        }

        public async Task ProcessBatchAsync(IReadOnlyList<Document> docs, CancellationToken cancellationToken = default(CancellationToken), Func<Task> checkpoint = null)
        {
            var ids = docs.Select(x => x.Id);
            try
            {
                Log.Information("SampleProcessor: About to OnNext");
                Stream.OnNext(ids);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "SampleProcessor: OnNext error");
                Stream.OnError(ex);
                throw;
            }

            if (checkpoint != null)
            {
                Log.Information($"***Checkpointing on {docs.Count}");
                await checkpoint();
            }
        }
    }
}
