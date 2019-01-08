using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Raven.Client;
using Raven.Client.Linq;

namespace WSReaper.Helpers
{
    public static class DocumentSessionExtensions
    {
        public static void PerformOnAll<T>(this IDocumentStore store,
                                           Func<IDocumentSession, IRavenQueryable<T>> buildQuery, Func<T, bool> action, bool saveChanges = true)
        {

            const int pageSize = 1024;
            int totalChanges;

            do
            {

                totalChanges = 0;
                var handledCount = 0;
                int totalCount;
                do
                {
                    using (var session = store.OpenSession())
                    {
                        var query = buildQuery(session);

                        RavenQueryStatistics statistics;
                        var results = query
                            .Statistics(out statistics)
                            .Skip(handledCount)
                            .Take(pageSize)
                            .ToList();

                        int changes = 0;
                        results = results.Select(
                            x =>
                                {
                                    if (action(x))
                                        changes++;
                                    return x;
                                }).ToList();


                        handledCount += results.Count();

                        totalCount = statistics.TotalResults;
                        totalChanges += changes;

                        if (saveChanges && session.Advanced.HasChanges && changes > 0)
                        {
                            Console.WriteLine("Updating {0} friendly name with {1}/{2}", changes, handledCount,
                                              totalCount);
                            session.SaveChanges();
                            Thread.Sleep(changes*10);
                        }
                        else
                        {
                            Console.WriteLine("Updating friendly name with {0}/{1}", handledCount, totalCount);
                        }
                    }

                } while (handledCount < totalCount);
            } while (totalChanges > 0);
        }

        public static IEnumerable<T> GetAll<T>(this IDocumentStore store,
                                           Func<IDocumentSession, IRavenQueryable<T>> buildQuery)
        {

            const int pageSize = 1024;
            var results = new List<T>();
            using (var session = store.OpenSession())
            {
                session.Advanced.MaxNumberOfRequestsPerSession = 10000;
                int totalCount;
                do
                {

                    var query = buildQuery(session);

                    RavenQueryStatistics statistics;
                    var qResults = query
                        .Statistics(out statistics)
                        .Skip(results.Count)
                        .Take(pageSize)
                        .ToList();

                    results.AddRange(qResults);

                    totalCount = statistics.TotalResults;

                    Console.WriteLine("Loaded {0}/{1} items", results.Count, totalCount);

                } while (results.Count < totalCount);
            }
            return results;
        }
    }
}