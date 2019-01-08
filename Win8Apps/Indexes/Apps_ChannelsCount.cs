//using System.Linq;
//using Raven.Abstractions.Indexing;
//using Raven.Client.Indexes;
//using Win8Apps.Model;

//namespace Win8Apps.Indexes
//{
//    public class Apps_ChannelsCount : AbstractIndexCreationTask<App, Apps_ChannelsCount.ReduceResult>
//    {
//        public class ReduceResult
//        {
//            public string Channel { get; set; }
//            public int Count { get; set; }
//        }

//        public Apps_ChannelsCount()
//        {
//            Map = apps => from app in apps
//                          from channel in app.Channels
//                          select new
//                              {
//                                  Channel = channel.ToString(),
//                                  Count = 1
//                              };
//            Reduce = results => from result in results
//                                group result by result.Channel
//                                into g
//                                select new
//                                    {
//                                        Channel = g.Key,
//                                        Count = g.Sum(x => x.Count),
//                                    };
//            Index(x => x.Channel, FieldIndexing.NotAnalyzed);
//            Sort(x => x.Count, SortOptions.Long);
//        }
//    }
//}