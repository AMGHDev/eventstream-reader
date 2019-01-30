using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeFeedConsole
{
    public class SampleIntegrationListener : IDisposable
    {
        private SampleListener Listener { get; }
        private bool ThrowOnErrorId { get; }
        private List<IDisposable> Subscriptions { get; } = new List<IDisposable>();

        public SampleIntegrationListener(SampleListener listener, bool throwOnErrorId)
        {
            Listener = listener;
            ThrowOnErrorId = throwOnErrorId;
            EventHandlers.ThrowOnErrorId = throwOnErrorId;
        }

        public void StartListening()
        {
            Log.Information($"Listening starting with {ThrowOnErrorId}");
            var stream = Listener.Listen();
            var sub = stream.Subscribe(onNext: EventHandlers.OnNext, onError: EventHandlers.OnError, onCompleted: EventHandlers.OnCompleted);
            Subscriptions.Add(sub);
        }

        public void Dispose()
        {
            Log.Information("SampleIntegrationListener::Dispose()");
            Subscriptions.Iter(d => d.Dispose());
            Listener.Dispose();
        }        
    }
}
