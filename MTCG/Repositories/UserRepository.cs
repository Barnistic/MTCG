using MTCG.Models;
using MTCG.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using MTCG.Repositories.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MTCG.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetUser(string username)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        SELECT id, username, password, coins, elo 
                        FROM users 
                        WHERE username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var userDto = new UserDTO
                    {
                        id = reader["id"].ToString(),
                        name = reader["username"].ToString(),
                        password = reader["password"].ToString(),
                        coins = Convert.ToInt32(reader["coins"]),
                        elo = Convert.ToInt32(reader["elo"])
                    };

                    return new User(userDto);
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in GetUser: {e.Message}");
                return null;
            }
        }

        public List<User> GetAllUsers()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        SELECT id, username, password, coins, elo 
                        FROM users 
                        ORDER BY elo DESC", connection);

                using var reader = command.ExecuteReader();
                var users = new List<User>();
                while (reader.Read())
                {
                    var userDto = new UserDTO
                    {
                        id = reader["id"].ToString(),
                        name = reader["username"].ToString(),
                        password = reader["password"].ToString(),
                        coins = Convert.ToInt32(reader["coins"]),
                        elo = Convert.ToInt32(reader["elo"])
                    };

                    users.Add(new User(userDto));
                }

                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in GetAllUsers: {e.Message}");
                return null;
            }
        }

        public void AddUser(User user)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        INSERT INTO users (id, username, password, coins, elo) 
                        VALUES (@Id, @Username, @Password, @Coins, @Elo)", connection);
                command.Parameters.AddWithValue("@Id", user.Id.ToString());
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Coins", user.Coins);
                command.Parameters.AddWithValue("@Elo", user.ELO);

                command.ExecuteNonQuery();
                Console.WriteLine($"Added user: {user.Username}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in AddUser: {e.Message}");
            }
        }

        public void UpdateProfile(UserProfile userProfile)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        INSERT INTO user_profiles (user_id, name, bio, image)
                        VALUES (@UserId, @Name, @Bio, @Image)
                        ON CONFLICT (user_id) 
                        DO UPDATE SET 
                            name = @Name,
                            bio = @Bio,
                            image = @Image", connection);
                command.Parameters.AddWithValue("@UserId", userProfile.UserId);
                command.Parameters.AddWithValue("@Name", userProfile.Name);
                command.Parameters.AddWithValue("@Bio", userProfile.Bio);
                command.Parameters.AddWithValue("@Image", userProfile.Image);

                command.ExecuteNonQuery();
                Console.WriteLine($"Updated profile for UserId: {userProfile.UserId}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in UpdateProfile: {e.Message}");
            }
        }
    }
}
