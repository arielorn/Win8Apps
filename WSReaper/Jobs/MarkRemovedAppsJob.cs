using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Quartz;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;
using Win8Apps.Model;

namespace WSReaper.Jobs
{
    public class MarkRemovedAppsJob : ReapJob
    {
        public List<App> AllApps { get; set; }
        public HashSet<string> AllActiveAppIds { get; set; }

        protected override void ConcreteExecute(IJobExecutionContext context)
        {
            if (AllApps == null)
                return;

            var commands = new List<PatchCommandData>();
            for (int index = 0; index < AllApps.Count(); index++)
            {
                var app = AllApps[index];
                var isActive = AllActiveAppIds.Contains(app.AppId.ToString());

                if (app.IsActive != isActive)
                {
                    var cmd = new PatchCommandData()
                        {
                            Key = app.Id,
                            Patches = new[]
                                {
                                    new PatchRequest
                                        {
                                            Name = "IsActive",
                                            Value = RavenJToken.FromObject(isActive)
                                        }
                                }
                        };
                    commands.Add(cmd);
                }
            }

            if (commands.Count > 0)
            {
                const int batchSize = 512;
                int handled = 0;
                do
                {
                    var commandsToSave = commands
                        .Skip(handled)
                        .Take(batchSize)
                        .ToList();

                    Store.DatabaseCommands.Batch(commandsToSave);
                    handled += commandsToSave.Count();

                    Console.WriteLine("Saving {0} {1}/{2} applications", commandsToSave.Count(), handled, commands.Count);
                    Thread.Sleep(5000);
                } while (handled < commands.Count);
            }
        }
    }
}