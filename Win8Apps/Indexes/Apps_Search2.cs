using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using Win8Apps.Model;

namespace Win8Apps.Indexes
{
    public class Apps_Search2 : AbstractIndexCreationTask<App, Apps_Search2.Result>
    {
        public class Result
        {
            public string AppId { get; set; }
            public string Title { get; set; }
            public string Developer { get; set; }
            public string AgeRating { get; set; }
            //public string Description { get; set; }
            public string Category  { get; set; }
            public string Subcategory { get; set; }
            //public string ReleaseNotes { get; set; }
            public string Language { get; set; }
            public List<string> Country { get; set; }
            public string[] Query { get; set; }
            public float Price { get; set; }
            public float ScoreRating { get; set; }

        }
        public Apps_Search2()
        {
            Map = apps => from app in apps
                          let culture = CultureInfo.CreateSpecificCulture(app.Language)
                          let cultureBase = culture.Parent.Equals(CultureInfo.InvariantCulture) ? culture : culture.Parent
                          select new Result
                              {
                                  AppId = app.AppId.ToString(),
                                  Title = app.Title,
                                  Developer = app.Developer,
                                  AgeRating = app.AgeRating,
                                  Category = app.Category.Name,
                                  Subcategory = app.SubCategory != null ? app.SubCategory.Name : "",
                                  Language = cultureBase.EnglishName,// app.Language.ToLowerInvariant(),
                                  Country = app.Countries,
                                  Price = app.Price.Amount,
                                  ScoreRating = app.ScoreRating,
                                  Query = new[]
                                      {
                                          app.AppId.ToString(),
                                          app.Title,
                                          app.Developer,
                                          app.Description,
                                          app.Category.Name,
                                          app.SubCategory != null ? app.SubCategory.Name : "",
                                          app.ReleaseNotes
                                      }
                              };
            Index(x => x.Developer, FieldIndexing.NotAnalyzed);
            Index(x => x.AgeRating, FieldIndexing.NotAnalyzed);
            Index(x => x.Category, FieldIndexing.NotAnalyzed);
            Index(x => x.Subcategory, FieldIndexing.NotAnalyzed);
            Index(x => x.Language, FieldIndexing.NotAnalyzed);
            Index(x => x.Country, FieldIndexing.NotAnalyzed);
            Index(x => x.Query, FieldIndexing.Analyzed);
            //Index(x => x.Price, FieldIndexing.NotAnalyzed);
            //Index(x => x.ScoreRating, FieldIndexing.NotAnalyzed);

            Sort(x => x.Developer, SortOptions.String);
            Sort(x => x.AgeRating, SortOptions.String);
            Sort(x => x.Category, SortOptions.String);
            Sort(x => x.Subcategory, SortOptions.String);
            Sort(x => x.Language, SortOptions.String);
            Sort(x => x.Country, SortOptions.String);
            Sort(x => x.Price, SortOptions.Float);
            Sort(x => x.ScoreRating, SortOptions.Float);
        }
    }
}