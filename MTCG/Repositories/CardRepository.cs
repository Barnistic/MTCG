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
    public class CardRepository : ICardRepository
    {
        private readonly string _connectionString;
        public CardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateCard(Card card)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        INSERT INTO cards (id, name, damage, element_type, ownerid) 
                        VALUES (@Id, @Name, @Damage, @ElementType, @OwnerId)", connection);
                command.Parameters.AddWithValue("@Id", card.Id);
                command.Parameters.AddWithValue("@Name", card.Name);
                command.Parameters.AddWithValue("@Damage", card.Damage);
                command.Parameters.AddWithValue("@ElementType", card.Type);
                command.Parameters.AddWithValue("@OwnerId", DBNull.Value);

                command.ExecuteNonQuery();
                Console.WriteLine($"Created card: {card.Name} with ID: {card.Id}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in CreateCard: {e.Message}");
            }
        }

        public void ChangeCardOwner(string userId, string cardId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        UPDATE cards
                        SET ownerid = @UserId
                        WHERE id = @CardId", connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@CardId", cardId);

                command.ExecuteNonQuery();
                Console.WriteLine($"Changed owner of card {cardId} to user {userId}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in ChangeCardOwner: {e.Message}");
            }
        }

        public List<Card> GetUserStack(string ownerid)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        SELECT id, name, damage, element_type
                        FROM cards 
                        WHERE ownerid = @Ownerid", connection);
                command.Parameters.AddWithValue("@Ownerid", ownerid);

                using var reader = command.ExecuteReader();
                List<Card> cards = new();
                while (reader.Read())
                {
                    var cardDTO = new CardDTO
                    {
                        Id = reader["id"].ToString(),
                        Name = reader["name"].ToString(),
                        Damage = Convert.ToSingle(reader["damage"])
                    };

                    cards.Add(new Card(cardDTO));
                }

                return cards;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in GetUserStack: {e.Message}");
                return null;
            }
        }

        public void UpdateDeck(string userId, List<string> cardIds)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                // Clear existing deck
                using (var command = new NpgsqlCommand("DELETE FROM decks WHERE user_id = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.ExecuteNonQuery();
                }

                // Add new cards to deck
                foreach (var cardId in cardIds)
                {
                    using (var command = new NpgsqlCommand("INSERT INTO decks (user_id, card_id) VALUES (@UserId, @CardId)", connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@CardId", cardId);
                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine($"Updated deck for user {userId}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in UpdateDeck: {e.Message}");
            }
        }

        public Card GetCardById(string cardId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        SELECT id, name, damage, element_type
                        FROM cards
                        WHERE id = @CardId", connection);
                command.Parameters.AddWithValue("@CardId", cardId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var card = new Card
                    (
                        reader["name"].ToString(),
                        (int)reader["damage"],
                        reader["element_type"].ToString()
                    );

                    card.Id = reader["id"].ToString();

                    Console.WriteLine($"Retrieved card: {card.Name} with ID: {card.Id}");
                    return card;
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in GetCardById: {e.Message}");
                return null;
            }
        }
    }
}
