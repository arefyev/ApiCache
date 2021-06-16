using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Sample8Models;

namespace Sample8Data
{
    public abstract class Repository<T> : IRepository<T>
    {
        protected string _url;
        protected string _contoller;

        public virtual Task<T> Get(int id)
        {
            return GetStringAsync<T>($"{_url}/{_contoller}/{id}");
        }

        public virtual Task<IEnumerable<T>> List()
        {
            return GetStringAsync<IEnumerable<T>>($"{_url}/{_contoller}/");
        }

        public virtual async Task<IEnumerable<T>> Find(int[] ids)
        {
            var items = await List();

            return items.Where(x => ids.Contains(((IModel)x).Id));
        }

        public void SetConfig(string url)
        {
            _url = url;
        }

        protected async Task<T> GetStringAsync<T>(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var data = await httpClient.GetStringAsync(url);

                return JsonConvert.DeserializeObject<T>(data);
            }
        }
    }
}