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
        private User testUser = new("testUser", "password"); //For testing
        bool gameOver = false;
        public void StartGame()
        {
            //For testing purposes
            cardService.AddRandomCard(10, testUser);
            cardService.UpdateDeck(testUser);

            tradingService.AddListing(testUser, Card.CreateRandomCard(), "Monster", 10, "");
            tradingService.AddListing(testUser, Card.CreateRandomCard(), "Spell", 7, "Water");
            tradingService.AddListing(testUser, Card.CreateRandomCard(), "Monster", 2, "Fire");

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
                //Console.Clear();
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
                Console.WriteLine("Choose an option: Print market(1), Accept a trade(2), Add a listing(3),  Back(4)");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    //Print market
                    case "1":
                        tradingService.PrintMarket();
                        break;
                    //Accept a trade
                    case "2":
                        AcceptTrade();
                        break;
                    //Add a listing
                    case "3":
                        CreateListing();
                        break;
                    case "4":
                        exit = true;
                        break;
                }
            }
        }

        private void AcceptTrade()
        {
            Console.WriteLine($"Choose which trade to accept! (1-{tradingService.GetMarketCount()})");
            int tradeNumber = Int32.Parse(Console.ReadLine());
            if (tradeNumber < 1 || tradeNumber > tradingService.GetMarketCount())
            {
                Console.WriteLine($"Choose a number between 1-{tradingService.GetMarketCount()}");
                return;
            }
            else
            {
                cardService.PrintCardStack(LoggedInUser);
                Console.WriteLine("Pick which card to trade from your stack!");

                int stackNumber = Int32.Parse(Console.ReadLine());
                TradeEntry selectedListing = tradingService.GetListing(tradeNumber - 1);

                bool isTradeValid = tradingService.ValidateTrade(LoggedInUser.Stack[stackNumber - 1], selectedListing);
                Console.WriteLine("DEBUG: Selected card from stack: " + LoggedInUser.Stack[stackNumber - 1] + ", Listing: " + tradingService.GetListing(tradeNumber - 1));

                if (isTradeValid)
                {
                    cardService.AddCard(selectedListing.owner, LoggedInUser.Stack[stackNumber - 1]); //Add requested card to the trade's owner
                    cardService.AddCard(LoggedInUser, selectedListing.card); //Add traded card to the user's deck
                    cardService.RemoveCard(LoggedInUser, stackNumber - 1); //Remove selected card from the stack
                    tradingService.RemoveListing(selectedListing); //Remove listing from the market
                    Console.WriteLine("Successfull trade!");
                }
                else
                {
                    Console.WriteLine("Card doesn't match requested specifications!");
                    return;
                }
            }
        }

        private void CreateListing()
        {
            Card selectedCard;
            cardService.PrintCardStack(LoggedInUser);
            Console.WriteLine("Choose which card to put up for trade!");
            int stackNumber = Int32.Parse(Console.ReadLine());

            //Check input
            if (stackNumber < 1 || stackNumber > LoggedInUser.Stack.Count() + 1)
            {
                Console.WriteLine($"Choose a card between 1-{LoggedInUser.Stack.Count()}");
                return;
            }
            else
            {
                selectedCard = LoggedInUser.Stack[stackNumber - 1];
            }

            Console.WriteLine("Do you want in return a spell(1) or monster(2) card?");
            string requestedType = Console.ReadLine();

            if (requestedType == "1")
            {
                requestedType = "Spell";
            }
            else if (requestedType == "2")
            {
                requestedType = "Monster";
            }
            else
            {
                Console.WriteLine("Invalid input!");
                return;
            }

            Console.WriteLine("What minimum damage card do you want in return?");
            int requestedDamage = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Do you want type fire(1), water(2), normal(3), or all(4)?");
            string requestedElement = Console.ReadLine();

            switch (requestedElement)
            {
                case "1":
                    requestedElement = "Fire";
                    break;
                case "2":
                    requestedElement = "Water";
                    break;
                case "3":
                    requestedElement = "Normal";
                    break;
                case "4":
                    requestedElement = "";
                    break;
                default:
                    Console.WriteLine("Invalid input!");
                    break;
            }

            tradingService.AddListing(LoggedInUser, selectedCard, requestedType, requestedDamage, requestedElement);
            cardService.RemoveCard(LoggedInUser, stackNumber - 1);
        }
    }
}
