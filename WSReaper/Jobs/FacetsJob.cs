using System;
using Quartz;
using Win8Apps.Extensions;

namespace WSReaper.Jobs
{
    public class FacetsJob : ReapJob
    {
        protected override void ConcreteExecute(IJobExecutionContext context)
        {
            RavenDbUtils.TryCreatingFacets(Store);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Finished generating facets...");
        }
    }
}