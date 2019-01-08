using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Win8Apps.Model;

namespace WSReaper.Jobs
{
    public class FeaturedAppsJob : ReapJob
    {
        private readonly HttpClient _httpClient = new HttpClient();

        private const string CountryHomePage = "https://services.apps.microsoft.com/browse/6.2.9200-1/615/en-US_en-US/c/{country}/cp/{channel}/HomePageData/pt/{platform}/lf/1";

        protected override void ConcreteExecute(IJobExecutionContext context)
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", new[] { "*/*" });
            _httpClient.Timeout = TimeSpan.FromMinutes(15);

            var countriesUrls = from country in Utils.GetCountries()
                                from channel in Enum.GetValues(typeof(Channel)).Cast<int>()
                                from platform in Enum.GetNames(typeof(Platform))
                                select new
                                    {
                                        Url = CountryHomePage.Replace("{country}", country)
                                                             .Replace("{channel}", channel.ToString())
                                                             .Replace("{platform}", platform),
                                        Country = country
                                    };

            var featuresPages = new List<Tuple<string, string>>();
            Parallel.ForEach(
                countriesUrls,
                countryUrl =>
                {
                    var country = countryUrl.Country;
                    var url = countryUrl.Url;

                    Console.WriteLine("Retrieving features pages: {0}", url);

                    using (var response = _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).Result)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            return;

                        using (var stream = response.Content.ReadAsStreamAsync().Result)
                        {
                            XDocument homepage = XDocument.Load(stream);

                            var hpr = homepage.XPathSelectElement("Hpr");
                            var pages = hpr.XPathSelectElement("pages");

                            var pagesUrl = pages.XPathSelectElements("page/href")
                                                .Select(x => "https://wscont.apps.microsoft.com/winstore/" + x.Value)
                                                .ToList();

                            lock (featuresPages)
                                featuresPages.AddRange(pagesUrl.Select(p => Tuple.Create(country, p)));
                        }
                    }
                }
                );


            var featuredAppsList = new List<string>();
            var featuredTopics = new List<Tuple<string, string, string>>();
            //foreach (var featuresPage in featuresPages.Distinct())
            Parallel.ForEach(
                featuresPages.Distinct(new TupleItem2Comparer()),
                featuresPage =>
                {

                    var country = featuresPage.Item1;
                    var url = featuresPage.Item2;
                    Console.WriteLine("Retrieving features from: {0}", url);
                    using (var response = _httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).Result)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            return;

                        var data = response.Content.ReadAsStringAsync().Result;
                        var dataJson = JsonConvert.DeserializeObject<JToken>(data);

                        var catPagesToken = dataJson.Value<JToken>("catPages");
                        var catPages = catPagesToken.Values<JToken>();
                        var features = catPages.SelectMany(x =>
                            {
                                var featuresToken = x.Value<JToken>("features");
                                if (!featuresToken.HasValues)
                                    return new List<JToken>();
                                var featuresApps = featuresToken.Values<JToken>().ToList();
                                return featuresApps;
                            });

                        var featuresList = features.ToList();

                        lock (featuredAppsList)
                        {
                            featuredAppsList.AddRange(featuresList
                                .Where(f => f.Value<string>("type") == "app")
                                .Select(f => "Apps/" + f.Value<string>("id")));
                        }
                        lock (featuredTopics)
                        {
                            featuredTopics.AddRange(featuresList
                                .Where(f => f.Value<string>("type") == "topic")
                                .Select(f =>
                                    {
                                        var id = f.Value<string>("id");
                                        return Tuple.Create(id,
                                                            country,
                                                            "https://services.apps.microsoft.com/browse/6.2.9200-1/615/en-US_en-US/c/" +
                                                            country + "/Topic/" + id);
                                    }));
                        }
                    }
                }
                );

            featuredTopics = featuredTopics.Distinct(new TopicTupleComparer()).ToList();
            //var countdown = new CountdownEvent(featuredTopics.Count());
            var topics = new List<AppsList>();
            foreach (var featuredTopic in featuredTopics)
            {
                try
                {
                    Console.WriteLine("Retrieving features from: {0}", featuredTopic);

                    var id = featuredTopic.Item1;
                    var country = featuredTopic.Item2;
                    var url = featuredTopic.Item3;
                    using (var response = _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            return;

                        using (var stream = response.Content.ReadAsStreamAsync().Result)
                        {
                            XDocument topicContent = XDocument.Load(stream);

                            var tr = topicContent.XPathSelectElement("Tr");
                            var title = tr.XPathSelectElement("T").Value;
                            var description = tr.XPathSelectElement("D") != null
                                                  ? tr.XPathSelectElement("D").Value
                                                  : "";
                            var image = tr.XPathSelectElement("Img") != null
                                            ? tr.XPathSelectElement("Img").Value
                                            : "";

                            var pts = tr.XPathSelectElement("Pts");
                            var apps = pts.XPathSelectElements("Pt")
                                          .Select(x =>
                                              {
                                                  var i = x.XPathSelectElement("I").Value;
                                                  return "Apps/" + i;
                                              }).ToList();

                            lock (topics)
                            {
                                topics.Add(new AppsList
                                    {
                                        Id = "AppsLists/" + id,
                                        Title = title,
                                        Description = description,
                                        LastDateSeen = DateTime.UtcNow.Date,
                                        Image = image,
                                        Country = country,
                                        Apps = apps
                                    });
                            }
                        }
                    }
                }
                finally
                {
                    //countdown.Signal();
                }
            }

            //countdown.Wait();

            var featuredApps = new AppsList
                {
                    IsFeatured = true,
                    Id = "AppsLists/1",
                    Title = "Features",
                    Description = "All featured apps",
                    Apps = featuredAppsList.Distinct().ToList()
                };
            var featuredAppsHistory = new AppsList
                {
                    IsFeatured = true,
                    Id = "AppsLists/1/" + DateTime.UtcNow.ToString("yyyy-M-d"),
                    Title = "Features",
                    Description = "All featured apps",
                    Apps = featuredAppsList.Distinct().ToList()
                };

            using (var session = Store.OpenSession())
            {
                session.Store(featuredApps);
                session.Store(featuredAppsHistory);
                foreach (var topic in topics)
                    session.Store(topic);
                session.SaveChanges();
            }
        }

        public class TopicTupleComparer : IEqualityComparer<Tuple<string, string, string>>
        {
            public bool Equals(Tuple<string, string, string> x, Tuple<string, string, string> y)
            {
                return String.Compare(x.Item1, y.Item1, StringComparison.OrdinalIgnoreCase) == 0;
            }

            public int GetHashCode(Tuple<string, string, string> obj)
            {
                return 0;
            }
        }

        public class TupleItem2Comparer : IEqualityComparer<Tuple<string, string>>
        {
            public bool Equals(Tuple<string, string> x, Tuple<string, string> y)
            {
                return String.Compare(x.Item2, y.Item2, StringComparison.OrdinalIgnoreCase) == 0;
            }

            public int GetHashCode(Tuple<string, string> obj)
            {
                return 0;
            }
        }
    }
}