using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using Win8Apps.Extensions;
using Win8Apps.Indexes;
using Win8Apps.Model;
using Win8Apps.Model.Boost;
using Win8Apps.Models;

namespace Win8Apps.Controllers.Api
{
    public class AppsController : RavenDbApiController
    {
        [HttpGet]
        public IQueryable<App> Recent(int count = 21)
        {
            var apps = Queryable.OrderByDescending(DocumentSession.Query<App, Apps_ByLastUpdated>(), a => a.LastUpdated)
                                      .Take(count);

            return apps;
        }

        [HttpGet]
        public IQueryable<App> GoneFree(int count = 21)
        {
            var apps = Queryable.OrderByDescending(DocumentSession.Query<App>("Apps/GoneFree"), a => a.LastUpdated)
                .Take(count);

            return apps;
        }

        [HttpGet]
        public IQueryable<App> TopGames(int count = 21)
        {
            var apps = Queryable.OrderByDescending(DocumentSession.Query<App>()
                                                                        .Where(a => a.Category.Id == CategoryIds.Games), a => a.ScoreRating)
                                      .Take(count);

            return apps;
        }

        [HttpGet]
        public IQueryable<App> TopFreeGames(int count = 21)
        {
            const float @decimal = 0.0f;
            var games = Queryable.Where(DocumentSession.Query<App>("Apps/ByCategoryAndPrice"), a => a.Category.Id == CategoryIds.Games);
            var freeGames = games.Where(a => a.Price.Amount == @decimal);
            var sortedFreeGames = freeGames.OrderByDescending(a => a.ScoreRating);
            var apps = sortedFreeGames.Take(count);

            return apps;
        }

        [HttpGet]
        public IQueryable<App> TopApps(int count = 21)
        {
            var apps = Queryable.OrderByDescending(DocumentSession.Query<App>()
                                                                        .Where(a => a.Category.Id != CategoryIds.Games), a => a.ScoreRating)
                                      .Take(count);

            return apps;
        }

        [HttpGet]
        public IQueryable<App> TopFreeApps(int count = 21)
        {
            const float @decimal = 0.0f;
            var apps = Queryable.OrderByDescending(DocumentSession.Query<App>()
                                                                        .Where(a => a.Category.Id != CategoryIds.Games)
                                                                        .Where(a => a.Price.Amount <= @decimal), a => a.ScoreRating)
                                      .Take(count);

            return apps;
        }

        [HttpGet]
        public IQueryable<AppsList> Topics(int count = 10)
        {
            var topics = Queryable.OrderByDescending(DocumentSession.Query<AppsList>()
                                                                 .Where(t => !t.IsFeatured), t => t.LastDateSeen);

            return topics;
        }

        [HttpGet]
        public AppsList Topic(string topicId)
        {
            var topic = DocumentSession
                .Include<AppsList>(t => t.Apps)
                .Load<AppsList>(topicId);

            return topic;
        }

        [HttpGet]
        public IEnumerable<CategoryDescription> Categories()
        {
            var categories = Enum.GetValues(typeof(CategoryIds))
                                 .Cast<CategoryIds>()
                                 .Select(c => new CategoryDescription { Id = c, Title = c.GetDescription() })
                                 .Where(c => c.Title != null);

            return categories;
        }

        [HttpGet]
        public App App(string id)
        {
            Guid appId;
            App app;
            if (Guid.TryParse(id, out appId))
                app = DocumentSession.Load<App>("apps/" + id);
            else
            {
                app = DocumentSession.Query<App>()
                    .FirstOrDefault(a => a.FriendlyUrl == id);
            }

            return app;
        }

