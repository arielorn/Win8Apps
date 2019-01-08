using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Raven.Json.Linq;

namespace Win8Apps.Model.Indexes
{
    public class Apps_RatingRange : AbstractIndexCreationTask<App, Apps_RatingRange.ReduceResult>
    {
        public class ReduceResult
        {
            public float Rating { get; set; }
            public int Count { get; set; }
        }
        
        public Apps_RatingRange()
        {

            Map = apps => from app in apps
                          where app.ScoreRating != null
                          where app.ScoreRating.ToString() != "0.0"
                          let score = double.Parse(app.ScoreRating.ToString())
                          let doubleScore = score * 2
                          let intScore = (int)doubleScore
                          let finalScore = intScore / 2.0f
                          select new
                          {
                              Rating = finalScore,
                              Count = 1
                          }
;

            Reduce = results => from result in results
                                group result by result.Rating
                                into g
                                select new
                                    {
                                        Rating = g.Key,
                                        Count = g.Sum(x => x.Count)
                                    };
            
            Sort(x => x.Rating, SortOptions.Float);
        }
    }
}