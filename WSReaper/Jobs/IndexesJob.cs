using System;
using Quartz;
using Win8Apps.Extensions;

namespace WSReaper.Jobs
{
    public class IndexesJob : ReapJob
    {
        protected override void ConcreteExecute(IJobExecutionContext context)
        {
            RavenDbUtils.TryCreatingIndexes(Store);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Finished generating indices...");
        }
    }
}