        [HttpGet]
        public SearchViewModel Search(string query, int page = 1, string ageRating = null, string developer = null,
                                   string category = null, string subCategory = null, string country = null,
                                   string language = null, string sortBy = null, SortOrder sortOrder = SortOrder.Desc)
        {
            if (string.IsNullOrEmpty(query))
                query = string.Empty;

            RavenQueryStatistics statistics;
            var q = DocumentSession
                .Query<Apps_Search2.Result, Apps_Search2>()
                .Statistics(out statistics);

            if (!string.IsNullOrEmpty(query))
                q = q.Search(x => x.Query, query);

            q = BoostMethod(q);


            var filters = new List<FilterViewModel>();

            if (!string.IsNullOrEmpty(ageRating))
            {
                q = q.Where(x => x.AgeRating == ageRating);
                filters.Add(new FilterViewModel
                {
                    DisplayText = string.Format("Age Rating: {0}", ageRating),
                    Query = "ageRating"
                });
            }
            if (!string.IsNullOrEmpty(developer))
            {
                q = q.Where(x => x.Developer == developer);
                filters.Add(new FilterViewModel
                {
                    DisplayText = string.Format("Developer: {0}", developer),
                    Query = "developer"
                });
            }
            if (!string.IsNullOrEmpty(category))
            {
                q = q.Where(x => x.Category == category);
                filters.Add(new FilterViewModel
                {
                    DisplayText = string.Format("Category: {0}", category),
                    Query = "category"
                });

            }
            if (!string.IsNullOrEmpty(subCategory))
            {
                q = q.Where(x => x.Subcategory == subCategory);
                filters.Add(new FilterViewModel
                {
                    DisplayText = string.Format("Sub Category: {0}", subCategory),
                    Query = "subCategory"
                });
            }
            if (!string.IsNullOrEmpty(country))
            {
                q = q.Where(x => x.Country.Any(c => c == country));
                filters.Add(new FilterViewModel
                {
                    DisplayText = string.Format("Country: {0}", country),
                    Query = "country"
                });
            }
            if (!string.IsNullOrEmpty(language))
            {
                q = q.Where(x => x.Language == language);
                filters.Add(new FilterViewModel
                {
                    DisplayText = string.Format("Language: {0}", language),
                    Query = "language"
                });
            }

            SortBy sortByOrder;
            if (Enum.TryParse(sortBy, true, out sortByOrder))
            {
                Expression<Func<Apps_Search2.Result, object>> keySelector = null;
                if (sortByOrder == SortBy.Category)
                    keySelector = r => r.Category;
                else if (sortByOrder == SortBy.Language)
                    keySelector = r => r.Language;
                else if (sortByOrder == SortBy.Price)
                    keySelector = r => r.Price;
                else if (sortByOrder == SortBy.ScoreRating)
                    keySelector = r => r.ScoreRating;
                else if (sortByOrder == SortBy.Title)
                    keySelector = r => r.Title;

                if (keySelector != null)
                    q = sortOrder == SortOrder.Desc ? q.OrderByDescending(keySelector) : q.OrderBy(keySelector);
            }

            var queryable = q.As<App>();
            var results = queryable.Paging(page, SearchViewModel.DefaultPage, SearchViewModel.PageSize).ToList();

            if (results.Count == 0)
                return null;

            var facets = ProcessFacets(queryable, category, country);

            var viewModel = new SearchViewModel
            {
                QueryString = query,
                Apps = results,
                CurrentPage = page,
                TotalCount = statistics.TotalResults,
                Facets = facets
            };
            return viewModel;
        }

        private IRavenQueryable<Apps_Search2.Result> BoostMethod(IRavenQueryable<Apps_Search2.Result> q)
        {
            var boosted = DocumentSession.Load<BoostedResults>("options/boostedResults");
            if (boosted != null)
            {
                // Boost by developer
                if (boosted.Developers.Any())
                {
                    var boostDevelopersQuery = "* " +
                                               string.Join(" ",
                                                           boosted.Developers.Select(
                                                               x => "\"" + x.Criteria + "\"^" + x.Value));
                    q = q.Search(x => x.Developer, boostDevelopersQuery, 1, SearchOptions.And,
                                 EscapeQueryOptions.RawQuery);
                }

                // Boost by language
                if (boosted.Languages.Any())
                {
                    var boostedLanguagesQuery = "* " +
                                                string.Join(" ",
                                                            boosted.Languages.Select(
                                                                x => "\"" + x.Criteria + "\"^" + x.Value));
                    q = q.Search(x => x.Language, boostedLanguagesQuery, 1, SearchOptions.And,
                                 EscapeQueryOptions.RawQuery);
                }


                // Boost by specific apps
                if (boosted.Apps.Any())
                {
                    var boostedAppsQuery = "* " +
                                           string.Join(" ",
                                                       boosted.Apps.Select(x => "\"" + x.Criteria + "\"^" + x.Value));
                    q = q.Search(x => x.AppId, boostedAppsQuery, 1, SearchOptions.And, EscapeQueryOptions.RawQuery);
                }

            }
            return q;
        }

        private IEnumerable<Model.Facet> ProcessFacets(IQueryable<App> query, string category, string country)
        {
            var facetResults = query.ToFacets("facets/AppsFacets");

            var facets = new List<Model.Facet>();

            foreach (var facetResult in facetResults.Results)
            {
                if (facetResult.Value.Values.Count <= 1)
                    continue;
                if (string.IsNullOrEmpty(category))
                {
                    if (facetResult.Key == "Subcategory")
                        continue;
                }
                if (!string.IsNullOrEmpty(country))
                {
                    if (facetResult.Key == "Country")
                        continue;
                }

                if (facetResult.Value.Values.All(fv => fv.Hits == 1))
                    continue;

                var title = facetResult.Key;
                var facet = new Model.Facet { Title = title };
                foreach (var facetValue in facetResult.Value.Values.OrderBy(x => x.Range))
                {
                    var value = new FacetValue { Range = facetValue.Range, Hits = facetValue.Hits };
                    if (facetValue.Range == "EMPTY_STRING")
                    {
                        facet.Values.Insert(0, value);
                    }
                    else
                        facet.Values.Add(value);
                }


                facets.Add(facet);
            }

            return facets;
        }
    }



}