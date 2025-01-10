using MTCG.Models;
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
    public class RequestHandler
    {
        private readonly Dictionary<string, User> _users;
        private readonly Dictionary<string, string> _sessions; // token and username
        private readonly IUserRepository _userRepository;

        public RequestHandler(Dictionary<string, User> users, IUserRepository userRepository)
        {
            _users = users;
            _sessions = new Dictionary<string, string>();
            _userRepository = userRepository;
        }

        public void HandleRequest(string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandling request");
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                string method = requestLines[0].Split(' ')[0];
                string endpoint = requestLines[0].Split(' ')[1];

                Console.WriteLine($"\nMethod: {method}, Endpoint: {endpoint}");

                if (endpoint.StartsWith("/users"))
                {
                    if (method == "POST")
                    {
                        HandleUserPost(requestLines, stream);
                    }
                    else if (method == "GET")
                    {
                        string token = requestLines[1].Split(' ')[1];
                        var requester = ValidateToken(token);
                        if (requester == null)
                        {
                            SendResponse(stream, "401 Unathourized", "Invalid token");
                            return;
                        }
                        string username = endpoint.Split('/')[2];
                        HandleUserGet(username, requester, stream);
                    }
                }
                else if (endpoint == "/sessions")
                {
                    if (method == "POST")
                    {
                        HandleSessionPost(requestLines, stream);
                    }
                }
                else
                {
                    SendResponse(stream, "404 Not Found", "Endpoint not found");
                }
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload2");
            }
        }

        private void HandleUserPost(string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandleUserPost\n");
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                // Deserialize directly into a User object
                var user = JsonSerializer.Deserialize<User>(body);
                // Debugging: Check if user object is null
                if (user == null)
                {
                    Console.WriteLine("Deserialized user is null");
                    SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
                    return;
                }
                // Debugging: Check deserialized values
                Console.WriteLine($"Deserialized User: Username={user.Username}, Password={user.Password}");
                // Validate payload
                if (string.IsNullOrEmpty(user?.Username) || string.IsNullOrEmpty(user?.Password))
                {
                    SendResponse(stream, "400 Bad Request", "Invalid request payload");
                    return;
                }
                // Check if user already exists
                if (_users.ContainsKey(user.Username))
                {
                    SendResponse(stream, "400 Bad Request", "User already exists");
                    return;
                }
                // Register the user
                _users.Add(user.Username, user);
                string token = "Bearer " + user.Username + "-token";
                _sessions.Add(token, user.Username);
                _userRepository.AddUser(user);
                SendResponse(stream, "201 Created", "User created successfully, token: " + token);
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
        }

        private void HandleUserGet(string username, User requester, NetworkStream stream)
        {
            Console.WriteLine("\nHandleUserGet");
            if (_users.ContainsKey(username))
            {
                var user = _users[username];
                //Should pass the admin ID as a variable
                if (requester.Username == "admin" || requester.Username == username)
                {
                    string userJson = JsonSerializer.Serialize(user);
                    SendResponse(stream, "200 OK", userJson);
                }
                else
                {
                    SendResponse(stream, "403 Forbidden", "Acces denied");
                }
            }
            else
            {
                SendResponse(stream, "404 Not found", "User not found");
            }
        }

        private void HandleSessionPost(string[] requestLines, NetworkStream stream)
        {
            string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
            Console.WriteLine("Request body:");
            Console.WriteLine(body);
            try
            {
                Console.WriteLine("HandleSessionPost");
                // Deserialize directly into a User object
                var user = JsonSerializer.Deserialize<User>(body);
                // Validate payload
                if (string.IsNullOrEmpty(user?.Username) || string.IsNullOrEmpty(user?.Password))
                {
                    SendResponse(stream, "400 Bad Request", "Invalid request payload");
                    return;
                }
                // Check if the user exists and credentials are correct
                if (_users.ContainsKey(user.Username) && _users[user.Username].Password == user.Password)
                {
                    // Generate token
                    string token = $"{user.Username}-token"; // Example token format
                    // Return the token as the response
                    SendResponse(stream, "200 OK", token);
                }
                else
                {
                    SendResponse(stream, "401 Unauthorized", "Invalid credentials");
                }
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
        }

        private User ValidateToken(string token)
        {
            Console.WriteLine("Validating token");
            if (_sessions.ContainsKey(token))
            {
                string username = _sessions[token];
                Console.WriteLine("Token username: " + username);
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
