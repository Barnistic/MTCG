using MTCG.Models;
using MTCG.Services;
using MTCG.Interfaces;
using MTCG.Repositories.Interfaces;
using Npgsql.Internal;
using MTCG.Repositories;

namespace MTCG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Initialize services
            IUserService _userService = new UserService();
            ITradingService _tradingService = new TradingService();
            ICardService _cardService = new CardService();

            string connectionString = "Host=localhost;Database=mtcg_db;Username=admin;Password=admin;";

            IUserRepository userRepository = new UserRepository(connectionString);


            //Initialize game
            GameService Game = new(_userService, _cardService, _tradingService);

            //Initialize server
            Server server = new(10001, userRepository);
            server.Start();
            //Game.StartGame();

        }
    }
}
