using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using StackExchange.Redis;
using Dapr.Client;
using DaprSample.Shared.Models;
using DaprSample.MicroService.UsersService.Datas.Dto;
using DaprSample.Shared.Configs.Exceptions;

namespace DaprSample.MicroService.UsersService.Services
{
    public class UserService
    {
        private readonly IConfiguration _configuration;
        private IDatabase _database;
        private DaprClient _daprClient;

        public UserService(IConfiguration configuration, IDatabase database, DaprClient daprClient) 
        {
            _configuration = configuration;
            _database = database;
            _daprClient = daprClient;
        }
 
        // Userレコードを1件取得
        public async Task<User> GetUserById(long id)
        {
            System.Console.WriteLine("UserService-GetUserById:" + id);
            using var mysqlConnection = new MySqlConnection(_configuration.GetConnectionString("UserContext"));
            await mysqlConnection.OpenAsync();
 
            var query = "SELECT * FROM Users WHERE Id = @Id";
            var param = new { Id = id };
            var result = await mysqlConnection.QueryAsync<User>(query, param);
            var user = result.FirstOrDefault();
 
            if (user == null)
            {
              throw new ResourceNotFoundException($"User:Id={id} is not found");
            }
 
            return user;
        }
 
        // Userレコードを1件作成
        public async Task<User> AddUser(User user)
        {
            System.Console.WriteLine("UserService-AddUser:" + user.Id);
            using var mysqlConnection = new MySqlConnection(_configuration.GetConnectionString("UserContext"));
            await mysqlConnection.OpenAsync();
 
            using var transaction = await mysqlConnection.BeginTransactionAsync();

            try
            {
                var id = mysqlConnection.QuerySingleAsync<long>(
                    @"INSERT INTO Users (Name) VALUES (@Name);
                    SELECT last_insert_id()", user
                );

                await transaction.CommitAsync();

                user.Id = id.Result;
            }
            catch (MySqlException e)
            {
                await transaction.RollbackAsync();
                throw e;
            }
 
            return user;
        }

        public async Task<UserResponse> Login(User user)
        {
            System.Console.WriteLine("UserService-Login:" + user.Id);
            using var mysqlConnection = new MySqlConnection(_configuration.GetConnectionString("UserContext"));
            await mysqlConnection.OpenAsync();
            using var transaction = await mysqlConnection.BeginTransactionAsync();
            var res = new UserResponse();
            try
            {
                var getUser = await GetUserById(user.Id);

                var storeName = "statestore";
                var key = "user-" + getUser.Id;
                var counter = await _daprClient.GetStateAsync<long>(storeName, key);
                counter++;
                await _daprClient.SaveStateAsync(storeName, key, counter);

                res.LoginCounter = await _daprClient.GetStateAsync<long>(storeName, key);
            }
            catch (MySqlException e)
            {
                await transaction.RollbackAsync();
                throw e;
            }
 
            return res;
        }
    }
}
