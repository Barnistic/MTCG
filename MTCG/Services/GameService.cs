﻿using MTCG.Interfaces;
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

        User LoggedInUser;
        bool gameOver = false;
        public void StartGame()
        {
            while(true)
            {
                Console.WriteLine("Choose an option: Register(1), Login(2)");

                string choice = Console.ReadLine();

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
                cardService.PrintCardStack(LoggedInUser);
                break;
            }

            while (!gameOver)
            {
                Console.Clear();
                Console.WriteLine("Logged in user: " + LoggedInUser.Username);
                Console.WriteLine("Choose an option: Battle(1), Manage stack(2), Trade(3), Shop(4), Exit(5)");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Battle");
                        break;
                    case "2":
                        ManageStack(LoggedInUser);
                        break;
                    case "3":
                        Console.WriteLine("Trade");
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
                Console.WriteLine("Choose an option: Show stack(1), Show deck(2), Remove a card(3), Move cards(4), Exit(5)");

                string choice = Console.ReadLine();
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
                Console.WriteLine("Choose an option: Buy a package - 5 coins(1), Exit(2)");

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
    }
}
