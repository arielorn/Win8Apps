using System.Collections.Generic;
using System.Linq;
using Quartz;
using WSReaper.Helpers;
using Win8Apps.Model;

namespace WSReaper.Jobs
{
    public class LoadAllAppsJob : ReapJob
    {
        public List<App> AllApps { get; private set; }
        protected override void ConcreteExecute(IJobExecutionContext context)
        {
            AllApps = Store
                .GetAll(s => s.Query<App>()
                              .Customize(x => x.WaitForNonStaleResults()))
                .ToList();
        }
    }
}