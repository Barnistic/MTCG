using System;
using System.Collections.Generic;
using MTCG.Models;
using MTCG.Services;
using NUnit.Framework;

namespace MTCG.Tests.ServiceTests
{
    [TestFixture]
    public class BattleServiceTest
    {
        private BattleService _battleService;

        [SetUp]
        public void Setup()
        {
            _battleService = new BattleService();
        }

        [Test]
        public void CalculateDamage_ShouldReturnCorrectDamage()
        {
            // Arrange
            var attacker = new Card("WaterCard", 50, "Water");
            var defender = new Card("FireCard", 50, "Fire");

            // Act
            var damage = _battleService.CalculateDamage(attacker, defender);

            // Assert
            Assert.That(damage, Is.EqualTo(100));
        }

        [Test]
        public void Battle_ShouldLogBattle()
        {
            // Arrange
            var user1 = new User
            {
                Username = "Player1",
                Deck = new List<Card>
                {
                    new Card("WaterCard", 50, "Water"),
                    new Card("FireCard", 50, "Fire")
                }
            };

            var user2 = new User
            {
                Username = "Player2",
                Deck = new List<Card>
                {
                    new Card("RegularCard", 50, "Regular"),
                    new Card("WaterCard", 50, "Water")
                }
            };

            // Act
            _battleService.Battle(user1, user2);
            var battleLog = _battleService.GetBattleLog();

            // Assert
            Assert.That(battleLog, Does.Contain("The battle begins!"));
            Assert.That(battleLog, Does.Contain("Round 1:"));
            Assert.That(battleLog, Does.Contain("Player1"));
            Assert.That(battleLog, Does.Contain("Player2"));
        }

        [Test]
        public void Battle_ShouldUpdateUserStats()
        {
            // Arrange
            var user1 = new User
            {
                Username = "Player1",
                Deck = new List<Card>
                {
                    new Card("WaterCard", 50, "Water"),
                    new Card("FireCard", 50, "Fire")
                }
            };

            var user2 = new User
            {
                Username = "Player2",
                Deck = new List<Card>
                {
                    new Card("RegularCard", 50, "Regular"),
                    new Card("WaterCard", 50, "Water")
                }
            };

            // Act
            _battleService.Battle(user1, user2);

            // Assert
            Assert.That(user1.Win + user1.Loss, Is.EqualTo(1));
            Assert.That(user2.Win + user2.Loss, Is.EqualTo(1));
        }

        [Test]
        public void Battle_ShouldHandleTie()
        {
            // Arrange
            var user1 = new User
            {
                Username = "Player1",
                Deck = new List<Card>
                {
                    new Card("WaterCard", 50, "Water")
                }
            };

            var user2 = new User
            {
                Username = "Player2",
                Deck = new List<Card>
                {
                    new Card("WaterCard", 50, "Water")
                }
            };

            // Act
            _battleService.Battle(user1, user2);

            // Assert
            Assert.That(user1.ELO, Is.EqualTo(101));
            Assert.That(user2.ELO, Is.EqualTo(101));
        }
    }
}






