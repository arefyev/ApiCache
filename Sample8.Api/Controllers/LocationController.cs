using Microsoft.AspNetCore.Mvc;
using Sample8.Api.Services;
using System.Collections.Generic;

namespace Sample8Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locService;

        public LocationController(ILocationService locService)
        {
            _locService = locService;
        }

        /// <summary>
        /// Возвращает все страны
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Success</response>
        [HttpGet("countries")]
        public IEnumerable<string> Countries()
        {
            return _locService.GetCountries();
        }

        /// <summary>
        /// Возвращает все города
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Success</response>
        [HttpGet("cities")]
        public IEnumerable<string> Cities()
        {
            return _locService.GetCities();
        }

        /// <summary>
        /// Возвращает все города для определенной страны
        /// </summary>
        /// <param name="country">Страна поиска городов</param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        [HttpGet("cities/{country}")]
        public IEnumerable<string> Cities(string country)
        {
            return _locService.GetCities(country);
        }
    }
}
