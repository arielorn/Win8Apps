using System;

namespace Win8Apps.Model
{
    public class Stat
    {
        public string Id
        {
            get { return "stats/" + Date.ToString("yyyy-M-d"); }
        }

        public DateTime Date { get; set; }
        public int AppsCount { get; set; }
        public int Developers { get; set; }
    }
}