using Microsoft.Extensions.Options;
using Sample8.Api.Storage;
using Sample8.Common;
using Sample8.Models.Common;
using Sample8.Proxy;
using Sample8Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample8.Api.Services
{
    public class LocationService : ILocationService
    {
        #region - Members -
        private readonly IConnector _connector;
        private readonly ILocationStorage _locStorage;
        private readonly IOptions<AppSettings> _url;
        #endregion

        public LocationService(IConnector connector, ILocationStorage locStorage, IOptions<AppSettings> url)
        {
            _connector = connector;
            _locStorage = locStorage;
            _url = url;
        }

        public IEnumerable<string> GetCountries()
        {
            return _locStorage.GetCountries();
        }

        public IEnumerable<string> GetCities()
        {
            return _locStorage.GetCities();
        }

        public IEnumerable<string> GetCities(string country)
        {
            return _locStorage.GetCities(country);
        }

        public async Task<bool> Update()
        {
            var countries = await _connector.RequestAsync<DataContainer>(MapAuthToken.None, MapSerializeType.Json, ContentType.JSON, null, _url.Value.Host, _url.Value.Endpoints.Countries, true, null, null, null, MethodType.GET);

            if (countries == null)
                return false;

            _locStorage.Update(countries.Data);

            return true;
        }
    }
}
