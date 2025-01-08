using MTCG.Models;
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

        public RequestHandler(Dictionary<string, User> users)
        {
            _users = users;
        }

        public void HandleRequest(string[] requestLines, NetworkStream stream)
        {
            try
            {
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                Console.WriteLine("Request body: ");
                Console.WriteLine(body);

                var user = JsonSerializer.Deserialize<User>(body);

                if (user == null)
                {
                    Console.WriteLine("Deserialized user is null");
                    SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
                    return;
                }

                //Console.WriteLine($"Deserialized user: Username={user.Username}, Password={user.Password}");

                if (string.IsNullOrEmpty(user?.Username) || string.IsNullOrEmpty(user?.Password))
                {
                    SendResponse(stream, "400 Bad Request", "Invalid request payload");
                    return;
                }

                string method = requestLines[0].Split(' ')[0];
                string endpoint = requestLines[0].Split(' ')[1];

                if (endpoint == "/users")
                {
                    if (method == "POST")
                    {
                        HandleUserPost(requestLines, stream);
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
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
        }

        private void HandleUserPost(string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("HandleUserPost");
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                Console.WriteLine("Request body:");
                Console.WriteLine(body);
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
                SendResponse(stream, "201 Created", "User created successfully");
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
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
