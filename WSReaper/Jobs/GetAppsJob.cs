using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Quartz;
using Raven.Abstractions.Commands;
using Raven.Client.Document;
using Raven.Json.Linq;
using Win8Apps.Model;

namespace WSReaper.Jobs
{
    public class GetAppsJob : ReapJob
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public HashSet<string> AppIds { get; private set; }

        public bool LoadIdsOnly { get; set; }

        protected override void ConcreteExecute(IJobExecutionContext context)
        {
            _httpClient.DefaultRequestHeaders.Add("Accept", new[] { "application/xml", "text/xml" });
            _httpClient.Timeout = TimeSpan.FromMinutes(15);
            
            JobDataMap data = context.JobDetail.JobDataMap;
			string operationStr = data.GetString("operation");

            Operation operation;
            if (!Enum.TryParse(operationStr, true, out operation))
                operation = Operation.Full;


            GetApps2(operation: operation);
        }

        public void Execute(int pages = -1, int apps = -1, Operation operation = Operation.Full)
        {
            using (Store = new DocumentStore
                {
                    ConnectionStringName = "RavenHQ"
                }.Initialize())
            {
                _httpClient.DefaultRequestHeaders.Add("Accept", new[] {"application/xml", "text/xml"});
                _httpClient.Timeout = TimeSpan.FromMinutes(15);

                GetApps2(pages, apps, operation);
            }
        }



        private void GetApps2(int pages = -1, int apps = -1, Operation operation = Operation.Full)
        {
            Store.Conventions.MaxNumberOfRequestsPerSession = 820;

            const string sitemapIndex = "http://apps.microsoft.com/windows/sitemap_index.xml";
            var prefixLength = "http://apps.microsoft.com/windows/".Length;

            XNamespace xmlns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");

            XmlNameTable table = new NameTable();
            var manager = new XmlNamespaceManager(table);
            manager.AddNamespace("", xmlns.NamespaceName);

            IEnumerable<string> sitemapPages;
            using (var response = _httpClient.GetAsync(sitemapIndex, HttpCompletionOption.ResponseContentRead).Result)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    return;

                using (var stream = response.Content.ReadAsStreamAsync().Result)
                {
                    XDocument sitemap = XDocument.Load(stream);

                    var sitemapIndexElem = sitemap.Element(xmlns.GetName("sitemapindex"));
                    var sitemaps = sitemapIndexElem.Elements(xmlns.GetName("sitemap"));

                    sitemapPages = sitemaps
                        .Select(x => x.Element(xmlns.GetName("loc"))).Select(x => x.Value);
                }
            }

            var globalAppsToCultures = new ConcurrentDictionary<string, AppInfo2>();

            if (pages > 0)
                sitemapPages = sitemapPages.Take(pages);

            Parallel.ForEach(
                sitemapPages,
                new ParallelOptions {MaxDegreeOfParallelism = 2},
                sitemapPage =>
                    {
                        Console.WriteLine("Requesting: {0}", sitemapPage);
                        using (var response = _httpClient.GetAsync(sitemapPage, HttpCompletionOption.ResponseContentRead).Result)
                        {
                            if (response.StatusCode != HttpStatusCode.OK)
                                return;

                            Console.WriteLine("Received: {0}", sitemapPage);

                            using (var stream = response.Content.ReadAsStreamAsync().Result)
                            {
                                Console.WriteLine("Loading: {0}", sitemapPage);

                                XDocument sitemapPageContent = XDocument.Load(stream);

                                var urlsetElem = sitemapPageContent.Element(xmlns.GetName("urlset"));
                                var urlsElems = urlsetElem.Elements(xmlns.GetName("url"));
                                var appsLinks = urlsElems.Select(x => x.Element(xmlns.GetName("loc")))
                                                         .Select(x => x.Value);


                                foreach (var appLink in appsLinks)
                                {
                                    var appId = appLink.Substring(appLink.LastIndexOf('/') + 1);
                                    var culture = appLink.Substring(prefixLength,
                                                                    appLink.IndexOf('/', prefixLength) -
                                                                    prefixLength);

                                    if (appId == "ROW")
                                        continue;

                                    globalAppsToCultures.AddOrUpdate(
                                        appId,
                                        new AppInfo2
                                            {
                                                Id = appId,
                                                Url = appLink,
                                                Countries = new List<string>(new[] {culture})
                                            },
                                        (c, ai) =>
                                            {
                                                ai.Countries.Add(culture);
                                                return ai;
                                            }
                                        );
                                }
                                Console.WriteLine("Total Found {0} applications", globalAppsToCultures.Count);
                            }
                        }
                    });

