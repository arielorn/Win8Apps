using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Indexes;
using Win8Apps.Indexes;

namespace Win8Apps.Extensions
{
    public static class RavenDbUtils
    {
        public static void TryCreatingFacets(IDocumentStore store)
        {
            using (var session = store.OpenSession())
            {
                //session.Store(new FacetSetup
                //    {
                //        Id = "facets/AppsFacets",
                //        Facets =
                //            {
                //                new Raven.Abstractions.Data.Facet
                //                    {
                //                        Name = "AgeRating",
                //                        TermSortMode = FacetTermSortMode.ValueAsc
                //                    },
                //                new Raven.Abstractions.Data.Facet
                //                    {
                //                        Name = "Developer",
                //                        MaxResults = 10,
                //                        TermSortMode = FacetTermSortMode.HitsDesc
                //                    },
                //                new Raven.Abstractions.Data.Facet
                //                    {
                //                        Name = "Category",
                //                        TermSortMode = FacetTermSortMode.HitsDesc
                //                    },
                //                new Raven.Abstractions.Data.Facet
                //                    {
                //                        Name = "Subcategory",
                //                        TermSortMode = FacetTermSortMode.HitsDesc
                //                    },
                //                new Raven.Abstractions.Data.Facet
                //                    {
                //                        Name = "Country",
                //                        TermSortMode = FacetTermSortMode.HitsDesc,
                //                        MaxResults = 8
                //                    },
                //                new Raven.Abstractions.Data.Facet
                //                    {
                //                        Name = "Language",
                //                        TermSortMode = FacetTermSortMode.HitsDesc,
                //                        MaxResults = 8
                //                    }
                //            }
                //    });

                var facetSetup2 = new FacetSetup
                    {
                        Id = "facets/StatsFacets", 
                        Facets =
                            {
                                new Raven.Abstractions.Data.Facet
                                    {
                                        Name = "Category"
                                    },
                                new Raven.Abstractions.Data.Facet
                                    {
                                        Name = "Language"
                                    },
                                new Raven.Abstractions.Data.Facet
                                    {
                                        Name = "SupportedPlatforms"
                                    },
                                new Raven.Abstractions.Data.Facet
                                    {
                                        Name = "Countries"
                                    },
                                new Raven.Abstractions.Data.Facet
                                    {
                                        Name = "ScoreRating_Range", Mode = FacetMode.Ranges, Ranges =
                                            {
                                                string.Format("[NULL TO {0}]", NumberUtil.NumberToString(1)), 
                                                string.Format("[{0} TO {1}]", NumberUtil.NumberToString(1), NumberUtil.NumberToString(2)), 
                                                string.Format("[{0} TO {1}]", NumberUtil.NumberToString(2), NumberUtil.NumberToString(3)), 
                                                string.Format("[{0} TO {1}]", NumberUtil.NumberToString(3), NumberUtil.NumberToString(4)), 
                                                string.Format("[{0} TO NULL]", NumberUtil.NumberToString(4)),
                                            }
                                    },
                                new Raven.Abstractions.Data.Facet
                                    {
                                        Name = "Price_Range", Mode = FacetMode.Ranges, Ranges =
                                            {
                                                string.Format("[NULL TO {0}]", NumberUtil.NumberToString(0.0f)), 
                                                string.Format("[{0} TO {1}]", NumberUtil.NumberToString(0.0f), NumberUtil.NumberToString(0.999f)), 
                                                string.Format("[{0} TO {1}]", NumberUtil.NumberToString(1.0f), NumberUtil.NumberToString(2.999f)), 
                                                string.Format("[{0} TO {1}]", NumberUtil.NumberToString(3.0f), NumberUtil.NumberToString(4.999f)), 
                                                string.Format("[{0} TO NULL]", NumberUtil.NumberToString(5.0f)),
                                            }
                                    }
                            }
                    };
                session.Store(facetSetup2);

                session.SaveChanges();
            }
        }

        public static void TryCreatingIndexes(IDocumentStore store)
        {
            IndexCreation.CreateIndexes(typeof(Apps_Search2).Assembly, store);
        }
    }
}