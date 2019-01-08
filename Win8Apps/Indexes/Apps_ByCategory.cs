using System.Linq;
using Raven.Client.Indexes;

namespace Win8Apps.Model.Indexes
{
    public class Apps_ByCategory : AbstractIndexCreationTask<App, Apps_ByCategory.ReduceResult>
    {
        public class ReduceResult
        {
            public string Category { get; set; }
            public int Count { get; set; }
        }

        public Apps_ByCategory()
        {
            Map = apps => from app in apps
                          select new
                              {
                                  Category = app.Category.Name,
                                  Count = 1
                              };
            Reduce = results => from result in results
                                group result by result.Category
                                into g
                                select new
                                    {
                                        Category = g.Key,
                                        Count = g.Sum(x => x.Count)
                                    };
        }
    }
}