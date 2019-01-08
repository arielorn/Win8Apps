//using System.Linq;
//using Raven.Abstractions.Indexing;
//using Raven.Client.Indexes;
//using Win8Apps.Model;

//namespace Win8Apps.Indexes
//{
//    public class Apps_Search : AbstractIndexCreationTask<App, Apps_Search.Result>
//    {
//        public class Result
//        {
//            public string[] Query { get; set; }
//        }

//        public Apps_Search()
//        {
//            Map = apps => from app in apps
//                          select new
//                              {
//                                  Query = new[]
//                                      {
//                                          app.AppId.ToString(),
//                                          app.Title,
//                                          app.Description,
//                                          app.Category.Name,
//                                          app.ReleaseNotes
//                                      }
                              
//                              };
//            Index(x => x.Query, FieldIndexing.Analyzed);
//        }
//    }
//}