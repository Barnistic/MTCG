using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using MTCG.Models;
using MTCG.Repositories;
using MTCG.Repositories.DTOs;
using NUnit.Framework;
using MTCG.Repositories.Interfaces;

namespace MTCG.Tests.RepositoryTests
{
    [TestFixture]
    public class GameRepositoryTest
    {
        private IGameRepository _gameRepository;
        private string _connectionString = "Host=localhost;Database=mtcg_db;Username=admin;Password=admin;";

        [SetUp]
        public void Setup()
        {
            _gameRepository = new GameRepository(_connectionString);
            CleanDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            CleanDatabase();
        }

        private void CleanDatabase()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("DELETE FROM stats; DELETE FROM users;", connection);
            command.ExecuteNonQuery();
        }

        private void CreateUser(string userId, string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("INSERT INTO users (id, username, password, coins) VALUES (@Id, @Username, 'password', 100)", connection);
            command.Parameters.AddWithValue("@Id", userId);
            command.Parameters.AddWithValue("@Username", username);
            command.ExecuteNonQuery();
        }

        [Test]
        public void CreateStats_ShouldCreateStats()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "TestUser",
                ELO = 1000,
                Win = 10,
                Loss = 5
            };
            CreateUser(user.Id, user.Username);

            // Act
            _gameRepository.CreateStats(user);

            // Assert
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("SELECT user_id, elo, win, loss FROM stats WHERE user_id = @UserId", connection);
            command.Parameters.AddWithValue("@UserId", user.Id);
            using var reader = command.ExecuteReader();
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader["user_id"].ToString(), Is.EqualTo(user.Id));
            Assert.That(Convert.ToInt32(reader["elo"]), Is.EqualTo(user.ELO));
            Assert.That(Convert.ToInt32(reader["win"]), Is.EqualTo(user.Win));
            Assert.That(Convert.ToInt32(reader["loss"]), Is.EqualTo(user.Loss));
        }

        [Test]
        public void UpdateStats_ShouldUpdateStats()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "TestUser",
                ELO = 1000,
                Win = 10,
                Loss = 5,
                Coins = 50
            };
            CreateUser(user.Id, user.Username);
            _gameRepository.CreateStats(user);

            user.ELO = 1100;
            user.Win = 15;
            user.Loss = 6;
            user.Coins = 100;

            // Act
            _gameRepository.UpdateStats(user);

            // Assert
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using (var command = new NpgsqlCommand("SELECT elo, win, loss FROM stats WHERE user_id = @UserId", connection))
            {
                command.Parameters.AddWithValue("@UserId", user.Id);
                using var reader = command.ExecuteReader();
                Assert.That(reader.Read(), Is.True);
                Assert.That(Convert.ToInt32(reader["elo"]), Is.EqualTo(user.ELO));
                Assert.That(Convert.ToInt32(reader["win"]), Is.EqualTo(user.Win));
                Assert.That(Convert.ToInt32(reader["loss"]), Is.EqualTo(user.Loss));
            }

            using (var coinsCommand = new NpgsqlCommand("SELECT coins FROM users WHERE id = @UserId", connection))
            {
                coinsCommand.Parameters.AddWithValue("@UserId", user.Id);
                using var coinsReader = coinsCommand.ExecuteReader();
                Assert.That(coinsReader.Read(), Is.True);
                Assert.That(Convert.ToInt32(coinsReader["coins"]), Is.EqualTo(user.Coins));
            }
        }

        [Test]
        public void GetStat_ShouldReturnStat()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "TestUser",
                ELO = 1000,
                Win = 10,
                Loss = 5
            };
            CreateUser(user.Id, user.Username);
            _gameRepository.CreateStats(user);

            // Act
            var stat = _gameRepository.GetStat(user.Id);

            // Assert
            Assert.That(stat, Is.Not.Null);
            Assert.That(stat.Name, Is.EqualTo(user.Username));
            Assert.That(stat.Elo, Is.EqualTo(user.ELO));
            Assert.That(stat.Win, Is.EqualTo(user.Win));
            Assert.That(stat.Loss, Is.EqualTo(user.Loss));
        }

        [Test]
        public void GetScoreBoard_ShouldReturnScoreBoard()
        {
            // Arrange
            var user1 = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "User1",
                ELO = 1000,
                Win = 10,
                Loss = 5
            };
            var user2 = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "User2",
                ELO = 1100,
                Win = 15,
                Loss = 3
            };
            CreateUser(user1.Id, user1.Username);
            CreateUser(user2.Id, user2.Username);
            _gameRepository.CreateStats(user1);
            _gameRepository.CreateStats(user2);

            // Act
            var scoreBoard = _gameRepository.GetScoreBoard();

            // Assert
            Assert.That(scoreBoard, Is.Not.Null);
            Assert.That(scoreBoard.Count, Is.EqualTo(2));
            Assert.That(scoreBoard[0].Name, Is.EqualTo(user2.Username));
            Assert.That(scoreBoard[0].Elo, Is.EqualTo(user2.ELO));
            Assert.That(scoreBoard[1].Name, Is.EqualTo(user1.Username));
            Assert.That(scoreBoard[1].Elo, Is.EqualTo(user1.ELO));
        }
    }
}



