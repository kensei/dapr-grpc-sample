using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DaprSample.Api.Datas.Dto;
using DaprSample.Api.Configs.Exceptions;
using DaprSample.Shared.Models;
using DaprSample.Api.Helpers;
using System.Text;

namespace DaprSample.Api.Services
{
    public class UserService
    {
        private HttpClient _httpClient;

        public UserService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }
 
        // Userレコードを1件取得
        public async Task<User> GetUserById(long id)
        {
            var response = await _httpClient.GetAsync("/api/users/" + id);
            if(response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var deserializedResponse = ServiceResponseHelper.DeserializeObject<User>(jsonString);
                return deserializedResponse.Data;
            }
            else
            {
                throw new ServiceCallFailException();
            }
        }
 
        // Userレコードを1件作成
        public async Task<User> AddUser(User user)
        {
            var paramJson = JsonConvert.SerializeObject(user);
            var paramContent = new StringContent(paramJson, Encoding.UTF8, @"application/json");
            var response = await _httpClient.PostAsync("/api/users/", paramContent);
            if(response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var deserializedResponse = ServiceResponseHelper.DeserializeObject<User>(jsonString);
                return deserializedResponse.Data;
            }
            else
            {
                throw new ServiceCallFailException();
            }
        }

        public async Task<UserResponse> Login(User user)
        {
            var paramJson = JsonConvert.SerializeObject(user);
            var paramContent = new StringContent(paramJson, Encoding.UTF8, @"application/json");
            var response = await _httpClient.PostAsync("/api/users/login", paramContent);
            if(response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var deserializedResponse = ServiceResponseHelper.DeserializeObject<UserResponse>(jsonString);
                return deserializedResponse.Data;
            }
            else
            {
                throw new ServiceCallFailException();
            }
        }
    }
}
