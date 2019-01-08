using System.Collections.Generic;
using Win8Apps.Model;

namespace Win8Apps.Models
{
    public class TopicViewModel
    {
        public AppsList Topic { get; set; }
        public IEnumerable<App> Apps { get; set; }
    }
}