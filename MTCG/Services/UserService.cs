using MTCG.Interfaces;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    public class UserService : IUserService
    {
        public User Register(string username, string password)
        {
            // Check if user already exists in the database, handle errors
            // Add user to the database
            User newUser = new(username, password);
            Console.WriteLine(newUser);
            return newUser;
        }
    }
}
