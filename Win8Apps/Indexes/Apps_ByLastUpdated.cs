using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Win8Apps.Model;

namespace Win8Apps.Indexes
{
    public class Apps_ByLastUpdated : AbstractIndexCreationTask<App>
    {

        public Apps_ByLastUpdated()
        {
            Map = apps => from app in apps
                          select new
                              {
                                  app.LastUpdated
                              };

            Sort(x => x.LastUpdated, SortOptions.String);
        }
    }
}