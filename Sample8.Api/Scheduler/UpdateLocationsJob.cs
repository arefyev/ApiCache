using Microsoft.Extensions.Options;
using Sample8.Api.Services;
using Sample8Models.Common;
using System;
using System.Threading.Tasks;

namespace Sample8.Api.Scheduler
{
    public class UpdateLocationsJob : IJob
    {
        private readonly ILocationService _locService;
        private readonly IOptions<AppSettings> _settings;
        private int _counter;

        public UpdateLocationsJob(ILocationService locService, IOptions<AppSettings> settings)
        {
            _locService = locService;
            _settings = settings;
        }

        public async Task<bool> DoJob()
        {
            try
            {
                await RecalculateData();
            }
            catch (Exception e)
            {
                //log
                return false;
            }
            return true;
        }

        private async Task<bool> RecalculateData()
        {
            if (await _locService.Update())
                return true;

            if (_counter > 5) // оставим 5 попыток для получения данных, в случае неуспеха в кеше останутся прежние данные
            {
                _counter = 0;
                return false;
            }

            await Task.Delay(_settings.Value.Time2Retry * 1000);

            return await RecalculateData();
        }
    }
}
