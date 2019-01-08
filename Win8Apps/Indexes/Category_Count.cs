//using System;
//using System.Linq;
//using Raven.Abstractions.Indexing;
//using Raven.Client.Indexes;
//using Win8Apps.Model;

//namespace Win8Apps.Indexes
//{
//    public class Category_Count : AbstractIndexCreationTask<App, Category_Count.ReduceResult>
//    {
//        public class ReduceResult
//        {
//            public string Category { get; set; }
//            public int Count { get; set; }
//            public DateTimeOffset LastSeenAt { get; set; }
//        }

//        public Category_Count()
//        {
//            Map = apps => from app in apps
//                          select new
//                              {
//                                  Category = app.Category.Name.ToString().ToLower(),
//                                  Count = 1,
//                                  LastSeenAt = app.LastUpdated
//                              };
//            Reduce = results => from categoryCount in results
//                                group categoryCount by categoryCount.Category
//                                into g
//                                select new
//                                    {
//                                        Category = g.Key,
//                                        Count = g.Sum(x => x.Count),
//                                        LastSeenAt = g.Max(x => x.LastSeenAt)
//                                    };

//            Index(x => x.Category, FieldIndexing.NotAnalyzed);
//        }
//    }
//}