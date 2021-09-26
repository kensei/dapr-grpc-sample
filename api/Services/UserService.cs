using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;
using DaprSample.Shared.Models;
using DaprSample.Api.Configs.Exceptions;

namespace DaprSample.Api.Services
{
    public class UserService
    {
        private readonly IConfiguration _configuration;
 
        public UserService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }
 
        // Userレコードを1件取得
        public async Task<User> GetUserById(long id)
        {
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
    }
}