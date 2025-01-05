using MTCG.Controllers;
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

                Console.WriteLine($"Deserialized user: Username={user.Username}, Password={user.Password}");

                if (string.IsNullOrEmpty(user?.Username) || string.IsNullOrEmpty(user?.Password))
                {
                    SendResponse(stream, "400 Bad Request", "Invalid request payload");
                    return;
                }

                string endpoint = requestLines[0].Split(' ')[1];

                HttpResponse? response = null;

                UserController userController = new(_users);

                if (endpoint == "/users")
                {
                    response = userController.HandleUserPost(user, stream);
                }
                else if (endpoint == "/sessions")
                {
                    response = userController.HandleSessionPost(user, stream);
                }
                else
                {
                    response = new HttpResponse("400 Bad Request", "Endpoint not found");
                }


                if (response == null)
                {
                    SendResponse(stream, "500", "An unexpected error has occured");
                }
                else
                {
                    SendResponse(stream, response._status, response._body);
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
