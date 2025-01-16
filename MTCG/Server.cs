using MTCG.Infrastructure;
using MTCG.Models;
using MTCG.Repositories.Interfaces;
using MTCG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTCG
{
    public class Server
    {
        private readonly int _port;
        private readonly Dictionary<string, User> _users; // Dictionary to store User objects
        private readonly RequestHandler _requestHandler;

        public Server(int port, IUserRepository userRepository, ICardRepository cardRepository, IGameRepository gameRepository, ITradingRepository tradingRepository)
        {
            _port = port;
            _users = new Dictionary<string, User>(); // Initialize the dictionary with User objects
            _requestHandler = new RequestHandler(_users, userRepository, cardRepository, gameRepository, tradingRepository);
        }

        public void Start()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, _port);
            Console.WriteLine($"Starting HTTP server on port {_port}...");
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                // Handle each client connection in a separate thread
                ThreadPool.QueueUserWorkItem(HandleClient, client);
            }
        }

        private void HandleClient(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;

            try
            {
                using (var stream = client.GetStream())
                {
                    // Read the incoming request
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Console.WriteLine("\nRequest received");
                    Console.WriteLine(request);

                    // Parse the HTTP request
                    var lines = request.Split("\r\n");
                    _requestHandler.HandleRequest(lines, stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
