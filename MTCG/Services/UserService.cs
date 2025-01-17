﻿using MTCG.Models;
using MTCG.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{

    public class UserService : IUserService
    {
        private string AdminName = "admin";
        private string AdminPassword = "admin";

        private List<User> users = new List<User>(); // Initialize the list to avoid null reference

        public User Register()
        {
            // Check if user already exists in the database, handle errors
            // Add user to the database

            Console.WriteLine("Username: ");
            string username = Console.ReadLine();


            Console.WriteLine("Password: ");
            string password = Console.ReadLine();

            User newUser = new(username, password);

            return newUser;
        }

        public User Login()
        {
            Console.Clear();
            string username, password;
            while (true)
            {
                Console.WriteLine("Username: ");
                username = Console.ReadLine();


                Console.WriteLine("Password: ");
                password = Console.ReadLine();

                if (Authenticate(username, password))
                {
                    break;
                }
            }

            User newUser = new(username, password);

            return newUser;
        }

        public void Win(User user) { user.Coins += 2; user.ELO += 3; }
        public void Lose(User user) { user.ELO -= 5; }

        private bool Authenticate(string username, string password)
        {
            if (username == AdminName && password == AdminPassword) return true;
            return false;
        }

        public void AddUser(User user)
        {
            users.Add(user);
        }

        public List<User> GetUsers()
        {
            return users;
        }
    }
}
