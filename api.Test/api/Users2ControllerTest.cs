using System.Net.Http;
using System.Threading.Tasks;
using DaprSample.Api.Datas.Dto;
using DaprSample.Shared.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DaprSample.Api.Tests
{
    [TestFixture]
    class Users2ControllerTest
    {
        private HttpClient _httpClient;
        private IConfiguration _configuration;
        private ApiWebApplicationFactory _factory;
 
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _factory = new ApiWebApplicationFactory(_configuration);
            _httpClient = _factory.CreateClient();
        }
 
        [OneTimeTearDown]
        public void Dispose()
        {
            _factory.Dispose();
            _httpClient.Dispose();
        }
 
        [Test]
        public async Task GetUserByIdTest()
        {
            var userId = 1;
            var getUserResponse =  await _httpClient.GetAsync($"http://localhost:5000/api/v2.0/users/{userId}");
 
            Assert.IsTrue(getUserResponse.IsSuccessStatusCode);
 
            var response = await getUserResponse.Content.ReadAsStringAsync();
            TestContext.WriteLine($"response:{response}");
            var commonResponse = new CommonResponse<User>();
            Assert.DoesNotThrow(() => 
            {
                commonResponse = JsonConvert.DeserializeObject<CommonResponse<User>>(response);
            });
 
            Assert.AreEqual(commonResponse.ErrorCode, 0);
            Assert.AreEqual(commonResponse.Data.Id, userId);
            Assert.AreEqual(commonResponse.Data.Name, "hoge");
        }
    }
}