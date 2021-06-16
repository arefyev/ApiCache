using Hangfire;
using System;

namespace Sample8.Api.Scheduler
{
    public class SchedulerJob
    {
        public static void Run()
        {
            RecurringJob.RemoveIfExists(nameof(UpdateLocationsJob));
            RecurringJob.AddOrUpdate<UpdateLocationsJob>(nameof(UpdateLocationsJob), job => job.DoJob(), Cron.Hourly(), TimeZoneInfo.Utc);

            BackgroundJob.Enqueue<UpdateLocationsJob>(x => x.DoJob());
        }
    }
}
