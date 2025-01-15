using MTCG.Models;
using MTCG.Repositories.DTOs;
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
        private void HandleUserPost(string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandleUserPost");
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                var user = JsonSerializer.Deserialize<User>(body);
                if (user == null)
                {
                    Console.WriteLine("Deserialized user is null");
                    SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
                    return;
                }
                Console.WriteLine($"Deserialized User: Username={user.Username}, Password={user.Password}");
                if (string.IsNullOrEmpty(user?.Username) || string.IsNullOrEmpty(user?.Password))
                {
                    SendResponse(stream, "400 Bad Request", "Invalid request payload");
                    return;
                }
                if (_users.ContainsKey(user.Username))
                {
                    SendResponse(stream, "409 Conflict", "User with same username already registered");
                    return;
                }
                _users.Add(user.Username, user);
                string token = "Bearer " + user.Username + "-mtcgToken";
                _sessions.Add(token, user.Username);
                _userRepository.AddUser(user);
                SendResponse(stream, "201 Created", "{ \"token\": \"" + token + "\" }");
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
                if (requester.Username == "admin" || requester.Username == username)
                {
                    var response = _userRepository.GetUser(username);
                    string userJson = JsonSerializer.Serialize(response);
                    SendResponse(stream, "200 OK", userJson);
                }
                else
                {
                    SendResponse(stream, "403 Forbidden", "Access denied");
                }
            }
            else
            {
                SendResponse(stream, "404 Not Found", "User not found");
            }
        }

        private void HandleUserPut(string username, User requester, string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandleUserPut");
                if (_users.ContainsKey(username))
                {
                    if (requester.Username == "admin" || requester.Username == username)
                    {
                        string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                        var updatedUser = JsonSerializer.Deserialize<UserProfile>(body);
                        Console.WriteLine("Updated user: " + updatedUser);
                        if (updatedUser == null || string.IsNullOrEmpty(updatedUser.Name))
                        {
                            SendResponse(stream, "400 Bad Request", "Invalid request payload");
                            return;
                        }
                        updatedUser.UserId = requester.Id.ToString();
                        _userRepository.UpdateProfile(updatedUser);
                        SendResponse(stream, "200 OK", "User successfully updated");
                    }
                    else
                    {
                        SendResponse(stream, "403 Forbidden", "Access denied");
                    }
                }
                else
                {
                    SendResponse(stream, "404 Not Found", "User not found");
                }
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
                Console.WriteLine("\nHandleSessionPost");
                var loginDto = JsonSerializer.Deserialize<LoginDTO>(body);
                if (string.IsNullOrEmpty(loginDto?.Username) || string.IsNullOrEmpty(loginDto?.Password))
                {
                    SendResponse(stream, "400 Bad Request", "Invalid request payload");
                    return;
                }
                if (_users.ContainsKey(loginDto.Username) && _users[loginDto.Username].Password == loginDto.Password)
                {
                    string token = $"Bearer {loginDto.Username}-mtcgToken";
                    _sessions.Add(token, loginDto.Username);
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
    }
}
