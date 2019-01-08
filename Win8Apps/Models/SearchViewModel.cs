using System;
using System.Collections.Generic;
using Win8Apps.Model;

namespace Win8Apps.Models
{
    public class SearchViewModel
    {
        public const int PageSize = 21;
        public const int DefaultPage = 1;

        public string QueryString { get; set; }
        public IEnumerable<App> Apps { get; set; }
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<Facet> Facets { get; set; }

        public bool HasNextPage
        {
            get { return CurrentPage * PageSize < TotalCount; }
        }

        public bool HasPrevPage
        {
            get { return CurrentPage * PageSize > PageSize * DefaultPage; }
        }

        public int MaxPage
        {
            get { return (int)Math.Ceiling((double)TotalCount / PageSize); }
        }

    }
}