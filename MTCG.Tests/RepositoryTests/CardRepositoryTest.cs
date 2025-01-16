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
    public class CardRepositoryTest
    {
        private ICardRepository _cardRepository;
        private string _connectionString = "Host=localhost;Database=mtcg_db;Username=admin;Password=admin;";

        [SetUp]
        public void Setup()
        {
            _cardRepository = new CardRepository(_connectionString);
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
            using var command = new NpgsqlCommand("DELETE FROM cards; DELETE FROM decks; DELETE FROM users;", connection);
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
        public void CreateCard_ShouldCreateCard()
        {
            // Arrange
            var card = new Card("TestCard", 50, "Fire");

            // Act
            _cardRepository.CreateCard(card);

            // Assert
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("SELECT id, name, damage, element_type FROM cards WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", card.Id);
            using var reader = command.ExecuteReader();
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader["id"].ToString(), Is.EqualTo(card.Id));
            Assert.That(reader["name"].ToString(), Is.EqualTo(card.Name));
            Assert.That(Convert.ToInt32(reader["damage"]), Is.EqualTo(card.Damage));
            Assert.That(reader["element_type"].ToString(), Is.EqualTo(card.Type));
        }

        [Test]
        public void ChangeCardOwner_ShouldChangeCardOwner()
        {
            // Arrange
            var card = new Card("TestCard", 50, "Fire");
            _cardRepository.CreateCard(card);
            var userId = Guid.NewGuid().ToString();
            CreateUser(userId, "TestUser");

            // Act
            _cardRepository.ChangeCardOwner(userId, card.Id);

            // Assert
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("SELECT ownerid FROM cards WHERE id = @Id", connection);
            command.Parameters.AddWithValue("@Id", card.Id);
            using var reader = command.ExecuteReader();
            Assert.That(reader.Read(), Is.True);
            Assert.That(reader["ownerid"].ToString(), Is.EqualTo(userId));
        }

        [Test]
        public void GetUserStack_ShouldReturnUserStack()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            CreateUser(userId, "TestUser");
            var card1 = new Card("TestCard1", 50, "Fire");
            var card2 = new Card("TestCard2", 60, "Water");
            _cardRepository.CreateCard(card1);
            _cardRepository.CreateCard(card2);
            _cardRepository.ChangeCardOwner(userId, card1.Id);
            _cardRepository.ChangeCardOwner(userId, card2.Id);

            // Act
            var userStack = _cardRepository.GetUserStack(userId);

            // Assert
            Assert.That(userStack, Is.Not.Null);
            Assert.That(userStack.Count, Is.EqualTo(2));
            Assert.That(userStack.Any(c => c.Id == card1.Id), Is.True);
            Assert.That(userStack.Any(c => c.Id == card2.Id), Is.True);
        }

        [Test]
        public void UpdateDeck_ShouldUpdateDeck()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            CreateUser(userId, "TestUser");
            var card1 = new Card("TestCard1", 50, "Fire");
            var card2 = new Card("TestCard2", 60, "Water");
            _cardRepository.CreateCard(card1);
            _cardRepository.CreateCard(card2);
            _cardRepository.ChangeCardOwner(userId, card1.Id);
            _cardRepository.ChangeCardOwner(userId, card2.Id);
            var cardIds = new List<string> { card1.Id, card2.Id };

            // Act
            _cardRepository.UpdateDeck(userId, cardIds);

            // Assert
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand("SELECT card_id FROM decks WHERE user_id = @UserId", connection);
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = command.ExecuteReader();
            var deckCardIds = new List<string>();
            while (reader.Read())
            {
                deckCardIds.Add(reader["card_id"].ToString());
            }
            Assert.That(deckCardIds.Count, Is.EqualTo(2));
            Assert.That(deckCardIds.Contains(card1.Id), Is.True);
            Assert.That(deckCardIds.Contains(card2.Id), Is.True);
        }

        [Test]
        public void GetCardById_ShouldReturnCard()
        {
            // Arrange
            var card = new Card("TestCard", 50, "Fire");
            _cardRepository.CreateCard(card);

            // Act
            var retrievedCard = _cardRepository.GetCardById(card.Id);

            // Assert
            Assert.That(retrievedCard, Is.Not.Null);
            Assert.That(retrievedCard.Id, Is.EqualTo(card.Id));
            Assert.That(retrievedCard.Name, Is.EqualTo(card.Name));
            Assert.That(retrievedCard.Damage, Is.EqualTo(card.Damage));
            Assert.That(retrievedCard.Type, Is.EqualTo(card.Type));
        }
    }
}


