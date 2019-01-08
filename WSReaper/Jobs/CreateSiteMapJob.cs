using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Quartz;
using Win8Apps.Model;
using WSReaper.Helpers;

namespace WSReaper.Jobs
{
    public class CreateSiteMapJob : ReapJob
    {
        public List<App> AllApps { get; set; }

        protected override void ConcreteExecute(IJobExecutionContext context)
        {
            XNamespace ns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
            XElement urlset;
            var siteMaps = new List<string>();

            if (AllApps == null)
                return;

            var appsByCat = AllApps
                .GroupBy(x => x.Category.Name);

            var topics = Store.GetAll(s => s.Query<AppsList>())
                .Where(t => t.IsFeatured == false);

            var pages = new[]
                {
                    "", 
                    "/",
                    "/Apps/Recent",
                    "/Apps/TopGames",
                    "/Apps/TopFreeGames",
                    "/Apps/TopApps",
                    "/Apps/TopFreeApps",
                    "/Apps/Topics",
                    "/Apps/Search",
                    "/Apps/GoneFree",
                    "/Stats/Overview",
                    "/Stats/Apps",
                    "/Stats/Developers",
                    "/Home/AboutUs",
                    "/Home/Advertise",
                    "/Home/ContactUs"
                };

            foreach (var categroy in appsByCat)
            {
                var catName = categroy.Key;

                var siteMapDocument = new XDocument(urlset = new XElement(ns + "urlset"));
                foreach (var app in categroy)
                {
                    urlset.Add(new XElement(ns + "url",
                                            new XElement(ns + "loc", "http://www.winapps.co/Apps/App/" + app.AppId),
                                            new XElement(ns + "changefreq", "daily"),
                                            new XElement(ns + "priority", "0.7")
                                   ));

                    urlset.Add(new XElement(ns + "url",
                                            new XElement(ns + "loc", "http://www.winapps.co/Apps/App/" + app.FriendlyUrl),
                                            new XElement(ns + "changefreq", "daily"),
                                            new XElement(ns + "priority", "0.7")
                                   ));

                }

                var siteMap = "sitemap-" + SlugConverter.TitleToSlug(catName) + ".xml";
                siteMaps.Add(siteMap);
                siteMapDocument.Save(String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, siteMap));

            }

            var siteMapCategories = new XDocument(urlset = new XElement(ns + "urlset"));
            foreach (var category in appsByCat)
            {
                urlset.Add(new XElement(ns + "url",
                                        new XElement(ns + "loc",
                                                     string.Format(
                                                         "http://www.winapps.co/Apps/Search?category={0}",
                                                         HttpUtility.UrlEncode(category.Key))),
                                                     new XElement(ns + "changefreq", "daily"),
                                                     new XElement(ns + "priority", "0.9")
                                            ));

                var appsBySubCat = category
                    .Where(x => x.SubCategory != null)
                    .GroupBy(x => x.SubCategory.Name);

                if (appsBySubCat.Any())
                {

                    foreach (var subCategory in appsBySubCat)
                    {
                        urlset.Add(new XElement(ns + "url",
                                                new XElement(ns + "loc",
                                                             string.Format(
                                                                 "http://www.winapps.co/Apps/Search?category={0}&Subcategory={1}",
                                                                 HttpUtility.UrlEncode(category.Key),
                                                                 HttpUtility.UrlEncode(subCategory.Key))),
                                                new XElement(ns + "changefreq", "daily"),
                                                new XElement(ns + "priority", "0.9")
                                       ));

                    }

                    urlset.Add(new XElement(ns + "url",
                                            new XElement(ns + "loc",
                                                         string.Format("http://www.winapps.co/Apps/Search?category={0}&Subcategory={1}",
                                                         HttpUtility.UrlEncode(category.Key),
                                                         "EMPTY_STRING")),
                                            new XElement(ns + "changefreq", "daily"),
                                            new XElement(ns + "priority", "0.9")
                                   ));
                }
            }
            siteMaps.Add("sitemap-categories.xml");
            siteMapCategories.Save(String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, "sitemap-categories.xml"));


            var siteMapTopics = new XDocument(urlset = new XElement(ns + "urlset"));
            foreach (var topic in topics)
            {
                urlset.Add(new XElement(ns + "url",
                                        new XElement(ns + "loc",
                                                     string.Format(
                                                         "http://www.winapps.co/Apps/Topic?topicId={0}",
                                                         HttpUtility.UrlEncode(topic.Id))),
                                        new XElement(ns + "changefreq", "daily"),
                                        new XElement(ns + "priority", "0.9")
                               ));
            }
            siteMaps.Add("sitemap-topics.xml");
            siteMapTopics.Save(String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, "sitemap-topics.xml"));


            var siteMapBuiltIn = new XDocument(urlset = new XElement(ns + "urlset"));
            foreach (var page in pages)
            {
                urlset.Add(new XElement(ns + "url",
                                        new XElement(ns + "loc", "http://www.winapps.co" + page),
                                        new XElement(ns + "changefreq", "daily"),
                                        new XElement(ns + "priority", "1.0")
                               ));
            }
            siteMaps.Add("sitemap-pages.xml");
            siteMapBuiltIn.Save(string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, "sitemap-pages.xml"));

            var siteMapIndexDocument = new XDocument(urlset = new XElement(ns + "sitemapindex"));
            foreach (var siteMap in siteMaps)
            {
                urlset.Add(new XElement(ns + "sitemap",
                                        new XElement(ns + "loc", "http://www.winapps.co/sitemaps/" + siteMap),
                                        new XElement(ns + "lastmod", DateTime.Now.ToString("yyyy-MM-dd"))
                               ));

            }

            siteMapIndexDocument.Save(String.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, "sitemap.xml"));
        }
    }
}