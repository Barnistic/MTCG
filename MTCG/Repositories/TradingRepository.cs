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
    public class TradingRepository : ITradingRepository
    {
        private readonly string _connectionString;
        public TradingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddTradingDeal(TradeEntry tradeDeal, string ownerId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        INSERT INTO trades (user_id, id, card_id, reqtype, reqdamage) 
                        VALUES (@User_id, @Id, @Card_id, @ReqType, @ReqDamage)", connection);
                command.Parameters.AddWithValue("@User_id", ownerId);
                command.Parameters.AddWithValue("@Id", tradeDeal.Id);
                command.Parameters.AddWithValue("@Card_id", tradeDeal.CardToTrade);
                command.Parameters.AddWithValue("@ReqType", tradeDeal.Type);
                command.Parameters.AddWithValue("@ReqDamage", tradeDeal.MinimumDamage);

                command.ExecuteNonQuery();
                Console.WriteLine($"Created deal with ID: {tradeDeal.Id}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in AddTradingDeal: {e.Message}");
            }
        }

        public void DeleteTradingDeal(string dealId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        DELETE FROM trades 
                        WHERE id = @Id", connection);
                command.Parameters.AddWithValue("@Id", dealId);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Deleted deal with ID: {dealId}");
                }
                else
                {
                    Console.WriteLine($"No deal found with ID: {dealId}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in DeleteTradingDeal: {e.Message}");
            }
        }

        public List<TradeEntry> GetTradingDeals()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        SELECT user_id, id, card_id, reqType, reqDamage
                        FROM trades", connection);

                using var reader = command.ExecuteReader();
                var tradingDeals = new List<TradeEntry>();
                while (reader.Read())
                {
                    var tradeEntry = new TradeEntry()
                    {
                        Id = reader["id"].ToString(),
                        CardToTrade = reader["card_id"].ToString(),
                        Type = reader["reqType"].ToString(),
                        MinimumDamage = Convert.ToInt32(reader["reqDamage"])
                    };

                    tradingDeals.Add(tradeEntry);
                }

                return tradingDeals;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in GetTradingDeals: {e.Message}");
                return null;
            }
        }

        public string GetTradeOwnerId(string tradeId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(@"
                        SELECT user_id
                        FROM trades
                        WHERE id = @TradeId", connection);
                command.Parameters.AddWithValue("@TradeId", tradeId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string ownerId = reader["user_id"].ToString();
                    Console.WriteLine($"Retrieved owner ID: {ownerId} for trade ID: {tradeId}");
                    return ownerId;
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in GetTradeOwnerId: {e.Message}");
                return null;
            }
        }
    }
}
