using System;
using System.Linq;
using Quartz;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using Win8Apps.Model;

namespace WSReaper.Jobs
{
    public class TestFacetsCountJob : ReapJob
    {
        protected override void ConcreteExecute(IJobExecutionContext context)
        {
            //var priceTerms = Store.DatabaseCommands.GetTerms("Apps/Stats", "Price", "", 100).ToList();
            //var categoryTerms = Store.DatabaseCommands.GetTerms("Apps/Stats", "Category", "", 100).ToList();

            //var facets = Store.DatabaseCommands.GetFacets(
            //    "Apps/Stats",
            //    new IndexQuery
            //        {
            //        },
            //    "facets/StatsFacets", 2, 10000);

            using (var session = Store.OpenSession())
            {
                session.Advanced.AllowNonAuthoritativeInformation = true;
                session.Advanced.MaxNumberOfRequestsPerSession = 10000;
                RavenQueryStatistics stats;
                var query = session.Query<App>("Apps/Stats")
                    .Statistics(out stats)
                    ;

              
                //var count = query.Count();
                var results = query.ToList();

                var facetResults = query.ToFacets("facets/StatsFacets");

                var category = facetResults.Results["Category"];
                var price = facetResults.Results["Price_Range"];
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Finished generating statistics...");
        }
    }
}