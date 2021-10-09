using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DaprSample.Shared.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace DaprSample.MicroService.UsersService2.Tests
{
    public class UserService2WebApplicationFactory: WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.Test.json", optional: false)
                    .AddEnvironmentVariables()
                    .Build();
                var connectionString = config.GetConnectionString("TestUserContext");
 
                var descriptor = services.SingleOrDefault( d => d.ServiceType == typeof(DbContextOptions<TestContext>));
                services.Remove(descriptor);
 
                var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
                services.AddDbContext<TestContext>(
                options => options
                    .UseMySql(connectionString, serverVersion)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                );
                services.AddSingleton<MySqlConnection>(c => new MySqlConnection(connectionString));
 
                var sp = services.BuildServiceProvider(); 
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TestContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
 
                CreateTestUser(db);
            });
        }

        private void CreateTestUser(TestContext context)
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