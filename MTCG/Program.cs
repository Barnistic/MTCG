using MTCG.Models;
using MTCG.Services;
using MTCG.Interfaces;

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


            //Initialize game
            GameService Game = new(_userService, _cardService, _tradingService);

            //Initialize server
            Server server = new(10001, _userService);
            server.Start();
            //Game.StartGame();

        }
    }
}
