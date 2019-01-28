using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using Topshelf;

namespace ChangeFeedConsole
{
    public class Program
    {
        static bool ThrowOnErrorId = true; // set this flag=true if you want the stream to blow on an id with "error" in it
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Log.Logger = new LoggerConfiguration()
                                    //.WriteTo.Seq("http://localhost:5341/")
                                    .WriteTo.Console()
                                    .CreateLogger();

            Log.Information("***Program::Main STARTING***");
            HostFactory.Run(config =>
            {
                config.Service<FeedService>(s =>
                {
                    s.ConstructUsing(() => new FeedService(ThrowOnErrorId));

                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                config.SetServiceName("FeedService");
                config.SetDisplayName("FeedService");
                config.SetDescription("FeedService");
            });
        }
    }
}
