using System.Collections.Generic;
using Raven.Abstractions.Data;

namespace Win8Apps.Model
{
    public class Facet
    {
        public string Title { get; set; }
        public List<FacetValue> Values { get; set; }

        public Facet()
        {
            Values = new List<FacetValue>();
        }
    }
}
