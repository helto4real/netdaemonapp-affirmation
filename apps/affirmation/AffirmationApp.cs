using System;
using System.Threading.Tasks;
using JoySoftware.HomeAssistant.NetDaemon.Common;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

// Use unique namespaces for your apps if you going to share with others to avoid
// conflicting names
namespace helto4real
{
    public class AffirmationApp : NetDaemonApp
    {
        public double Hours { get; set; }
        public override Task InitializeAsync()
        {
            Scheduler.RunEvery(TimeSpan.FromHours(Hours), UpdateSensorData);
            return Task.CompletedTask;
        }

        private async Task UpdateSensorData()
        {
            try
            {
                //{"affirmation":"I know you'll sort it out"}
                var todaysDictionary = await Http.GetJson<Dictionary<string, string>>("https://www.affirmations.dev/").ConfigureAwait(false);
                if (todaysDictionary.TryGetValue("affirmation", out var affirmation))
                {
                    await SetState("sensor.affirmation", "on", ("text", affirmation), ("attribution", "www.affirmations.dev"));
                }
            }
            catch (Exception e)
            {
                Log(LogLevel.Trace, e, "Error getting affirmation");
                await SetState("sensor.affirmation", "unavailable", ("text", "unavailable"!));
            }
        }
    }
}

