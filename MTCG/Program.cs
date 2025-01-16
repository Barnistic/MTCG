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
            //Initialize database
            string connectionString = "Host=localhost;Database=mtcg_db;Username=admin;Password=admin;";
            DatabaseInitializer databaseInitializer = new(connectionString);
            databaseInitializer.InitializeDatabase();

            //Initialize repos
            IUserRepository userRepository = new UserRepository(connectionString);
            ICardRepository cardRepository = new CardRepository(connectionString);
            IGameRepository gameRepository = new GameRepository(connectionString);
            ITradingRepository tradingRepository = new TradingRepository(connectionString);

            //Initialize server
            Server server = new(10001, userRepository, cardRepository, gameRepository, tradingRepository);
            server.Start();
        }
    }
}
