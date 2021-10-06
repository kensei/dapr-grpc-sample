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

namespace DaprSample.Api.Services
{
    public class UserService
    {
        private DaprClient _daprClient;

        public UserService(DaprClient httpClient) 
        {
            _daprClient = httpClient;
        }
 
        // Userレコードを1件取得
        public async Task<User> GetUserById(long id)
        {
            try {
                var request = _daprClient.CreateInvokeMethodRequest(EnumServiceName.UserService.ToString().ToLower(), "/api/users/" + id);
                request.Method = HttpMethod.Get;
                var response = await _daprClient.InvokeMethodWithResponseAsync(request);
                if(response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var deserializedResponse = ServiceResponseHelper.DeserializeObject<User>(jsonString);
                    return deserializedResponse.Data;
                }
                else
                {
                    throw new ServiceCallFailException("response is not success.");
                }
            }
            catch (InvocationException e)
            {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine(e.StackTrace);
                throw new ServiceCallFailException(e.Message);
            }
        }
 
        // Userレコードを1件作成
        public async Task<User> AddUser(User user)
        {
            try {
                var request = _daprClient.CreateInvokeMethodRequest(EnumServiceName.UserService.ToString().ToLower(), "/api/users/");
                request.Method = HttpMethod.Post;
                var paramJson = JsonConvert.SerializeObject(user);
                request.Content = new StringContent(paramJson, Encoding.UTF8, @"application/json");
                var response = await _daprClient.InvokeMethodWithResponseAsync(request);
                if(response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var deserializedResponse = ServiceResponseHelper.DeserializeObject<User>(jsonString);
                    return deserializedResponse.Data;
                }
                else
                {
                    throw new ServiceCallFailException("response is not success.");
                }
            } catch (InvocationException e)
            {
                throw new ServiceCallFailException(e.Message);
            }
        }

        public async Task<UserResponse> Login(User user)
        {
            try {
                var request = _daprClient.CreateInvokeMethodRequest(EnumServiceName.UserService.ToString().ToLower(), "/api/users/login");
                request.Method = HttpMethod.Post;
                var paramJson = JsonConvert.SerializeObject(user);
                request.Content = new StringContent(paramJson, Encoding.UTF8, @"application/json");
                var response = await _daprClient.InvokeMethodWithResponseAsync(request);
                if(response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var deserializedResponse = ServiceResponseHelper.DeserializeObject<UserResponse>(jsonString);
                    return deserializedResponse.Data;
                }
                else
                {
                    throw new ServiceCallFailException("response is not success.");
                }
            } catch (InvocationException e)
            {
                throw new ServiceCallFailException(e.Message);
            }
        }
    }
}
