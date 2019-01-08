using System.Collections.Generic;

namespace Win8Apps.Model.Boost
{
    public class BoostedResults
    {
        public string Id { get { return "options/boostedResults"; } }

        public IEnumerable<BoostedCriteria> Developers { get; set; }
        public IEnumerable<BoostedCriteria> Languages { get; set; }
        public IEnumerable<BoostedCriteria> Apps { get; set; }
    }
}