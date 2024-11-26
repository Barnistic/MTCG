using MTCG.Interfaces;
using MTCG.Models;
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

        private bool Authenticate(string username, string password)
        {
            if (username == AdminName && password == AdminPassword) return true;
            return false;
        }
    }
}
