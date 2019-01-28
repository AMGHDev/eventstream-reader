using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Azure.Documents.ChangeFeedProcessor.PartitionManagement;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ChangeFeedConsole
{
    public class FeedService
    {
        private string CosmosUri = "https://localhost:8081";
        private string CosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private SampleListener Listener { get; }
        private bool ThrowOnErrorId { get; }

        public FeedService(bool throwOnErrorId = false)
        {
            Listener = new SampleListener(new Uri(CosmosUri), CosmosKey);
            ThrowOnErrorId = throwOnErrorId;
        }

        public bool Start()
        {
            Log.Information($"FeedService::Start with ThrowOnErrorId={ThrowOnErrorId}");

            var stream = Listener.Listen();
            stream.Subscribe(onNext: OnNext, onError: OnError, onCompleted: OnCompleted);

            return true;
        }
        public void Stop()
        {
            Log.Information("FeedService::Stop");
            Listener?.Dispose();

            Task.Delay(5000).Wait(); // wait to clear
        }

        private void OnNext(IEnumerable<string> ids)
        {
            foreach (var id in ids)
            {
                if (id.IndexOf("error", StringComparison.CurrentCultureIgnoreCase) > -1 && ThrowOnErrorId)
                    throw new Exception($"Boom on {id}!");
                else
                    Log.Information($"Received doc {id}");
            }
        }
        private void OnCompleted()
        {
            Log.Information("Completed");
        }
        private void OnError(Exception ex)
        {
            Log.Error(ex, "I blew up!");
        }
    }
}
