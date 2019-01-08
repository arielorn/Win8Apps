using System.Linq;
using System.Web.Mvc;
using Raven.Abstractions.Data;
using Raven.Client;
using Win8Apps.Indexes;
using Win8Apps.Model;
using Win8Apps.Model.Indexes;

namespace Win8Apps.Controllers
{
    public class StatsController : RavenDbController
    {
        public const string StatsFacetsKey = "StatsFacets";
        public ActionResult Index()
        {
            return null;
        }

        public ActionResult Developers()
        {
            TempData[StatsFacetsKey] = CalcStatisticsFacet();
            return View();
        }

        public ActionResult Apps()
        {
            TempData[StatsFacetsKey] = CalcStatisticsFacet();
            return View();
        }

        public ActionResult Overview()
        {
            TempData[StatsFacetsKey] = CalcStatisticsFacet();
            return View();
        }

        private FacetResults CalcStatisticsFacet()
        {
            var query = DocumentSession
                .Query<App>("Apps/Stats");

            var facetResults = query.ToFacets("facets/StatsFacets");
            //FixRanges(facetResults);

            return facetResults;
        }

        public ActionResult TopDevelopers()
        {
            var result = DocumentSession.Query<Developers_AppsCount.ReduceResult, Developers_AppsCount>()
                                        .OrderByDescending(r => r.Count)
                                        .ThenBy(r => r.Developer)
                                        .Take(100);
            return PartialView(result);
        }


        public ActionResult AppsByCategory()
        {
            var facetResults = TempData[StatsFacetsKey] as FacetResults;
            return PartialView(facetResults.Results["Category"].Values);
        }

        public ActionResult AppsByPlatform()
        {
            var facetResults = TempData[StatsFacetsKey] as FacetResults;
            return PartialView(facetResults.Results["SupportedPlatforms"].Values);
        }

        public ActionResult AppsByLanguage()
        {
            var facetResults = TempData[StatsFacetsKey] as FacetResults;
            return PartialView(facetResults.Results["Language"].Values);

        }

        public ActionResult AppsByRatingRange()
        {
            var result = DocumentSession.Query<Apps_RatingRange.ReduceResult, Apps_RatingRange>()
                                        .OrderByDescending(r => r.Rating)
                                        .Take(100);

            return PartialView(result);
        }

        public ActionResult AppsByDate()
        {
            var stats = DocumentSession.Query<Stat>()
                .OrderBy(x => x.Date)
                .ToList();

            return PartialView(stats);
        }

        public ActionResult DevByDate()
        {
            var stats = DocumentSession.Query<Stat>()
                .OrderBy(x => x.Date)
                .ToList();

            return PartialView(stats);
        }
    }
}