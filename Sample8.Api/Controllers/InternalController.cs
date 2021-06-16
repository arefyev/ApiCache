using Microsoft.AspNetCore.Mvc;
using Sample8.Api.Services;
using System.Threading.Tasks;

namespace Sample8.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternalController : ControllerBase
    {
        private readonly ILocationService _locService;

        public InternalController(ILocationService locService)
        {
            _locService = locService;
        }

        [HttpGet("update")]
        public async Task<bool> Update()
        {
            await _locService.Update();
            return true;
        }
    }
}
