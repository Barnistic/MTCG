using MTCG.Models;
using MTCG.Services;
using MTCG.Repositories.Interfaces;
using Npgsql.Internal;
using MTCG.Repositories;
using MTCG.Infrastructure;
using MTCG.Services.Interfaces;

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

            DatabaseInitializer databaseInitializer = new(connectionString);
            databaseInitializer.InitializeDatabase();

            IUserRepository userRepository = new UserRepository(connectionString);
            ICardRepository cardRepository = new CardRepository(connectionString);
            IGameRepository gameRepository = new GameRepository(connectionString);


            //Initialize game
            GameService Game = new(_userService, _cardService, _tradingService);

            //Initialize server
            Server server = new(10001, userRepository, cardRepository, gameRepository);
            server.Start();
            //Game.StartGame();

        }
    }
}
