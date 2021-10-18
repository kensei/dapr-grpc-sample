using System;
using System.Threading.Tasks;
using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client;
using Dapr.Client.Autogen.Grpc.v1;
using DaprSample.MicroService.Proto.Serivces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DaprSample.MicroService.UsersService2.Services
{
    public class UserService : AppCallback.AppCallbackBase
    {
        private readonly ILogger<UserService> _logger;
        private readonly DaprClient _daprClient;
        private readonly UserServiceImpl _serviceImpl;

        public UserService(DaprClient daprClient, ILogger<UserService> logger, UserServiceImpl serviceImpl)
        {
            _daprClient = daprClient;
            _logger = logger;
            _serviceImpl = serviceImpl;
        }

        public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
        {
            var response = new InvokeResponse();
            Console.WriteLine($"method:{request.Method}");
            switch (request.Method)
            {
                case "GetUserById":                
                    var input = request.Data.Unpack<GetUserByIdRequest>();
                    var output = await _serviceImpl.GetUserById(input, context);
                    response.Data = Any.Pack(output);
                    break;
                case "AddUser":
                case "Login":
                    break;
                default:
                    break;
            }
            return response;
        }
    }
}