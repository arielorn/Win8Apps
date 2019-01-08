namespace WSReaper
{
    public class OldCode
    {
        //private static void Main()
        //{
        //    HttpClient.DefaultRequestHeaders.Add("Accept", new[] { "application/xml", "text/xml" });
        //    HttpClient.Timeout = TimeSpan.FromMinutes(15);
        //    using (_store = new DocumentStore
        //        {
        //            ConnectionStringName = "RavenHQ"
        //        }.Initialize())
        //    {

        //        while (true)
        //        {
        //            Console.ForegroundColor = ConsoleColor.Gray;
        //            var cmd = Console.ReadLine();

        //            if (cmd == "cls")
        //                Console.Clear();
        //            else if (cmd == "q")
        //                break;
        //            else if (cmd == "stats")
        //            {
        //                new StatsJob().Execute(null);


        //            }
        //            else if (cmd == "index")
        //            {
        //                new IndexesJob().Execute(null);
        //            }
        //            else if (cmd == "facet")
        //            {
        //                new FacetsJob().Execute(null);
        //            }
        //            else if (cmd.StartsWith("full") ||
        //                cmd.StartsWith("new") ||
        //                cmd.StartsWith("update"))
        //            {
        //                int apps;
        //                int pages;
        //                ParseArgs(cmd, out pages, out apps);

        //                Console.ForegroundColor = ConsoleColor.Green;
        //                Console.WriteLine("Starting scrapping store: Pages {0}, Apps {1}",
        //                    pages == -1 ? "All" : pages.ToString(),
        //                    apps == -1 ? "All" : apps.ToString());


        //                new GetAppsJob().Execute(null);

        //                Console.ForegroundColor = ConsoleColor.Gray;
        //                if (cmd.StartsWith("full"))
        //                {
        //                    GetApps2(pages: pages, apps: apps);
        //                }
        //                else if (cmd.StartsWith("new"))
        //                {
        //                    GetApps2(pages: pages, apps: apps, newOnly: true);
        //                }
        //                else if (cmd.StartsWith("update"))
        //                {
        //                    GetApps2(pages: pages, apps: apps, updateOnly: true);
        //                }

        //                Console.ForegroundColor = ConsoleColor.Red;
        //                Console.WriteLine("Finished scrapping Store...");
        //            }
        //        }
        //    }
        //}

        //private static void ParseArgs(string cmd, out int pages, out int apps)
        //{
        //    var args = cmd.Split(' ');
        //    pages = -1;
        //    apps = -1;
        //    if (args.Length == 2)
        //    {
        //        if (!int.TryParse(args[1], out pages))
        //            pages = -1;
        //    }
        //    else if (args.Length == 3)
        //    {
        //        if (!int.TryParse(args[1], out pages))
        //            pages = -1;
        //        if (!int.TryParse(args[2], out apps))
        //            apps = -1;
        //    }
        //}


        //private static void Delay(int minutes)
        //{
        //    int delay = minutes * 2;
        //    while (delay > 0)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Green;
        //        Console.WriteLine("Starting in {0:0.#} minutes...", ((float) delay)/2);
        //        Thread.Sleep(30*1000);
        //        delay--;
        //    }
        //}

        //private static void SetLastScan()
        //{
        //    using (var session = _store.OpenSession())
        //    {
        //        var lastStoreScan = new LastStoreScan {Date = DateTime.UtcNow.Date};
        //        session.Store(lastStoreScan, "options/lastStoreScan");
        //    }
        //}

        //private static void GetApps()
        //{
        //    using (_store = new DocumentStore
        //        {
        //            Url = "http://localhost:8081",
        //            //Credentials = new NetworkCredential("itay", "Dark2807", "tzunami"),
        //            DefaultDatabase = "WindowsStore"
        //        }.Initialize())
        //    using (var cache = _store.AggressivelyCacheFor(TimeSpan.FromMinutes(15)))
        //    {
        //        _store.Conventions.MaxNumberOfRequestsPerSession = 82;

        //        var vars = (from accessibilityFilter in Enumerable.Range(0, 1)
        //                    from country in Utils.GetCountries()
        //                    from channel in Enum.GetValues(typeof(Channel)).Cast<Channel>()
        //                    from platform in Enum.GetNames(typeof(Platform))
        //                    where !(channel == Channel.Surface && platform != Platform.ARM.ToString())
        //                    from category in Enum.GetValues(typeof(CategoryIds)).Cast<int>().Where(c => c <= (int)CategoryIds.Government).OrderByDescending(i => i)
        //                    select new BrowseQuery
        //                        {
        //                            AccessibilityFilter = accessibilityFilter,
        //                            Channel = channel,
        //                            Platform = platform,
        //                            Category = category,
        //                            Country = country
        //                        });

        //        var count = vars.Count();

        //        Parallel.ForEach(
        //            vars,
        //            new ParallelOptions {MaxDegreeOfParallelism = 4},
        //            (varsItem, pls, index) =>
        //                {
        //                    //Thread.Sleep((int)index);
        //                    int pageNum = 0;
        //                    int loaded = 0;
        //                    var hasMore = true;

        //                    while (hasMore)
        //                    {
        //                        //Thread.Sleep((30*varsItem.Category) + (10*(pageNum + 1)) + (50*(1 + varsItem.AccessibilityFilter)));
        //                        try
        //                        {
        //                            pageNum++;
        //                            var finalUrl = varsItem.CreateBrowseUrl(pageNum);

        //                            Console.ForegroundColor = ConsoleColor.White;
        //                            Console.WriteLine(
        //                                "({8}/{9}) Getting Platform {0}, Acccessibility {1}, Channel {2}, Language {3}, Locale {4}, Country {5}, Category {6}, PageNum {7}",
        //                                varsItem.Platform, varsItem.AccessibilityFilter,
        //                                varsItem.Channel,
        //                                "0", "en-US", varsItem.Country, varsItem.Category, pageNum,
        //                                index, count);

        //                            using (var response = HttpClient.GetAsync(finalUrl, HttpCompletionOption.ResponseContentRead).Result)
        //                            {
        //                                if (response.StatusCode != HttpStatusCode.OK)
        //                                    break;
        //                                using (var stream = response.Content.ReadAsStreamAsync().Result)
        //                                {
        //                                    var result = WriteApplicationsToRaven("en-US", varsItem, stream, loaded);
        //                                    hasMore = result.Item1;
        //                                    loaded = result.Item2;
        //                                }
        //                            }
        //                        }
        //                        catch (WebException ex)
        //                        {
        //                            Console.ForegroundColor = ConsoleColor.Red;
        //                            Console.WriteLine("Error: {0}", ex.Message);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            Console.ForegroundColor = ConsoleColor.Red;
        //                            Console.WriteLine("Error: {0}", ex.Message);
        //                        }
        //                    }
        //                }
        //            );
        //    }
        //}


        //public class AppInfo
        //{
        //    public string Id { get; set; }
        //    public long Version { get; set; }
        //}
        
        //private static Tuple<bool, int, bool> WriteApplicationsToRaven(string locale, BrowseQuery query, Stream stream, int loaded)
        //{
        //    var xDocument = XDocument.Load(stream);

        //    var total = (int) xDocument.XPathSelectElement("Ptl/AC");

        //    bool madeChanges = false;

        //    using (var session = _store.OpenSession())
        //    {
        //        var apps = xDocument.XPathSelectElements("Ptl/Pts/Pt").Select(
        //            e => new AppInfo
        //            {
        //                Id = e.XPathSelectElement("I").Value,
        //                Version = e.XPathSelectElement("V") != null ? (long)e.XPathSelectElement("V") : 0L
        //            });

        //        var ids = apps.Select(a => a.Id).ToList();
        //        var foundInXml = ids.Count;
        //        loaded += ids.Count();

        //        var existingApps = session.Load<App>(ids.Select(i => "apps/" + i));
        //        if (_newOnly)
        //        {
        //            var enumerable = existingApps
        //                .Where(x => x != null)
        //                .Select(x => x.AppId.ToString());
        //            apps = apps.Where(a => !enumerable.Contains(a.Id)).ToList();
        //        }

        //        Console.ForegroundColor = ConsoleColor.Gray;
        //        Console.WriteLine("Found {0} apps, {1} to review ({2} loaded, {3} total)", foundInXml, ids.Count,
        //                          loaded, total);

        //        Parallel.ForEach(
        //            apps,
        //            new ParallelOptions { MaxDegreeOfParallelism = 4 },
        //            appInfo =>
        //                {
        //                    try
        //                    {
        //                        var app = session.Load<App>("apps/" + appInfo.Id);
        //                        if (app != null)
        //                        {
        //                            if (app.Countries == null)
        //                                app.Countries = new List<string>(1);
        //                            if (!app.Countries.Contains(query.Country))
        //                            {
        //                                app.Countries.Add(query.Country);
        //                                madeChanges = true;
        //                            }
        //                            if (app.Channels == null)
        //                                app.Channels = new List<Channel>(1);
        //                            if (!app.Channels.Contains(query.Channel))
        //                            {
        //                                app.Channels.Add(query.Channel);
        //                                madeChanges = true;
        //                            }

        //                            if (app.Version != appInfo.Version)
        //                            {
        //                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
        //                                Console.WriteLine("App was updated: {0}: {1}", app.Id, app.Title);
        //                            }
        //                            if (madeChanges)
        //                            {
        //                                Console.ForegroundColor = ConsoleColor.Yellow;
        //                                Console.WriteLine("Updated: {0}: {1}", app.Id, app.Title);
        //                            }
        //                        }
        //                        else if (!_updateOnly)
        //                        {
        //                            //Thread.Sleep(loaded / 2);
        //                            app = GetAppDetails(locale, query.Country, query.Channel, appInfo.Id);
        //                            if (app == null)
        //                                return;

        //                            session.Store(app);
        //                            madeChanges = true;

        //                            Console.ForegroundColor = ConsoleColor.Green;
        //                            Console.WriteLine("Added: {0}: {1}", app.Id, app.Title);
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Console.ForegroundColor = ConsoleColor.Red;
        //                        Console.WriteLine("Error: {0}", ex.Message);
        //                    }

        //                }
        //            );
        //        if (madeChanges)
        //        {
        //            Console.ForegroundColor = ConsoleColor.Green;
        //            Console.WriteLine("Saving changes");

        //            session.SaveChanges();
        //        }
        //    }
        //    return Tuple.Create(loaded < total, loaded, madeChanges);
        //}



        //private static App GetAppDetails(string locale, string country, Channel channel, string id)
        //{
        //    const string url =
        //        "https://services.apps.microsoft.com/browse/6.2.9200-1/615/en-US_{locale}/c/{country}/cp/{channel}/Apps/{appId}";

        //    string finalUrl = url.Replace("{appId}", id);
        //    finalUrl = finalUrl.Replace("{locale}", locale);
        //    finalUrl = finalUrl.Replace("{channel}", channel.ToString());
        //    finalUrl = finalUrl.Replace("{country}", country);

        //    try
        //    {
        //        using (var response = HttpClient.GetAsync(finalUrl).Result)
        //        {
        //            if (response.StatusCode != HttpStatusCode.OK)
        //                return null;

        //            using (var stream = response.Content.ReadAsStreamAsync().Result)
        //            {
        //                var xDocument = XDocument.Load(stream);
        //                var app = CreateApp(xDocument);

        //                app.Countries.Add(country);
        //                app.Channels.Add(channel);

        //                return app;
        //            }
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Red;
        //        Console.WriteLine("Error: {0}", ex.Message);
        //        return null;
        //    }
        //}

        //private static void GetAppReviews(App app)
        //{
        //    if (app.RatingCount == 0)
        //    {
        //        return;
        //    }
        //    string url = "https://services.apps.microsoft.com/4R/6.2.9200-1/615/en-US.en/m/{country}/Apps/{appId}/Reviews/all/s/date/1/pn/1";
        //    url = url.Replace("{appId}", app.AppId.ToString());

        //    var reviews = new ConcurrentBag<Tuple<int, int>>();
        //    Parallel.ForEach(
        //        Utils.GetCountries(),
        //        new ParallelOptions { MaxDegreeOfParallelism = 4 },
        //        country =>
        //            {
        //                if (app.RatingCount == reviews.Sum(t => t.Item1))
        //                    return;

        //                string finalUrl = url.Replace("{country}", country);
        //                using (var response = HttpClient.GetAsync(finalUrl).Result)
        //                {
        //                    if (response.StatusCode != HttpStatusCode.OK)
        //                        return;

        //                    var val = response.Content.ReadAsStringAsync().Result;
        //                    {
        //                        val = val.Replace("&", "&amp;");
        //                        var xDocument = XDocument.Load(new StringReader(val));

        //                        var reviewsElem = xDocument.XPathSelectElement("Reviews");
        //                        var totalReviewCount = (int)reviewsElem.XPathSelectElement("TotalReviewCount");
        //                        var totalRatingCount = (int)reviewsElem.XPathSelectElement("TotalRatingCount");

        //                        reviews.Add(Tuple.Create(totalReviewCount, totalRatingCount));
        //                    }
        //                }
        //            }
        //        );

        //    var reviewsCount = reviews.Sum(t => t.Item1);
        //    var reviewsRating = (float)reviews.Sum(t => t.Item2) / reviewsCount;
        //    app.ReviewsRating = reviewsRating;
        //}

        //private static App CreateApp(RavenJObject emr)
        //{
        //    var appPt = emr.Value<RavenJObject>("Pt");

        //    var id = appPt.Value<Guid>("I");
        //    var r = appPt.Value<Guid>("R");
        //    var b = appPt.Value<Guid>("B");
        //    var pfn = appPt.Value<string>("Pfn");
        //    var language = appPt.Value<string>("L");
        //    var title = appPt.Value<string>("T");
        //    var ageRating = appPt.Value<string>("Wr");
        //    var ico = appPt.Value<string>("Ico");
        //    var background = appPt.Value<string>("Bg");
        //    var foreground = appPt.Value<string>("Fg");
        //    var scoreRating = appPt.Value<float>("Sr");
        //    var currency = appPt.Value<string>("Cc");
        //    var price = appPt.Value<float>("P");

        //    var category = appPt.Value<RavenJObject>("C");
        //    var categoryId = category.Value<int>("I");
        //    var categoryName = category.Value<string>("N");

        //    var subcategory = appPt.Value<RavenJObject>("Sc");
        //    var subcategoryId = subcategory != null ? subcategory.Value<int>("I") : 0;
        //    var subcategoryName = subcategory != null ? subcategory.Value<string>("N") : "";

        //    var type = appPt.Value<int>("Typ");
        //    var accessibility = appPt.Value<bool>("Acc");
        //    var dca = appPt.Value<bool>("Dca");
        //    var trial = appPt.Value<bool>("Try");
        //    var lastUpdated = appPt.Value<DateTime>("Lud");


        //    var s = emr.Value<int>("S");
        //    var description = emr.Value<string>("D");
        //    var features = new List<string>(0);
        //    var dbps = emr.Value<RavenJToken>("Dbps");
        //    if (dbps != null)
        //    {
        //        var featuresTokens = dbps.Values<RavenJToken>();
        //        features = featuresTokens
        //            .SelectMany(t => t is RavenJValue ? new[] { t.Value<string>() } : t.Values<string>())
        //            .ToList();
        //    }
        //    var releaseNotes = emr.Value<string>("Ud");
        //    var ratingCount = appPt.Value<int>("Src");
        //    var oc = appPt.Value<string>("Oc");

        //    var sls = emr.Value<RavenJToken>("Sls");
        //    var supportedLanguageTokens = sls.Values<RavenJToken>();
        //    var supportedLanguages = supportedLanguageTokens
        //        .SelectMany(t => t is RavenJValue ? new[] { t.Value<string>() } : t.Values<string>())
        //        .ToList();

        //    var developer = emr.Value<string>("Dev");
        //    var version = emr.Value<long>("V");
        //    var copyrights = emr.Value<string>("Cr");
        //    var website = emr.Value<string>("Ws");
        //    var supportWebsite = emr.Value<string>("Sws");
        //    var privacyPolicy = emr.Value<string>("Pu");
        //    var eula = emr.Value<string>("Eula");
        //    var downloadSize = emr.Value<long>("Ds");

        //    var sas2 = emr.Value<RavenJToken>("Sas2");
        //    var supportedPlatformsTokens = sas2.Values<RavenJToken>();
        //    var selectMany = supportedPlatformsTokens
        //        .SelectMany(t => t.Values<RavenJToken>())
        //        .ToList();
        //    var supportedPlatforms = selectMany
        //        .Select(t => t is RavenJValue ? t.Value<string>() : t.Value<string>("An"))
        //        .ToList();


        //    var permissions = new List<string>(0);
        //    var cs = emr.Value<RavenJToken>("Cs");
        //    if (cs != null)
        //    {
        //        var permissionsTokens = cs.Values<RavenJToken>();
        //        permissions = permissionsTokens
        //            .SelectMany(t => t is RavenJValue ? new[] { t.Value<string>() } : t.Values<string>())
        //            .ToList();
        //    }

        //    var recommendedHardware = new List<string>(0);
        //    var hbps = emr.Value<RavenJToken>("Hbps");
        //    if (hbps != null)
        //    {
        //        var recommendedHardwareTokens = hbps.Values<RavenJToken>();
        //        recommendedHardware = recommendedHardwareTokens
        //            .SelectMany(t => t is RavenJValue ? new[] { t.Value<string>() } : t.Values<string>())
        //            .ToList();
        //    }

        //    var sss = emr.Value<RavenJToken>("Sss");
        //    var screenshotsTokens = sss.Values<RavenJToken>();
        //    var selectMany2 = screenshotsTokens
        //        .SelectMany(t => t.Values<RavenJToken>())
        //        .ToList();
        //    var screenshots = selectMany2
        //        .Select(t => t is RavenJValue
        //                         ? (object)new
        //                         {
        //                             Url = t.Value<string>(),
        //                         }
        //                         : (object)new
        //                         {
        //                             Url = t.Value<string>("U"),
        //                             Caption = t.Value<string>("Cap")
        //                         }).ToList();

        //    return new App
        //        {
        //            Id = "apps/" + id,
        //            AppId = id,
        //            R = r,
        //            B = b,
        //            PackageFamilyName = pfn,
        //            Language = language,
        //            Title = title,
        //            AgeRating = ageRating,
        //            Icon = ico,
        //            Background = background,
        //            Foreground = foreground,
        //            ScoreRating = scoreRating,
        //            Price = new
        //                {
        //                    Currency = currency,
        //                    Amount = price
        //                },
        //            Category = new
        //                {
        //                    Id = categoryId,
        //                    Name = categoryName
        //                },
        //            SubCategory = subcategory != null
        //                              ? new
        //                                  {
        //                                      Id = subcategoryId,
        //                                      Name = subcategoryName
        //                                  }
        //                              : null,
        //            Type = type,
        //            Accessibility = accessibility,
        //            Dca = dca,
        //            Trial = trial,
        //            LastUpdated = lastUpdated,
        //            S = s,
        //            Description = description,
        //            Features = features,
        //            ReleaseNotes = releaseNotes,
        //            RatingCount = ratingCount,
        //            Oc = oc,
        //            SupportedLanguages = supportedLanguages,
        //            Developer = developer,
        //            Version = version,
        //            Copyrights = copyrights,
        //            Website = website,
        //            SupportWebsite = supportWebsite,
        //            PrivacyPolicy = privacyPolicy,
        //            Eula = eula,
        //            DownloadSize = downloadSize,
        //            SupportedPlatforms = supportedPlatforms,
        //            Permissions = permissions,
        //            RecommendedHardware = recommendedHardware,
        //            Screenshots = screenshots
        //        };
        //}
    }
}