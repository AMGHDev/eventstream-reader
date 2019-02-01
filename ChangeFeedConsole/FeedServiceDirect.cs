using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeFeedConsole
{
    public class FeedServiceDirect
    {
        private string CosmosUri = "https://localhost:8081";
        private string CosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private SampleListener Listener { get; }
        private bool ThrowOnErrorId { get; }

        public FeedServiceDirect(bool throwOnErrorId)
        {
            ThrowOnErrorId = throwOnErrorId;
            Listener = new SampleListener(new Uri(CosmosUri), CosmosKey);
            EventHandlers.ThrowOnErrorId = throwOnErrorId;
        }

        public bool Start()
        {            
            Log.Information($"FeedServiceDirect::Start");
            var obs = Listener.Listen();
            obs.Subscribe(onNext: EventHandlers.OnNext, onCompleted: EventHandlers.OnCompleted, onError: EventHandlers.OnError);
            return true;
        }
        public void Stop()
        {
            Log.Information("FeedServiceDirect::Stop");
            Listener?.Dispose();

            Task.Delay(5000).Wait(); // wait to clear
        }
    }
}
