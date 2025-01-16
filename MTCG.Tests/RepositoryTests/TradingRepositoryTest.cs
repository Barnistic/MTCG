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
    public class TradingRepositoryTest
    {
        private ITradingRepository _tradingRepository;
        private string _connectionString = "Host=localhost;Database=mtcg_db;Username=admin;Password=admin;";

        [SetUp]
        public void Setup()
        {
            _tradingRepository = new TradingRepository(_connectionString);
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
            using var command = new NpgsqlCommand("DELETE FROM trades; DELETE FROM users; DELETE FROM cards;", connection);
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

        private void CreateCard(string cardId, string cardName, string cardType, int cardDamage)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("INSERT INTO cards (id, name, damage, element_type) VALUES (@Id, @Name, @Damage, @ElementType)", connection);
            command.Parameters.AddWithValue("@Id", cardId);
            command.Parameters.AddWithValue("@Name", cardName);
            command.Parameters.AddWithValue("@Damage", cardDamage);
            command.Parameters.AddWithValue("@ElementType", cardType);
            command.ExecuteNonQuery();
        }

        [Test]
        public void AddTradingDeal_ShouldAddTradingDeal()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            CreateUser(userId, "TestUser");
            var cardId = Guid.NewGuid().ToString();
            CreateCard(cardId, "TestCard", "Fire", 50);
            var tradeDeal = new TradeEntry
            {
                Id = Guid.NewGuid().ToString(),
                CardToTrade = cardId,
                Type = "Fire",
                MinimumDamage = 50
            };

            // Act
            _tradingRepository.AddTradingDeal(tradeDeal, userId);

            // Assert
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("SELECT user_id, id, card_id, reqtype, reqdamage FROM trades WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", tradeDeal.Id);
            using var reader = command.ExecuteReader();
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader["user_id"].ToString(), Is.EqualTo(userId));
            Assert.That(reader["id"].ToString(), Is.EqualTo(tradeDeal.Id));
            Assert.That(reader["card_id"].ToString(), Is.EqualTo(tradeDeal.CardToTrade));
            Assert.That(reader["reqtype"].ToString(), Is.EqualTo(tradeDeal.Type));
            Assert.That(Convert.ToInt32(reader["reqdamage"]), Is.EqualTo(tradeDeal.MinimumDamage));
        }

        [Test]
        public void DeleteTradingDeal_ShouldDeleteTradingDeal()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            CreateUser(userId, "TestUser");
            var cardId = Guid.NewGuid().ToString();
            CreateCard(cardId, "TestCard", "Fire", 50);
            var tradeDeal = new TradeEntry
            {
                Id = Guid.NewGuid().ToString(),
                CardToTrade = cardId,
                Type = "Fire",
                MinimumDamage = 50
            };
            _tradingRepository.AddTradingDeal(tradeDeal, userId);

            // Act
            _tradingRepository.DeleteTradingDeal(tradeDeal.Id);

            // Assert
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("SELECT id FROM trades WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", tradeDeal.Id);
            using var reader = command.ExecuteReader();
            Assert.That(reader.Read(), Is.False); //If reader is "false" there are no more rows
        }

        [Test]
        public void GetTradingDeals_ShouldReturnTradingDeals()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            CreateUser(userId, "TestUser");
            var cardId1 = Guid.NewGuid().ToString();
            var cardId2 = Guid.NewGuid().ToString();
            CreateCard(cardId1, "TestCard1", "Fire", 50);
            CreateCard(cardId2, "TestCard2", "Water", 60);
            var tradeDeal1 = new TradeEntry
            {
                Id = Guid.NewGuid().ToString(),
                CardToTrade = cardId1,
                Type = "Fire",
                MinimumDamage = 50
            };
            var tradeDeal2 = new TradeEntry
            {
                Id = Guid.NewGuid().ToString(),
                CardToTrade = cardId2,
                Type = "Water",
                MinimumDamage = 60
            };
            _tradingRepository.AddTradingDeal(tradeDeal1, userId);
            _tradingRepository.AddTradingDeal(tradeDeal2, userId);

            // Act
            var tradingDeals = _tradingRepository.GetTradingDeals();

            // Assert
            Assert.That(tradingDeals, Is.Not.Null);
            Assert.That(tradingDeals.Count, Is.EqualTo(2));
            Assert.That(tradingDeals.Any(td => td.Id == tradeDeal1.Id), Is.True);
            Assert.That(tradingDeals.Any(td => td.Id == tradeDeal2.Id), Is.True);
        }

        [Test]
        public void GetTradeOwnerId_ShouldReturnTradeOwnerId()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            CreateUser(userId, "TestUser");
            var cardId = Guid.NewGuid().ToString();
            CreateCard(cardId, "TestCard", "Fire", 50);
            var tradeDeal = new TradeEntry
            {
                Id = Guid.NewGuid().ToString(),
                CardToTrade = cardId,
                Type = "Fire",
                MinimumDamage = 50
            };
            _tradingRepository.AddTradingDeal(tradeDeal, userId);

            // Act
            var ownerId = _tradingRepository.GetTradeOwnerId(tradeDeal.Id);

            // Assert
            Assert.That(ownerId, Is.EqualTo(userId));
        }
    }
}




