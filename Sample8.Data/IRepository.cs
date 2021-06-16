using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample8Data
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Returns all elements
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> List();
        /// <summary>
        /// Returns a certain element
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> Get(int id);
        /// <summary>
        /// Finds element with IDs
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> Find(int[] ids);
        /// <summary>
        /// Sets path to service
        /// </summary>
        /// <param name="url"></param>
        void SetConfig(string url);
    }
}
