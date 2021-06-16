using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Sample8.Api.Scheduler;
using Sample8.Api.Services;
using Sample8.Api.Storage;
using Sample8.Proxy;
using Sample8Models.Common;
using System.IO;
using System.Text.Json;

namespace Sample8Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
       WebHost.CreateDefaultBuilder(args)
              .ConfigureAppConfiguration((hostingContext, config) =>
              {
                  config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
              })
              .UseStartup<Startup>();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {// ужать до особых разрешений на практике
                options.AddDefaultPolicy(
                                 builder => builder
                         .SetIsOriginAllowed(_ => true)
                         .AllowAnyHeader().AllowAnyMethod().AllowCredentials()
                        );
            });

            services.AddCors().AddMvc().AddNewtonsoftJson()
            .AddMvcOptions(options => { })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "«адание 8 API", Version = "v1" });

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Sample8Api.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));


            services
                    .AddSingleton<ILocationStorage, LocationStorage>()
                    .AddSingleton<IConnector, NetConnector>()
                    .AddScoped<ILocationService, LocationService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
                c.RoutePrefix = string.Empty;
            });

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            SchedulerJob.Run();
        }
    }
}
