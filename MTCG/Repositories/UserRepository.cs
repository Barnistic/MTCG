using MTCG.Models;
using MTCG.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Dapper;
using MTCG.Repositories.DTOs;

namespace MTCG.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<User> GetUsers()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                var users = connection.Query<User>(@"
                    SELECT id, name, password, coins, elo 
                    FROM users 
                    ORDER BY elo DESC").ToList();

                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in GetUsers: {e.Message}");
                return null;
            }
        }

        public void AddUser(User user)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Execute(@"
                    INSERT INTO users (id, name, password, coins, elo) 
                    VALUES (@Id, @Username, @Password, @Coins, @Elo)",
                    new { Id = user.Id.ToString(), user.Username, user.Password, user.Coins, user.ELO});

                Console.WriteLine($"Added user: {user.Username}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in AddUser: {e.Message}");
            }
        }
    }
}
