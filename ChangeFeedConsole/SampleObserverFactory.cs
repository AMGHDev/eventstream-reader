using Microsoft.Azure.Documents.ChangeFeedProcessor.FeedProcessing;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeFeedConsole
{
    public class SampleObserverFactory : IChangeFeedObserverFactory
    {
        private IObserver<IEnumerable<string>> Stream { get; }

        public SampleObserverFactory(IObserver<IEnumerable<string>> stream)
        {
            Stream = stream;
        }
        public IChangeFeedObserver CreateObserver()
        {
            Log.Information("SampleObserverFactory: Creating observer");
            return new SampleObserver(Stream);
        }
    }
}
