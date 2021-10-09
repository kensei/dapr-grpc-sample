using System.Net.Http;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using DaprSample.MicroService.UsersService2.Proto;
using System.Threading.Tasks;

namespace DaprSample.MicroService.UsersService2.Tests
{
    public class UserServiceTest
    {
        private HttpClient _httpClient;
        private UserService2WebApplicationFactory _factory;
        
        [SetUp]
        public void Setup()
        {
            _factory = new UserService2WebApplicationFactory();
            _httpClient = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
            _factory.Dispose();
        }

        [Test]
        public void GetUserByIdTest()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5002", new GrpcChannelOptions
            {
                HttpClient = _httpClient
            });
            var client = new Users.UsersClient(channel);
            
            GetUserByIdResponse response = null;
            var requestParam = new GetUserByIdRequest
            {
                Id = 1
            };
            Assert.DoesNotThrowAsync(async () => 
            {
                response = await client.GetUserByIdAsync(requestParam);
            });
            Assert.AreEqual(response.User.Id, 1);
            Assert.AreEqual(response.User.Name, "user1");
        }
    }
}