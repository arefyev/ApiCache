using Sample8Models.Common;
using Sample8Models.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample8Biz
{
    public interface IService<T>
    {
        Task<IEnumerable<T>> List();

        Task<T> Get(int id);

        Task<SearchResultView> List(SearchParams searchParams);

        Task<object> Import(SearchParams searchParams);
    }
}
