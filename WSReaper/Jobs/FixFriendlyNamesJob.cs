using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Quartz;
using WSReaper.Helpers;
using Win8Apps.Model;

namespace WSReaper.Jobs
{
    public class FixFriendlyNamesJob : ReapJob
    {
        public List<App> AllApps { get; set; }

        protected override void ConcreteExecute(IJobExecutionContext context)
        {
            var usedUrls = new HashSet<string>();
            if (AllApps == null)
            {
                AllApps = Store.GetAll(s => s.Query<App>()
                                             .Customize(x => x.WaitForNonStaleResults()))
                               .ToList();
            }

            var changed = new List<App>();
            foreach (var app in AllApps)
            {
                if (string.IsNullOrEmpty(app.FriendlyUrl) || usedUrls.Contains(app.FriendlyUrl))
                {
                    var friendlyUrlBase = SlugConverter.TitleToSlug(app.Title);
                    var friendlyUrl = friendlyUrlBase;
                    int index = 2;
                    while (usedUrls.Contains(friendlyUrl))
                    {
                        friendlyUrl = string.Format("{0}{1}", friendlyUrlBase, index++);
                    }

                    app.FriendlyUrl = friendlyUrl;
                    changed.Add(app);
                }
                usedUrls.Add(app.FriendlyUrl);
            }

            using (var session = Store.OpenSession())
            {
                session.Advanced.MaxNumberOfRequestsPerSession = 10000;
                for (int index = 0; index < changed.Count; index++)
                {
                    var app = changed[index];
                    session.Store(app);

                    if (index%500 == 0)
                    {
                        Console.WriteLine("Saving {0}/{1} items", index, changed.Count);
                        session.SaveChanges();
                        Thread.Sleep(4000);
                    }
                }

                session.SaveChanges();
            }
        }
    }
}