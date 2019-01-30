using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeFeedConsole
{
    public static class EventHandlers
    {
        public static bool ThrowOnErrorId = false;
        public static void OnNext(IEnumerable<string> ids)
        {
            Log.Information("OnNext");
            foreach (var id in ids)
            {
                if (id.IndexOf("error", StringComparison.CurrentCultureIgnoreCase) > -1 && ThrowOnErrorId)
                    throw new Exception($"OnNext: Boom on {id}!");
                else
                    Log.Information($"OnNext: Hello doc {id}");
            }
        }
        public static void OnCompleted()
        {
            Log.Information("OnCompleted");
        }
        public static void OnError(Exception ex)
        {
            Log.Error(ex, $"OnError: I blew up with message {ex.Message}");
        }
    }
}
