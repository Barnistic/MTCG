using MTCG.Models;
using MTCG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    internal class GameService
    {
        IUserService _userService;
        ICardService _cardService;
        ITradingService _tradingService;

        private User? LoggedInUser;
        private User testUser = new("testUser", "password"); //For testing

        bool gameOver = false;

        public GameService(IUserService userService, ICardService cardService, ITradingService tradingService)
        {
            this._userService = userService;
            this._cardService = cardService;
            this._tradingService = tradingService;
        }

        public void StartGame()
        {
            //For testing purposes
            _userService.AddUser(testUser);
            _cardService.AddRandomCard(30, testUser);
            _cardService.UpdateDeck(testUser);

            _tradingService.AddListing(testUser, Card.CreateRandomCard(), "Monster", 10, "");
            _tradingService.AddListing(testUser, Card.CreateRandomCard(), "Spell", 7, "Water");
            _tradingService.AddListing(testUser, Card.CreateRandomCard(), "Monster", 2, "Fire");

            while (true)
            {
                Console.WriteLine("Choose an option: Register(1), Login(2)");

                string? choice = Console.ReadLine();

                switch(choice)
                {
                    case "1":
                        LoggedInUser = _userService.Register();
                        _userService.AddUser(LoggedInUser);
                        _cardService.AddRandomCard(30, LoggedInUser);
                        _cardService.UpdateDeck(LoggedInUser);
                        Console.WriteLine("Successfully registered");
                        break;
                    case "2":
                        LoggedInUser = _userService.Login();
                        _userService.AddUser(LoggedInUser);
                        _cardService.UpdateDeck(LoggedInUser);
                        Console.WriteLine("Successfully logged in!");
                        break;
                    default:
                        continue;
                }
                Console.WriteLine(LoggedInUser);
                break;
            }

            while (!gameOver)
            {
                //Console.Clear();
                Console.WriteLine("Logged in user: " + LoggedInUser.Username);
                Console.WriteLine("Choose an option: Battle(1), Manage stack(2), Trade(3), Shop(4), Scoreboard(5), Exit(6)");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (LoggedInUser.Stack.Count() < 5)
                        {
                            Console.WriteLine("Not enough cards!");
                            continue;
                        }
                        IBattleService battleService = new BattleService(_cardService, _userService);
                        battleService.Battle(LoggedInUser);
                        break;
                    case "2":
                        ManageStack(LoggedInUser);
                        break;
                    case "3":
                        _tradingService.PrintMarket();
                        Trade(LoggedInUser);
                        break;
                    case "4":
                        Shop(LoggedInUser);
                        break;
                    case "5":
                        PrintScoreboard();
                        break;
                    case "6":
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
                        _cardService.PrintCardStack(user);
                        break;
                    case "2":
                        _cardService.PrintCardDeck(user);
                        break;
                    case "3":
                        _cardService.PrintCardStack(user);
                        Console.WriteLine("Choose a card to remove: ");
                        int input = Int32.Parse(Console.ReadLine());
                        _cardService.RemoveCard(user, input - 1);
                        _cardService.UpdateDeck(user);
                        break;
                    case "4":
                        _cardService.PrintCardStack(user);
                        Console.WriteLine("Choose the first card: ");
                        int i = Int32.Parse(Console.ReadLine());
                        Console.WriteLine("Choose the second card: ");
                        int j = Int32.Parse(Console.ReadLine());
                        _cardService.MoveCard(user, i-1, j-1);
                        _cardService.UpdateDeck(user);
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
                        _cardService.UpdateDeck(user);
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
                        _tradingService.PrintMarket();
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
            Console.WriteLine($"Choose which trade to accept! (1-{_tradingService.GetMarketCount()})");
            int tradeNumber = Int32.Parse(Console.ReadLine());
            if (tradeNumber < 1 || tradeNumber > _tradingService.GetMarketCount())
            {
                Console.WriteLine($"Choose a number between 1-{_tradingService.GetMarketCount()}");
                return;
            }
            else
            {
                _cardService.PrintCardStack(LoggedInUser);
                Console.WriteLine("Pick which card to trade from your stack!");

                int stackNumber = Int32.Parse(Console.ReadLine());
                TradeEntry selectedListing = _tradingService.GetListing(tradeNumber - 1);

                bool isTradeValid = _tradingService.ValidateTrade(LoggedInUser.Stack[stackNumber - 1], selectedListing);
                Console.WriteLine("DEBUG: Selected card from stack: " + LoggedInUser.Stack[stackNumber - 1] + ", Listing: " + _tradingService.GetListing(tradeNumber - 1));

                if (isTradeValid)
                {
                    _cardService.AddCard(selectedListing.owner, LoggedInUser.Stack[stackNumber - 1]); //Add requested card to the trade's owner
                    _cardService.AddCard(LoggedInUser, selectedListing.card); //Add traded card to the user's deck
                    _cardService.RemoveCard(LoggedInUser, stackNumber - 1); //Remove selected card from the stack
                    _tradingService.RemoveListing(selectedListing); //Remove listing from the market
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
            _cardService.PrintCardStack(LoggedInUser);
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

            _tradingService.AddListing(LoggedInUser, selectedCard, requestedType, requestedDamage, requestedElement);
            _cardService.RemoveCard(LoggedInUser, stackNumber - 1);
        }

        private void PrintScoreboard()
        {
            int i = 1;
            List<User> users = _userService.GetUsers();
            foreach (User user in users)
            {
                Console.WriteLine($"{i}: {user}, ELO: {user.ELO}");
                i++;
            }
        }
    }
}
