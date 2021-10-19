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
                    var getUserByIdRequest = request.Data.Unpack<GetUserByIdRequest>();
                    var getUserByIdResponse = await _serviceImpl.GetUserById(getUserByIdRequest, context);
                    response.Data = Any.Pack(getUserByIdResponse);
                    break;
                case "AddUser":
                    var addUserRequest = request.Data.Unpack<AddUserRequest>();
                    var addUserResponse = await _serviceImpl.AddUser(addUserRequest, context);
                    response.Data = Any.Pack(addUserResponse);
                    break;
                case "Login":
                    var loginRequest = request.Data.Unpack<LoginRequest>();
                    var loginResponse = await _serviceImpl.Login(loginRequest, context);
                    response.Data = Any.Pack(loginResponse);
                    break;
                default:
                    break;
            }
            return response;
        }
    }
}