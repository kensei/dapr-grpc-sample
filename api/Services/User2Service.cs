using System;
using System.Net.Http;
using System.Threading.Tasks;
using DaprSample.Api.Datas.Dto;
using DaprSample.Api.Configs.Exceptions;
using DaprSample.Shared.Models;
using DaprSample.Api.Datas.Enums;
using DaprSample.Api.Helpers;
using Dapr.Client;
using System.Text;
using Newtonsoft.Json;
using DaprSample.MicroService.Proto.Serivces;
using DaprSample.Shared.Configs.Exceptions;

namespace DaprSample.Api.Services
{
    public class User2Service
    {
        private DaprClient _daprClient;

        public User2Service(DaprClient httpClient) 
        {
            _daprClient = httpClient;
        }
 
        // Userレコードを1件取得
        public async Task<User> GetUserById(long id)
        {
            try {
                var request = new GetUserByIdRequest()
                {
                    Id = (ulong)id
                };
                var rpcRes = await _daprClient.InvokeMethodGrpcAsync<GetUserByIdRequest, GetUserByIdResponse>(EnumServiceName.UserService2.ToString().ToLower(), "GetUserById", request);
                var res = new User()
                {
                    Id = (long)rpcRes.User.Id,
                    Name = rpcRes.User.Name,
                };
                return res;
            }
            catch (RpcException e)
            {
                throw new ServiceCallFailException($"rpc inner exception {e.Message}");
            }
            catch (InvocationException e)
            {
                throw new ServiceCallFailException($"invoce exception {e.Message}");
            }
        }
 
        // Userレコードを1件作成
        public async Task<User> AddUser(User user)
        {
            try {
                var request = new AddUserRequest()
                {
                    User = new MicroService.Proto.Messages.User()
                    {
                        Id = (ulong)user.Id,
                    }
                };
                var rpcRes = await _daprClient.InvokeMethodGrpcAsync<AddUserRequest, AddUserResponse>(EnumServiceName.UserService2.ToString().ToLower(), "AddUser", request);
                var res = new User()
                {
                    Id = (long)rpcRes.User.Id,
                    Name = rpcRes.User.Name,
                };
                return res;
            }
            catch (RpcException e)
            {
                throw new ServiceCallFailException($"rpc inner exception {e.Message}");
            }
            catch (InvocationException e)
            {
                throw new ServiceCallFailException($"invoce exception {e.Message}");
            }
        }

        public async Task<UserResponse> Login(User user)
        {
            try {
                var request = new LoginRequest()
                {
                    User = new MicroService.Proto.Messages.User()
                    {
                        Id = (ulong)user.Id,
                    }
                };
                var rpcRes = await _daprClient.InvokeMethodGrpcAsync<LoginRequest, LoginResponse>(EnumServiceName.UserService2.ToString().ToLower(), "Login", request);
                var res = new UserResponse()
                {
                    LoginCounter = rpcRes.LoginCounter,
                };
                return res;
            }
            catch (RpcException e)
            {
                throw new ServiceCallFailException($"rpc inner exception {e.Message}");
            }
            catch (InvocationException e)
            {
                throw new ServiceCallFailException($"invoce exception {e.Message}");
            }
        }
    }
}
