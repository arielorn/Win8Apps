using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Win8Apps.Model;

namespace Win8Apps.Indexes
{
    public class Apps_PlatformsCount : AbstractIndexCreationTask<App, Apps_PlatformsCount.ReduceResult>
    {
        public class ReduceResult
        {
            public string Platform { get; set; }
            public int Count { get; set; }
        }

        public Apps_PlatformsCount()
        {
            Map = apps => from app in apps
                          from platform in app.SupportedPlatforms
                          select new
                          {
                              Platform = platform,
                              Count = 1
                          };
            Reduce = results => from result in results
                                group result by result.Platform
                                    into g
                                    select new
                                    {
                                        Platform = g.Key,
                                        Count = g.Sum(x => x.Count),
                                    };
            Index(x => x.Platform, FieldIndexing.NotAnalyzed);
            Sort(x => x.Count, SortOptions.Long);
        }
    }
}