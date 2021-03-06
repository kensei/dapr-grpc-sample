using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dapr;
using Dapr.Client;
using Grpc.Net.Client;
using MySql.Data.MySqlClient;
using DaprSample.MicroService.UsersService2.Services;
using DaprSample.Shared.Models;

namespace DaprSample.MicroService.UsersService2.Tests
{
    public class UserService2WebApplicationFactory: WebApplicationFactory<Startup>
    {
        public DaprClient DaprClient { get; private set; }
        public HttpClient HttpClient { get; private set; }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.Test.json", optional: false)
                    .AddEnvironmentVariables()
                    .Build();
                var connectionString = config.GetConnectionString("UserContext");
 
                var descriptor = services.SingleOrDefault( d => d.ServiceType == typeof(DbContextOptions<TestDbContext>));
                services.Remove(descriptor);
 
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
                services.AddDbContext<TestDbContext>(
                options => options
                    .UseMySql(connectionString, serverVersion)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                );
                services.AddSingleton<MySqlConnection>(c => new MySqlConnection(connectionString));

                HttpClient = new AppCallbackClient(new UserService(default, default, new UserServiceImpl(config, default, default)));
                DaprClient = new DaprClientBuilder()
                    .UseGrpcChannelOptions(new GrpcChannelOptions(){ HttpClient = HttpClient, })
                    .UseJsonSerializationOptions(new JsonSerializerOptions())
                    .Build();
 
                var sp = services.BuildServiceProvider(); 
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TestDbContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
 
                CreateTestUser(db);
            });
        }

        private void CreateTestUser(TestDbContext context)
        {
            var user = new User
            {
                Id = 1,
                Name = "user1"
            };
 
            context.Users.AddRange(user);
            context.SaveChanges();           
        }
    }
}