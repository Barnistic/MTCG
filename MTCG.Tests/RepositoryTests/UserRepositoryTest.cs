using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using NSubstitute;
using MTCG.Models;
using MTCG.Repositories;
using MTCG.Repositories.DTOs;
using NUnit.Framework;
using MTCG.Repositories.Interfaces;

namespace MTCG.Tests.RepositoryTests
{
    [TestFixture]
    public class UserRepositoryTest
    {
        private IUserRepository _userRepository;
        private string _connectionString = "Host=localhost;Database=mtcg_db;Username=admin;Password=admin;";

        [SetUp]
        public void Setup()
        {
            _userRepository = new UserRepository(_connectionString);
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
            using var command = new NpgsqlCommand("DELETE FROM users; DELETE FROM user_profiles;", connection);
            command.ExecuteNonQuery();
        }

        [Test]
        public void AddUser_ShouldAddUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "NewUser",
                Password = "NewPass",
                Coins = 20,
                ELO = 100
            };

            // Act
            _userRepository.AddUser(user);

            // Assert
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("SELECT id, username, password, coins FROM users WHERE username = @Username", connection);
            command.Parameters.AddWithValue("@Username", user.Username);
            using var reader = command.ExecuteReader();
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader["id"].ToString(), Is.EqualTo(user.Id));
            Assert.That(reader["username"].ToString(), Is.EqualTo(user.Username));
            Assert.That(reader["password"].ToString(), Is.EqualTo(user.Password));
            Assert.That(Convert.ToInt32(reader["coins"]), Is.EqualTo(user.Coins));
        }

        [Test]
        public void GetUser_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "User1",
                Password = "Pass1",
                Coins = 21,
                ELO = 101
            };
            _userRepository.AddUser(user);

            // Act
            var retrievedUser = _userRepository.GetUser(user.Username);

            // Assert
            Assert.That(retrievedUser, Is.Not.Null);
            Assert.That(retrievedUser.Username, Is.EqualTo(user.Username));
            Assert.That(retrievedUser.Password, Is.EqualTo(user.Password));
            Assert.That(retrievedUser.Coins, Is.EqualTo(user.Coins));
            Assert.That(retrievedUser.Profile, Is.Not.Null);
        }

        [Test]
        public void UpdateProfile_ShouldUpdateUserProfile()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "User1",
                Password = "Pass1",
                Coins = 100,
                ELO = 1500
            };
            _userRepository.AddUser(user);

            var userProfile = new UserProfile
            {
                UserId = user.Id,
                Name = "UpdatedName",
                Bio = "UpdatedBio",
                Image = "UpdatedImage"
            };

            // Act
            _userRepository.UpdateProfile(userProfile);

            // Assert
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("SELECT user_id, name, bio, image FROM user_profiles WHERE user_id = @UserId", connection);
            command.Parameters.AddWithValue("@UserId", userProfile.UserId);
            using var reader = command.ExecuteReader();
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader["user_id"].ToString(), Is.EqualTo(userProfile.UserId));
            Assert.That(reader["name"].ToString(), Is.EqualTo(userProfile.Name));
            Assert.That(reader["bio"].ToString(), Is.EqualTo(userProfile.Bio));
            Assert.That(reader["image"].ToString(), Is.EqualTo(userProfile.Image));
        }
    }
}
