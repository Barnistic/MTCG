using MTCG.Models;
using MTCG.Repositories.DTOs;
using MTCG.Repositories.Interfaces;
using MTCG.Services;
using MTCG.Services.Interfaces;
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
        private readonly IGameRepository _gameRepository;
        private readonly ITradingRepository _tradingRepository;
        private readonly List<Card> _cardPackages;
        private readonly List<TradeEntry> _tradingDeals;

        private readonly IBattleService _battleService = new BattleService();
        private static readonly Queue<User> _battleLobby = new Queue<User>();

        public RequestHandler(Dictionary<string, User> users, IUserRepository userRepository, ICardRepository cardRepository, IGameRepository gameRepository, ITradingRepository tradingRepository)
        {
            _users = users;
            _sessions = new Dictionary<string, string>();
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _gameRepository = gameRepository;
            _tradingRepository = tradingRepository;
            _cardPackages = new List<Card>();
            _tradingDeals = new List<TradeEntry>();

            //Admin user creation
            UserDTO adminUser = new();
            adminUser.Id = "1";
            adminUser.Name = "admin";
            adminUser.Password = "admin";
            adminUser.Coins = 10000;
            User admin = new(adminUser);
            _users.Add("admin", admin);
            _sessions.Add("Bearer admin-mtcgToken", "admin");
        }

        public void HandleRequest(string[] requestLines, NetworkStream stream)
        {
            try
            {
                if (requestLines.Length < 1)
                {
                    SendResponse(stream, "400 Bad Request", "Invalid request");
                    return;
                }

                string method = requestLines[0].Split(' ')[0];
                string endpoint = requestLines[0].Split(' ')[1];
                string body = string.Empty;
                string token = null;

                // Parse headers
                for (int i = 1; i < requestLines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(requestLines[i]))
                    {
                        // Body starts after the empty line
                        body = string.Join("\r\n", requestLines.Skip(i + 1));
                        break;
                    }

                    if (requestLines[i].StartsWith("Authorization:"))
                    {
                        token = requestLines[i].Substring("Authorization:".Length).Trim();
                    }
                }

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
                else if (endpoint.StartsWith("/deck"))
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
                        HandleTradingDealPost(tradingDealId, requester, requestLines, stream);
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
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (status == null) throw new ArgumentNullException(nameof(status));
            if (body == null) throw new ArgumentNullException(nameof(body));

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
