using NUnit.Framework;
using Dapr.Client;
using DaprSample.MicroService.Proto.Serivces;

namespace DaprSample.MicroService.UsersService2.Tests
{
    public class UserService2Test
    {
        private UserService2WebApplicationFactory _factory;
        private DaprClient _daprClient;
        
        [SetUp]
        public void Setup()
        {
            _factory = new UserService2WebApplicationFactory();
            _factory.CreateClient();
            _daprClient = _factory.DaprClient;
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
            _daprClient.Dispose();
        }

        [Test]
        public void GetUserByIdTest()
        {
            var request = new GetUserByIdRequest
            {
                Id = 1
            };
            var response = new GetUserByIdResponse();

            Assert.DoesNotThrowAsync(async () => 
            {
                response = await _daprClient.InvokeMethodGrpcAsync<GetUserByIdRequest, GetUserByIdResponse>("test", "GetUserById", request);
            });
            Assert.AreEqual(response.User.Id, 1);
            Assert.AreEqual(response.User.Name, "user1");
        }

        // [Test]
        // public void GetUserByIdTest2()
        // {
        //     var channel = GrpcChannel.ForAddress("http://localhost:5002", new GrpcChannelOptions
        //     {
        //         HttpClient = _httpClient
        //     });
        //     var client = new Dapr.Client.Autogen.Grpc.v1.Dapr.DaprClient(channel);
            
        //     // make request param
        //     var requestParam = new GetUserByIdRequest
        //     {
        //         Id = 1
        //     };
        //     var envelope = new Dapr.Client.Autogen.Grpc.v1.InvokeServiceRequest()
        //     {
        //         Id = "userservice2",
        //         Message = new Dapr.Client.Autogen.Grpc.v1.InvokeRequest()
        //         {
        //             Method = "GetUserById",
        //             ContentType = "application/grpc",
        //             Data = Any.Pack(requestParam),
        //         },
        //     };
        //     var options = new CallOptions(new Metadata(), cancellationToken: default);

        //     var serviceRes = new Dapr.Client.Autogen.Grpc.v1.InvokeResponse();
        //     Assert.DoesNotThrowAsync(async () => 
        //     {
        //         serviceRes = await client.InvokeServiceAsync(envelope, options);
        //     });
        //     var response = serviceRes.Data.Unpack<GetUserByIdResponse>();
        //     Assert.AreEqual(response.User.Id, 1);
        //     Assert.AreEqual(response.User.Name, "user1");
        // }
    }
}