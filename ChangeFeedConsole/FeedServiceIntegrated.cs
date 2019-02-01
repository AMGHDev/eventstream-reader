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
    public class FeedServiceIntegrated
    {
        private string CosmosUri = "https://localhost:8081";
        private string CosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private SampleIntegrationListener Integrator { get; }

        public FeedServiceIntegrated(bool throwOnErrorId = false)
        {
            Integrator = new SampleIntegrationListener(new SampleListener(new Uri(CosmosUri), CosmosKey, "Sample", "_Monitored", "_Leases"), throwOnErrorId);
        }

        public bool Start()
        {
            Log.Information($"FeedServiceIntegrated::Start");
            Integrator.StartListening();
            return true;
        }
        public void Stop()
        {
            Log.Information("FeedServiceIntegrated::Stop");
            Integrator?.Dispose();

            Task.Delay(5000).Wait(); // wait to clear

            Console.ReadKey();
        }    
    }
}
