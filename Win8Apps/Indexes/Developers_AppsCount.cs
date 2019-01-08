using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Win8Apps.Model;

namespace Win8Apps.Indexes
{
    public class Developers_AppsCount : AbstractIndexCreationTask<App, Developers_AppsCount.ReduceResult>
    {
        public class ReduceResult
        {
            public string Developer { get; set; }
            public int Count { get; set; }
        }

        public Developers_AppsCount()
        {
            Map = apps => from app in apps
                          select new
                              {
                                  app.Developer,
                                  Count = 1
                              };
            Reduce = results => from result in results
                                group result by result.Developer
                                into g
                                select new
                                    {
                                        Developer = g.Key,
                                        Count = g.Sum(x => x.Count),
                                    };

            Index(x => x.Developer, FieldIndexing.NotAnalyzed);
            Sort(x => x.Count, SortOptions.Long);
        }
    }
}