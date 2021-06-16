using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample8.Api.Services
{
    public interface ILocationService
    {
        /// <summary>
        /// Возвращает все страны
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetCountries();
        /// <summary>
        /// Возвращает все города всех стран
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetCities();
        /// <summary>
        /// Возвращает список город определенной страны
        /// </summary>
        /// <param name="country">Страна поиска</param>
        /// <returns></returns>
        IEnumerable<string> GetCities(string country);
        /// <summary>
        /// Обновляет храгилище данных
        /// </summary>
        /// <returns></returns>
        Task<bool> Update();
    }
}