            AppIds = new HashSet<string>(globalAppsToCultures.Keys);

            if (LoadIdsOnly)
                return;

            var toProcess = globalAppsToCultures.AsEnumerable();
            if (apps > 0)
                toProcess = toProcess.Take(apps);

            var toProcessList = toProcess.ToList();

            var countdown = new CountdownEvent(toProcessList.Count);

            var commands = new List<ICommandData>();

            Parallel.ForEach(
                toProcessList,
                new ParallelOptions {MaxDegreeOfParallelism = 8},
                (kvp, pls, index) =>
                    {
                        using (var session = Store.OpenSession())
                        {
                            try
                            {
                                Console.WriteLine("({0}/{1} - {2} remaining) Loading app {3}",
                                                  index, globalAppsToCultures.Count, countdown.CurrentCount,
                                                  kvp.Value.Id);
                                
                                var existing = session.Load<App>("apps/" + kvp.Value.Id);
                                try
                                {
                                    var app = GetAppDetails2(kvp.Value.Id, kvp.Value.Countries);
                                    if (app == null)
                                        return;

                                    if (existing != null)
                                    {
                                        if ((app.Price != null && existing.Price != null &&
                                            app.Price.Amount == existing.Price.Amount) ||
                                            app.LastUpdated == existing.LastUpdated)
                                        {
                                            return;
                                        }
                                        app.FriendlyUrl = existing.FriendlyUrl;
                                        app.PreviousPrice = existing.Price;
                                    }

                                    var putCommandData = new PutCommandData
                                        {
                                            Key = app.Id,
                                            Document = RavenJObject.FromObject(app),
                                            Metadata = new RavenJObject
                                                {
                                                    {"Raven-Entity-Name", new RavenJValue("Apps")},
                                                    {
                                                        "Raven-Clr-Type",
                                                        new RavenJValue("Win8Apps.Model.App, Win8Apps.Model")
                                                    }
                                                }
                                        };

                                    lock (commands)
                                    {
                                        commands.Add(putCommandData);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            finally
                            {
                                countdown.Signal();
                            }
                        }
                    });

            countdown.Wait();

            if (commands.Count > 0)
            {
                const int batchSize = 512;
                int handled = 0;
                do
                {
                    var commandsToSave = commands
                        .Skip(handled)
                        .Take(batchSize)
                        .ToList();

                    Store.DatabaseCommands.Batch(commandsToSave);
                    handled += commandsToSave.Count();

                    Console.WriteLine("Saving {0} {1}/{2} applications", commandsToSave.Count(), handled, commands.Count);
                    Thread.Sleep(5000);
                } while (handled < commands.Count);
            }
        }

        private App GetAppDetails2(string id, IEnumerable<string> countries)
        {
            try
            {
                const string url = "https://services.apps.microsoft.com/browse/6.2.9200-1/615/en-US_en-US/c/US/cp/10082778/Apps/";

                string finalUrl = url + id;
                using (var response = _httpClient.GetAsync(finalUrl).Result)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        return null;

                    using (var stream = response.Content.ReadAsStreamAsync().Result)
                    {
                        var xDocument = XDocument.Load(stream);
                        var app = CreateApp(xDocument);

                        app.Countries.AddRange(countries);

                        //GetAppReviews(app);

                        return app;
                    }
                }
            }
            catch (WebException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: {0}", ex.Message);
                return null;
            }
        }

        private App CreateApp(XDocument xDocument)
        {
            var emr = xDocument.XPathSelectElement("Emr");
            var pt = emr.XPathSelectElement("Pt");

            var id = (Guid) pt.XPathSelectElement("I");
            var r = (Guid) pt.XPathSelectElement("R");
            var b = pt.XPathSelectElement("B") != null ? (Guid) pt.XPathSelectElement("B") : Guid.Empty;
            var pfn = (string) pt.XPathSelectElement("Pfn");
            var language = (string) pt.XPathSelectElement("L");
            var title = (string) pt.XPathSelectElement("T");
            var ageRating = (string) pt.XPathSelectElement("Wr");
            var ico = (string) pt.XPathSelectElement("Ico");
            var background = (string) pt.XPathSelectElement("Bg");
            var foreground = (string) pt.XPathSelectElement("Fg");
            var scoreRating = pt.XPathSelectElement("Sr") != null ? (float) pt.XPathSelectElement("Sr") : 0f;
            var currency = (string) pt.XPathSelectElement("Cc");
            var price = pt.XPathSelectElement("P") != null ? (float) pt.XPathSelectElement("P") : 0f;

            var category = new Category
                {
                    Id = (CategoryIds) (int) pt.XPathSelectElement("C/I"),
                    Name = (string) pt.XPathSelectElement("C/N")
                };

            var subcategory = pt.XPathSelectElement("Sc") != null
                                  ? new Category
                                      {
                                          Id = (CategoryIds) (int) pt.XPathSelectElement("Sc/I"),
                                          Name = (string) pt.XPathSelectElement("Sc/N")
                                      }
                                  : null;


            var type = (int) pt.XPathSelectElement("Typ");
            var accessibility = (bool) pt.XPathSelectElement("Acc");
            var dca = (bool) pt.XPathSelectElement("Dca");
            var trial = (bool) pt.XPathSelectElement("Try");
            var lastUpdated = (DateTime) pt.XPathSelectElement("Lud");


            var s = (int) emr.XPathSelectElement("S");
            var description = (string) emr.XPathSelectElement("D");

            var features = emr.XPathSelectElements("Dbps/Dbp").Select(e => e.Value).ToList();

            var releaseNotes = (string) emr.XPathSelectElement("Ud");
            var ratingCount = emr.XPathSelectElement("Src") != null ? (int) emr.XPathSelectElement("Src") : 0;
            var oc = (string) emr.XPathSelectElement("Oc");

            var supportedLanguages = emr.XPathSelectElements("Sls/Sl").Select(e => e.Value).ToList();

            var developer = (string) emr.XPathSelectElement("Dev");
            var version = emr.XPathSelectElement("V") != null ? (long) emr.XPathSelectElement("V") : 0l;
            var copyrights = (string) emr.XPathSelectElement("Cr");
            var website = (string) emr.XPathSelectElement("Ws");
            var supportWebsite = (string) emr.XPathSelectElement("Sws");
            var privacyPolicy = (string) emr.XPathSelectElement("Pu");
            var eula = (string) emr.XPathSelectElement("Eula");
            var downloadSize = emr.XPathSelectElement("Ds") != null ? (long) emr.XPathSelectElement("Ds") : 0l;

            var supportedPlatforms = emr.XPathSelectElements("Sas2/Sa2/An").Select(e => e.Value).ToList();

            var permissions = new List<AppCapabilities>();
            var permissionsString = emr.XPathSelectElements("Cs/C").Select(e => e.Value).ToList();
            foreach (var permission in permissionsString)
            {
                AppCapabilities parsedPermission;
                if (Enum.TryParse(permission, out parsedPermission))
                {
                    permissions.Add(parsedPermission);
                }
            }
            var recommendedHardware = emr.XPathSelectElements("Hbps/Hbp").Select(e => e.Value).ToList();

            var screenshots = emr.XPathSelectElements("Sss/Ss")
                                 .Select(e => new Screenshot
                                     {
                                         Url = e.Element("U").Value,
                                         Caption = e.Element("Cap").Value
                                     }).ToList();

            var index = language.IndexOf('-');
            if (index != -1)
            {
                language = language.Substring(0, index);
            }

            return new App
                {
                    Id = "apps/" + id,
                    AppId = id,
                    R = r,
                    B = b,
                    PackageFamilyName = pfn,
                    Language = language,
                    Title = title,
                    AgeRating = ageRating,
                    Icon = ico,
                    Background = background,
                    Foreground = foreground,
                    ScoreRating = scoreRating,
                    Price = new Price
                        {
                            Currency = currency,
                            Amount = price
                        },
                    Category = category,
                    SubCategory = subcategory,
                    Type = type,
                    Accessibility = accessibility,
                    Dca = dca,
                    Trial = trial,
                    LastUpdated = lastUpdated,
                    S = s,
                    Description = description,
                    Features = features,
                    ReleaseNotes = releaseNotes,
                    RatingCount = ratingCount,
                    Oc = oc,
                    SupportedLanguages = supportedLanguages,
                    Developer = developer,
                    Version = version,
                    Copyrights = copyrights,
                    Website = website,
                    SupportWebsite = supportWebsite,
                    PrivacyPolicy = privacyPolicy,
                    Eula = eula,
                    DownloadSize = downloadSize,
                    SupportedPlatforms = supportedPlatforms,
                    Permissions = permissions,
                    RecommendedHardware = recommendedHardware,
                    Screenshots = screenshots,
                    Countries = new List<string>(1),
                    Channels = new List<Channel>(1),
                    IsActive = true
                };
        }


        [Flags]
        public enum Operation
        {
            Full = 1,
            New = 2,
            Update = 4,
            Remove = 8
        }

    }
}