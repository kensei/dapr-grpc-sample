using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DaprSample.Api.Datas.Dto;
using DaprSample.Api.Configs.Exceptions;
using DaprSample.Shared.Models;


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
                System.Console.WriteLine("result:" + jsonString);
                try {
                    var userResponse = JsonConvert.DeserializeObject<CommonResponse<User>>(jsonString);
                    System.Console.WriteLine("errorcode:" + userResponse.ErrorCode);
                    System.Console.WriteLine("id:" + userResponse.Data.Id);
                    System.Console.WriteLine("name:" + userResponse.Data.Name);
                    return userResponse.Data;
                } catch (JsonException e) {
                    try {
                        var errorResponse = JsonConvert.DeserializeObject<CommonResponse<string>>(jsonString);
                        throw new ServiceCallFailException(errorResponse.Data);
                    } catch (JsonException) {
                        throw new ServiceCallFailException(e.Message);
                    }
                }
            }
            else
            {
                throw new ServiceCallFailException();
            }
        }
 
        // Userレコードを1件作成
        public async Task<User> AddUser(User user)
        {
            var response = await _httpClient.GetAsync("/api/users/" + user.Id);
            if(response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<CommonResponse<User>>(jsonString);
                return userResponse.Data;
            }
            else
            {
                throw new ServiceCallFailException();
            }
        }

        public async Task<UserResponse> Login(User user)
        {
            var response = await _httpClient.GetAsync("/api/users/" + user.Id);
            if(response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<CommonResponse<UserResponse>>(jsonString);
                return userResponse.Data;
            }
            else
            {
                throw new ServiceCallFailException();
            }
        }
    }
}
