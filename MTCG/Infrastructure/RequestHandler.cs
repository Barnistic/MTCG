using MTCG.Models;
using MTCG.Repositories.DTOs;
using MTCG.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTCG.Infrastructure
{
    public partial class RequestHandler
    {
        private readonly Dictionary<string, User> _users;
        private readonly Dictionary<string, string> _sessions; // token and username
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly List<Card> _cardPackages;
        private readonly List<TradeEntry> _tradingDeals;

        public RequestHandler(Dictionary<string, User> users, IUserRepository userRepository, ICardRepository cardRepository)
        {
            _users = users;
            _sessions = new Dictionary<string, string>();
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _cardPackages = new List<Card>();
            _tradingDeals = new List<TradeEntry>();

            //Admin user creation
            UserDTO adminUser = new();
            adminUser.id = "1";
            adminUser.name = "admin";
            adminUser.password = "admin";
            adminUser.coins = 10000;
            adminUser.elo = 0;
            User admin = new(adminUser);
            _users.Add("admin", admin);
            _sessions.Add("Bearer admin-mtcgToken", "admin");
        }

        public void HandleRequest(string[] requestLines, NetworkStream stream)
        {
            try
            {
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                string method = requestLines[0].Split(' ')[0];
                string endpoint = requestLines[0].Split(' ')[1];

                Console.WriteLine($"Method: {method}, Endpoint: {endpoint}");
                //Console.WriteLine("Body:");
                //Console.WriteLine(body);

                // Skip token validation for login and registration
                if (endpoint == "/sessions" && method == "POST")
                {
                    HandleSessionPost(requestLines, stream);
                    return;
                }
                if (endpoint.StartsWith("/users") && method == "POST")
                {
                    HandleUserPost(requestLines, stream);
                    return;
                }

                // Validate token for other endpoints
                string token = requestLines[1].Split(' ')[1] + " " + requestLines[1].Split(' ')[2];
                //Console.WriteLine(token);
                var requester = ValidateToken(token);
                if (requester == null)
                {
                    SendResponse(stream, "401 Unauthorized", "Invalid token");
                    return;
                }

                if (endpoint.StartsWith("/users"))
                {
                    if (method == "GET")
                    {
                        string username = endpoint.Split('/')[2];
                        HandleUserGet(username, requester, stream);
                    }
                    else if (method == "PUT")
                    {
                        string username = endpoint.Split('/')[2];
                        HandleUserPut(username, requester, requestLines, stream);
                    }
                }
                else if (endpoint == "/packages")
                {
                    if (method == "POST")
                    {
                        HandlePackagePost(requester, requestLines, stream);
                    }
                }
                else if (endpoint == "/transactions/packages")
                {
                    if (method == "POST")
                    {
                        HandleTransactionPost(requester, stream);
                    }
                }
                else if (endpoint == "/cards")
                {
                    if (method == "GET")
                    {
                        HandleCardsGet(requester, stream);
                    }
                }
                else if (endpoint == "/deck")
                {
                    if (method == "GET")
                    {
                        HandleDeckGet(requester, requestLines, stream);
                    }
                    else if (method == "PUT")
                    {
                        HandleDeckPut(requester, requestLines, stream);
                    }
                }
                else if (endpoint == "/stats")
                {
                    if (method == "GET")
                    {
                        HandleStatsGet(requester, stream);
                    }
                }
                else if (endpoint == "/scoreboard")
                {
                    if (method == "GET")
                    {
                        HandleScoreboardGet(stream);
                    }
                }
                else if (endpoint == "/battles")
                {
                    if (method == "POST")
                    {
                        HandleBattlePost(requester, stream);
                    }
                }
                else if (endpoint == "/tradings")
                {
                    if (method == "GET")
                    {
                        HandleTradingsGet(stream);
                    }
                    else if (method == "POST")
                    {
                        HandleTradingPost(requester, requestLines, stream);
                    }
                }
                else if (endpoint.StartsWith("/tradings/"))
                {
                    string tradingDealId = endpoint.Split('/')[2];
                    if (method == "DELETE")
                    {
                        HandleTradingDelete(tradingDealId, requester, stream);
                    }
                    else if (method == "POST")
                    {
                        //HandleTradingDealPost(tradingDealId, requester, requestLines, stream);
                    }
                }
                else
                {
                    SendResponse(stream, "404 Not Found", "Endpoint not found");
                }
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine("JSON Exception: " + jsonEx.Message);
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                SendResponse(stream, "500 Internal Server Error", "An error occurred while processing the request");
            }
        }

        private void HandleStatsGet(User requester, NetworkStream stream)
        {
            Console.WriteLine("\nHandleStatsGet");
            string statsJson = JsonSerializer.Serialize(new { requester.Username, requester.ELO, requester.Coins });
            SendResponse(stream, "200 OK", statsJson);
        }

        private void HandleScoreboardGet(NetworkStream stream)
        {
            Console.WriteLine("\nHandleScoreboardGet");
            var scoreboard = _users.Values.OrderByDescending(u => u.ELO).Select(u => new { u.Username, u.ELO, u.Coins }).ToList();
            string scoreboardJson = JsonSerializer.Serialize(scoreboard);
            SendResponse(stream, "200 OK", scoreboardJson);
        }


        private void HandleBattlePost(User requester, NetworkStream stream)
        {
            Console.WriteLine("\nHandleBattlePost");
            // Implement battle logic here
            SendResponse(stream, "200 OK", "The battle has been carried out successfully");
        }

        private void HandleTradingsGet(NetworkStream stream)
        {
            Console.WriteLine("\nHandleTradingsGet");
            if (_tradingDeals.Any())
            {
                string dealsJson = JsonSerializer.Serialize(_tradingDeals);
                SendResponse(stream, "200 OK", dealsJson);
            }
            else
            {
                SendResponse(stream, "204 No Content", "There are no trading deals available");
            }
        }

        private void HandleTradingPost(User requester, string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandleTradingPost\n");
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                var tradingDeal = JsonSerializer.Deserialize<TradeEntry>(body);
                if (tradingDeal == null || tradingDeal.card == null || string.IsNullOrEmpty(tradingDeal.card.Id.ToString()))
                {
                    SendResponse(stream, "400 Bad Request", "Invalid request payload");
                    return;
                }
                if (_tradingDeals.Any(d => d.card.Id == tradingDeal.card.Id))
                {
                    SendResponse(stream, "409 Conflict", "A deal with this card ID already exists");
                    return;
                }
                if (!requester.Stack.Any(c => c.Id == tradingDeal.card.Id) || requester.Deck.Any(c => c.Id == tradingDeal.card.Id))
                {
                    SendResponse(stream, "403 Forbidden", "The deal contains a card that is not owned by the user or locked in the deck");
                    return;
                }
                _tradingDeals.Add(tradingDeal);
                SendResponse(stream, "201 Created", "Trading deal successfully created");
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
        }

        private void HandleTradingDelete(string tradingDealId, User requester, NetworkStream stream)
        {
            Console.WriteLine("\nHandleTradingDelete");
            var deal = _tradingDeals.FirstOrDefault(d => d.card.Id.ToString() == tradingDealId);
            if (deal == null)
            {
                SendResponse(stream, "404 Not Found", "The provided deal ID was not found");
                return;
            }
            if (deal.owner.Username != requester.Username)
            {
                SendResponse(stream, "403 Forbidden", "The deal contains a card that is not owned by the user");
                return;
            }
            _tradingDeals.Remove(deal);
            SendResponse(stream, "200 OK", "Trading deal successfully deleted");
        }

        /*private void HandleTradingDealPost(string tradingDealId, User requester, string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandleTradingDealPost\n");
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                var offeredCardId = JsonSerializer.Deserialize<Guid>(body);
                var deal = _tradingDeals.FirstOrDefault(d => d.card.Id.ToString() == tradingDealId);
                if (deal == null)
                {
                    SendResponse(stream, "404 Not Found", "The provided deal ID was not found");
                    return;
                }
                if (requester.Username == deal.owner.Username || !requester.Stack.Any(c => c.Id == offeredCardId) || requester.Deck.Any(c => c.Id == offeredCardId))
                {
                    SendResponse(stream, "403 Forbidden", "The offered card is not owned by the user, or the requirements are not met, or the offered card is locked in the deck, or the user tries to trade with self");
                    return;
                }
                // Implement trade logic here
                SendResponse(stream, "200 OK", "Trading deal successfully executed");
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
        }*/



        private User ValidateToken(string token)
        {
            if (_sessions.ContainsKey(token))
            {
                string username = _sessions[token];
                //Console.WriteLine("Token username: " + username);
                if (_users.ContainsKey(username))
                {
                    return _users[username];
                }
            }
            return null;
        }

        private void SendResponse(NetworkStream stream, string status, string body)
        {
            string response = $"HTTP/1.1 {status}\r\n" +
                              "Content-Type: text/plain\r\n" +
                              $"Content-Length: {body.Length}\r\n" +
                              "\r\n" +
                              body;

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }
    }
}
