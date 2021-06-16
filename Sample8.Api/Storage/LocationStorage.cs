using Sample8.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sample8.Api.Storage
{
    public class LocationStorage : ILocationStorage
    {
        private IEnumerable<string> Countries { get; set; }
        private IDictionary<string, List<string>> Cities { get; set; }

        public LocationStorage()
        {
            Countries = new List<string>();
            Cities = new Dictionary<string, List<string>>();
        }

        public IEnumerable<string> GetCities()
        {
            return Cities.Values.SelectMany(x => x).ToList();
        }

        public IEnumerable<string> GetCities(string country)
        {
            List<string> cities;
            return Cities.TryGetValue(country.ToLower(), out cities) ? cities : new List<string>(); // чтобы пользователю всегда приходил какой-то результат, вместо 204
        }

        public IEnumerable<string> GetCountries()
        {
            return Countries;
        }

        public void Update(List<CountryModel> model)
        {
            var countries = model.Select(x => x.Country);

            var _cities = new Dictionary<string, List<string>>();
            foreach (var item in model)
            {
                if (_cities.ContainsKey(item.Country.ToLower()))
                    continue;

                _cities.Add(item.Country.ToLower(), item.Cities);
            }
            Cities = _cities;

            Countries = countries.OrderBy(x => x);
        }
    }
}
