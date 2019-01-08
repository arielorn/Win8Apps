using Win8Apps.Model;

namespace WSReaper
{
    public class BrowseQuery
    {
        private const string Url =
            "https://services.apps.microsoft.com/browse/6.2.9200-1/615/en-US_{locale}/c/{country}/cp/{channel}/AppTileList/cid/{category}/pf/1/pc/0/pt/{platform}/af/{accessibilityFilter}/lf/{language}/s/3/2/pn/{pageNum}";

        public BrowseQuery()
        {
        }

        public BrowseQuery(BrowseQuery existing)
        {
            AccessibilityFilter = existing.AccessibilityFilter;
            Channel = existing.Channel;
            Platform = existing.Platform;
            Category = existing.Category;
            Country = existing.Country;
        }

        public string CreateBrowseUrl(int pageNum)
        {


            var finalUrl = Url.Replace("{platform}", Platform);
            finalUrl = finalUrl.Replace("{accessibilityFilter}", AccessibilityFilter.ToString());
            finalUrl = finalUrl.Replace("{channel}", ((int)Channel).ToString());
            finalUrl = finalUrl.Replace("{category}", Category.ToString());
            finalUrl = finalUrl.Replace("{language}", "0");
            finalUrl = finalUrl.Replace("{country}", Country);
            finalUrl = finalUrl.Replace("{locale}", "en-US");
            finalUrl = finalUrl.Replace("{pageNum}", pageNum.ToString());
            return finalUrl;
        }


        public int AccessibilityFilter;
        public Channel Channel;
        public string Platform;
        public int Category;
        public string Country;
    }
}