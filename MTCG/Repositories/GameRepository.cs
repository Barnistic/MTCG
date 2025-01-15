using MTCG.Models;
using MTCG.Repositories.DTOs;
using MTCG.Repositories.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly string _connectionString;
        public GameRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateStats(User user)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        INSERT INTO stats (user_id, elo, win, loss) 
                        VALUES (@User_id, @Elo, @Win, @Loss)", connection);
                command.Parameters.AddWithValue("@User_id", user.Id);
                command.Parameters.AddWithValue("@Elo", user.ELO);
                command.Parameters.AddWithValue("@Win", user.Win);
                command.Parameters.AddWithValue("@Loss", user.Loss);

                command.ExecuteNonQuery();
                Console.WriteLine($"Added stats for user: {user.Username}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in CreateStats: {e.Message}");
            }
        }

        public void UpdateStats(User user)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        UPDATE stats
                        SET elo = @Elo, win = @Win, loss = @Loss
                        WHERE user_id = @User_id", connection);
                command.Parameters.AddWithValue("@User_id", user.Id);
                command.Parameters.AddWithValue("@Elo", user.ELO);
                command.Parameters.AddWithValue("@Win", user.Win);
                command.Parameters.AddWithValue("@Loss", user.Loss);

                command.ExecuteNonQuery();

                // Update coins in the users table
                using var updateCoinsCommand = new NpgsqlCommand(@"
                        UPDATE users
                        SET coins = @Coins
                        WHERE id = @User_id", connection);
                updateCoinsCommand.Parameters.AddWithValue("@User_id", user.Id);
                updateCoinsCommand.Parameters.AddWithValue("@Coins", user.Coins);

                updateCoinsCommand.ExecuteNonQuery();

                Console.WriteLine($"Updated stats and coins for user: {user.Username}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in UpdateStats: {e.Message}");
            }
        }

        public StatDTO GetStat(string userId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        SELECT u.username, s.elo, s.win, s.loss
                        FROM stats s
                        JOIN users u ON s.user_id = u.id
                        WHERE user_id = @User_id", connection);
                command.Parameters.AddWithValue("@User_id", userId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var statDto = new StatDTO
                    {
                        Name = reader["username"].ToString(),
                        Elo = Convert.ToInt32(reader["elo"]),
                        Win = Convert.ToInt32(reader["win"]),
                        Loss = Convert.ToInt32(reader["loss"])
                    };

                    return statDto;
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in GetStat: {e.Message}");
                return null;
            }
        }

        public List<StatDTO> GetScoreBoard()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        SELECT u.username, s.elo, s.win, s.loss
                        FROM stats s
                        JOIN users u ON s.user_id = u.id
                        ORDER BY s.elo DESC", connection);

                using var reader = command.ExecuteReader();
                var scoreBoard = new List<StatDTO>();
                while (reader.Read())
                {
                    var statDto = new StatDTO
                    {
                        Name = reader["username"].ToString(),
                        Elo = Convert.ToInt32(reader["elo"]),
                        Win = Convert.ToInt32(reader["win"]),
                        Loss = Convert.ToInt32(reader["loss"])
                    };

                    scoreBoard.Add(statDto);
                }

                return scoreBoard;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in GetScoreBoard: {e.Message}");
                return null;
            }
        }
    }
}
