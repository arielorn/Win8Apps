using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WSReaper
{
    public class Utils
    {
        private static IEnumerable<string> _countries;

        public static IEnumerable<string> GetCountries()
        {
            return _countries ?? (_countries = GetCountriesCalc().ToList());
        }

        public static IEnumerable<string> GetCountriesCalc()
        {
            var countries = new[]
                {
                    "US",
                    "CA",
                    "FR",
                    "RU",
                    "IN",
                    "CH"
                };

            var otherCountries = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                            .Select(x => new RegionInfo(x.LCID))
                                            .Select(x => x.TwoLetterISORegionName)
                                            .Where(x => x.Length == 2)
                                            .Where(x => !countries.Contains(x))
                                            .Distinct()
                                            .OrderBy(x => x);

            foreach (var country in countries)
            {
                yield return country;
            }
            foreach (var country in otherCountries)
            {
                yield return country;
            }

        }
    }
}