using System;
using System.Linq;
using Quartz;
using Win8Apps.Indexes;
using Win8Apps.Model;

namespace WSReaper.Jobs
{
    public class StatsJob : ReapJob
    {
        protected override void ConcreteExecute(IJobExecutionContext context)
        {
            using (var session = Store.OpenSession())
            {
                var appsCount = session.Query<App>()
                                       .Customize(x => x.WaitForNonStaleResults())
                                       .Where(a => a.IsActive)
                                       .Count();

                var devCount = session.Query<Developers_AppsCount.ReduceResult, Developers_AppsCount>()
                                      .Customize(x => x.WaitForNonStaleResults())
                                      .Count();

                var stats = new Stat
                    {
                        AppsCount = appsCount,
                        Developers = devCount,
                        Date = DateTime.UtcNow.Date
                    };

                session.Store(stats);
                session.SaveChanges();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Finished generating statistics...");

        }
    }
}