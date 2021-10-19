using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Dapr.Client;
using Dapr.Client.Autogen.Grpc.v1;
using Moq;
using NUnit.Framework;
using DaprSample.Shared.Models;
using DaprSample.MicroService.Proto.Serivces;

namespace DaprSample.Api.Tests
{
    public class ApiWebApplicationFactory: WebApplicationFactory<Startup>
    {
        private IConfiguration _configuration;
 
        public ApiWebApplicationFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }
 
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.Test.json", optional: true)
                    .AddEnvironmentVariables();
                _configuration = builder.Build();

                var connectionString = _configuration.GetConnectionString("TestUserContext");
                TestContext.WriteLine($"connection:{connectionString}");

                // dapr mock
                var client = new MockClient();
                SetupGetUserByIdMock(client);
                services.AddSingleton<DaprClient>(_ => client.DaprClient);

                // dapper
                services.AddSingleton<MySqlConnection>(c => new MySqlConnection(connectionString));

                // database recreate
                services.AddDbContext<TestDbContext>(
                    options => options
                        .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)))
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors()
                    );
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var dbContext = scopedServices.GetRequiredService<TestDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<ApiWebApplicationFactory>>();
                    try
                    {
                        dbContext.Database.EnsureDeleted();
                        dbContext.Database.EnsureCreated();
                        InitializeDbForTests(dbContext);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "An error occurred seeding the " +
                            "database with test messages. Error: {Message}", e.Message);
                    }
                }
            }).ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
        }

        private MockClient SetupGetUserByIdMock(MockClient mockClient)
        {
            var responseData = new GetUserByIdResponse() {
                User = new DaprSample.MicroService.Proto.Messages.User()
                {
                    Id = 1,
                    Name = "hoge",
                }
            };
            var invokeResponse = new InvokeResponse
            {
                Data = Any.Pack(responseData),
            };
            var response = mockClient.Call<InvokeResponse>()
                .SetResponse(invokeResponse)
                .Build();
            var rpcStatus = new Status(StatusCode.OK, "");
            mockClient.Mock
                .Setup(m => m.InvokeServiceAsync(It.IsAny<Dapr.Client.Autogen.Grpc.v1.InvokeServiceRequest>(), It.IsAny<CallOptions>()))
                .Returns(response);
            
            return mockClient;
        }

        private void InitializeDbForTests(TestDbContext db)
        {
            db.Users.AddRange(GetSeedingUsers());
            db.SaveChanges();
        }

        private List<User> GetSeedingUsers()
        {
            return new List<User>()
            {
                new User() { Id = 1, Name = "hoge" },
            };
        }
    }
}
