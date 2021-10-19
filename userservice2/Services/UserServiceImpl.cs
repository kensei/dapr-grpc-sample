using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapr.Client;
using DaprSample.MicroService.Proto.Serivces;
using DaprSample.Shared.Configs.Exceptions;
using DaprSample.Shared.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using StackExchange.Redis;

namespace DaprSample.MicroService.UsersService2.Services
{
    public class UserServiceImpl
    {
        private readonly IConfiguration _configuration;
        private IDatabase _database;
        private DaprClient _daprClient;

        public UserServiceImpl(IConfiguration configuration, IDatabase database, DaprClient daprClient) 
        {
            _configuration = configuration;
            _database = database;
            _daprClient = daprClient;
        }

        public async Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, Grpc.Core.ServerCallContext context)
        {
            System.Console.WriteLine("UsersService2-GetUserById:" + request.Id);
            try {
                using var mysqlConnection = new MySqlConnection(_configuration.GetConnectionString("UserContext"));
                await mysqlConnection.OpenAsync();
                var query = "SELECT * FROM Users WHERE Id = @Id";
                var param = new { Id = request.Id };
                var result = await mysqlConnection.QueryAsync<User>(query, param);
                var user = result.FirstOrDefault();
    
                if (user == null)
                {
                    new ResourceNotFoundException($"user not found {request.Id}");
                }

                var ret = new GetUserByIdResponse
                {
                    User = new Proto.Messages.User()
                    {
                        Id = (ulong)user.Id,
                        Name = user.Name,
                    },
                };
                return ret;
            } catch (MySqlException e) {
                throw new RpcException("mysql error:", e);
            }
        }

        public async Task<AddUserResponse> AddUser(AddUserRequest request, Grpc.Core.ServerCallContext context)
        {
            System.Console.WriteLine("UsersService2-AddUser:" + request.User.Id);
            var ret = new AddUserResponse();
            using var mysqlConnection = new MySqlConnection(_configuration.GetConnectionString("UserContext"));
            await mysqlConnection.OpenAsync();
            using var transaction = await mysqlConnection.BeginTransactionAsync();
            try
            {
                var id = await mysqlConnection.QuerySingleAsync<long>(
                    @"INSERT INTO Users (Name) VALUES (@Name);
                    SELECT last_insert_id()", request.User.Name
                );

                await transaction.CommitAsync();

                ret.User = new Proto.Messages.User
                {
                    Id = (ulong)id,
                    Name = request.User.Name,
                };
                return ret;
            }
            catch (MySqlException e)
            {
                await transaction.RollbackAsync();
                throw new RpcException("mysql error:", e);
            }
        }

        public async Task<LoginResponse> Login(LoginRequest request, Grpc.Core.ServerCallContext context)
        {
            System.Console.WriteLine("UsersService2-Login:" + request.User.Id);
            // var ret = new LoginResponse();
            using var mysqlConnection = new MySqlConnection(_configuration.GetConnectionString("UserContext"));
            await mysqlConnection.OpenAsync();
            using var transaction = await mysqlConnection.BeginTransactionAsync();
            try
            {
                var getUser = await GetUserByIdAsync((long)request.User.Id);

                var storeName = "statestore";
                var key = "user-" + getUser.Id;
                var counter = await _daprClient.GetStateAsync<long>(storeName, key);
                counter++;
                await _daprClient.SaveStateAsync(storeName, key, counter);

                var res = await _daprClient.GetStateAsync<long>(storeName, key);

                var ret = new LoginResponse()
                {
                    LoginCounter = (uint)res,
                };
                return ret;
            }
            catch (MySqlException e)
            {
                await transaction.RollbackAsync();
                throw e;
            }
            catch (ResourceNotFoundException e)
            {
                throw new ResourceNotFoundException(e.Message);
            }
        }

        private async Task<Shared.Models.User> GetUserByIdAsync(long id)
        {
            using var mysqlConnection = new MySqlConnection(_configuration.GetConnectionString("UserContext"));
            await mysqlConnection.OpenAsync();
 
            var query = "SELECT * FROM Users WHERE Id = @Id";
            var param = new { Id = id };
            var result = await mysqlConnection.QueryAsync<Shared.Models.User>(query, param);
            var user = result.FirstOrDefault();
 
            if (user == null)
            {
              throw new ResourceNotFoundException($"User:Id={id} is not found");
            }
 
            return user;
        }
    }
}
