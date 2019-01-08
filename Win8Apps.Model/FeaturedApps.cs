using System;
using System.Collections.Generic;

namespace Win8Apps.Model
{
    public class AppsList
    {
        public string Id { get; set; }

        public bool IsFeatured { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime LastDateSeen { get; set; }
        
        public string Image { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        
        public List<string> Apps { get; set; }
    }
}