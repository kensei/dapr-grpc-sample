
using System.Linq;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Configuration;
using Dapper;
using StackExchange.Redis;
using MySql.Data.MySqlClient;
using DaprSample.MicroService.UsersService2.Proto;
using DaprSample.Shared.Configs.Exceptions;

namespace DaprSample.MicroService.UsersService2.Services
{
    public class  UsersService : DaprSample.MicroService.UsersService2.Proto.Users.UsersBase
    {
        private readonly IConfiguration _configuration;
        private IDatabase _database;
        private DaprClient _daprClient;

        public UsersService(IConfiguration configuration, IDatabase database, DaprClient daprClient) 
        {
            _configuration = configuration;
            _database = database;
            _daprClient = daprClient;
        }

        public override Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, Grpc.Core.ServerCallContext context)
        {
            System.Console.WriteLine("GetUserById:" + request.Id);
            using var mysqlConnection = new MySqlConnection(_configuration.GetConnectionString("UserContext"));
            mysqlConnection.Open();
 
            var query = "SELECT * FROM Users WHERE Id = @Id";
            var param = new { Id = request.Id };
            var result = mysqlConnection.Query<User>(query, param);
            var user = result.FirstOrDefault();
 
            if (user == null)
            {
                return Task.FromException<GetUserByIdResponse>(new ResourceNotFoundException($"User:Id={request.Id} is not found"));
            }

            return Task.FromResult(new GetUserByIdResponse
            {
                User = user,
            });
        }
    }
}