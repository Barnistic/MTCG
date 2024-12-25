using MTCG.Interfaces;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    internal class GameService
    {
        IUserService userService = new UserService();
        ICardService cardService = new CardService();
        IBattleService battleService = new BattleService();
        ITradingService tradingService = new TradingService();

        private User? LoggedInUser;
        bool gameOver = false;
        public void StartGame()
        {
            //For testing purposes
            tradingService.AddListing(Card.CreateRandomCard(), "Monster", 10, "");
            tradingService.AddListing(Card.CreateRandomCard(), "Spell", 7, "Water");
            tradingService.AddListing(Card.CreateRandomCard(), "Monster", 2, "Fire");

            while (true)
            {
                Console.WriteLine("Choose an option: Register(1), Login(2)");

                string? choice = Console.ReadLine();

                switch(choice)
                {
                    case "1":
                        LoggedInUser = userService.Register();
                        cardService.AddRandomCard(30, LoggedInUser);
                        cardService.UpdateDeck(LoggedInUser);
                        Console.WriteLine("Successfully registered");
                        break;
                    case "2":
                        LoggedInUser = userService.Login();
                        cardService.UpdateDeck(LoggedInUser);
                        Console.WriteLine("Successfully logged in!");
                        break;
                }
                Console.WriteLine(LoggedInUser);
                break;
            }

            while (!gameOver)
            {
                Console.Clear();
                Console.WriteLine("Logged in user: " + LoggedInUser.Username);
                Console.WriteLine("Choose an option: Battle(1), Manage stack(2), Trade(3), Shop(4), Exit(5)");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        battleService.Battle(LoggedInUser);
                        battleService.ProcessResult(LoggedInUser);
                        break;
                    case "2":
                        ManageStack(LoggedInUser);
                        break;
                    case "3":
                        tradingService.PrintMarket();
                        Trade(LoggedInUser);
                        break;
                    case "4":
                        Shop(LoggedInUser);
                        break;
                    case "5":
                        gameOver = true;
                        break;
                }
            }
        }
        public void ManageStack(User user)
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("Choose an option: Show stack(1), Show deck(2), Remove a card(3), Move cards(4), Back(5)");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        cardService.PrintCardStack(user);
                        break;
                    case "2":
                        cardService.PrintCardDeck(user);
                        break;
                    case "3":
                        cardService.PrintCardStack(user);
                        Console.WriteLine("Choose a card to remove: ");
                        int input = Int32.Parse(Console.ReadLine());
                        cardService.RemoveCard(user, input - 1);
                        cardService.UpdateDeck(user);
                        break;
                    case "4":
                        cardService.PrintCardStack(user);
                        Console.WriteLine("Choose the first card: ");
                        int i = Int32.Parse(Console.ReadLine());
                        Console.WriteLine("Choose the second card: ");
                        int j = Int32.Parse(Console.ReadLine());
                        cardService.MoveCard(user, i-1, j-1);
                        cardService.UpdateDeck(user);
                        break;
                    case "5":
                        exit = true;
                        break;
                }
            }
            
        }

        public void Shop(User user)
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("Choose an option: Buy a package - 5 coins(1), Back(2)");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        PackageService.BuyPackage(user);
                        cardService.UpdateDeck(user);
                        break;
                    case "2":
                        exit = true;
                        break;
                }
            }
        }

        public void Trade(User user)
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("Choose an option: Accept a trade(1), Add a listing(2),  Back(3)");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Choose which trade to accept! (1-" + tradingService.GetMarketCount() + ")");
                        int tradeNumber = Int32.Parse(Console.ReadLine());
                        if (tradeNumber < 1 || tradeNumber > tradingService.GetMarketCount())
                        {
                            Console.WriteLine("Choose a number between 1-" + tradingService.GetMarketCount());
                            continue;
                        } 
                        else
                        {
                            cardService.PrintCardStack(LoggedInUser);
                            Console.WriteLine("Pick which card to trade from your stack!");
                            int stackNumber = Int32.Parse(Console.ReadLine());
                            if (tradingService.AcceptTrade(LoggedInUser.Stack[stackNumber - 1], tradingService.GetListing(tradeNumber - 1))) 
                            {
                                cardService.RemoveCard(LoggedInUser, stackNumber - 1);
                            }
                            else
                            {
                                Console.WriteLine("Card doesn't match requested specifications!");
                                continue;
                            }
                        }
                        break;
                    case "2":
                        break;
                    case "3":
                        exit = true;
                        break;
                }
            }
        }
    }
}
