using System.Globalization;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Win8Apps.Model;

namespace Win8Apps.Indexes
{
    public class Apps_LanguageCount : AbstractIndexCreationTask<App, Apps_LanguageCount.ReduceResult>
    {
        public class ReduceResult
        {
            public string Language { get; set; }
            public int Count { get; set; }
        }

        public Apps_LanguageCount()
        {
            Map = apps => from app in apps
                          let language = new CultureInfo(app.Language)
                          let languageBase = language.Parent.Equals(CultureInfo.InvariantCulture) ? language : language.Parent
                          select new
                              {
                                  Language = languageBase.EnglishName,
                                  Count = 1
                              };
            Reduce = results => from result in results
                                group result by result.Language
                                into g
                                select new
                                    {
                                        Language = g.Key,
                                        Count = g.Sum(x => x.Count),
                                    };
            Index(x => x.Language, FieldIndexing.NotAnalyzed);
            Sort(x => x.Count, SortOptions.Long);
        }
    }
}