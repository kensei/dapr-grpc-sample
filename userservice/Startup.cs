using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaprSample.MicroService.UsersService.Services;
using DaprSample.Shared.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace DaprSample.MicroService.UsersService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
            services.AddDbContext<TestContext>(
                options => options
                    .UseMySql(
                        Configuration.GetConnectionString("UserContext"), 
                        serverVersion,
                        sqlOptions => {
                            sqlOptions.MigrationsAssembly("shared");
                        })
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors() // remove for production
                );
            var redisHost = Configuration.GetSection("RedisSettings").GetValue<string>("RedisHost");
            var config = new ConfigurationOptions()
            {
                EndPoints = { redisHost },
                KeepAlive = 180,
                ReconnectRetryPolicy = new ExponentialRetry(5000)
            };
            var redisDadabase = ConnectionMultiplexer.Connect(config).GetDatabase();
            services.AddSingleton<IDatabase>(redisDadabase);
            services.AddControllers().AddDapr();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "userservice", Version = "v1" });
            });
            services.AddScoped<UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "userservice v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
