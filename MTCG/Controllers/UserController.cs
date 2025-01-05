using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Controllers
{
    internal class UserController
    {
        private readonly Dictionary<string, User> _users;
        public UserController(Dictionary<string, User> users)
        {
            _users = users;
        }

        public HttpResponse HandleUserPost(User user, NetworkStream stream)
        {
            //Check if user already exissts
            if (_users.ContainsKey(user.Username))
            {
                return new HttpResponse("400 Bad Request", "User already exists");
            }

            _users.Add(user.Username, user);
            return new HttpResponse("201 Created", "User created scucessfully");
        }

        public HttpResponse HandleSessionPost(User user, NetworkStream stream)
        {
            if (_users.ContainsKey(user.Username) && _users[user.Username].Password == user.Password)
            {
                string token = $"{user.Username}-token";

                return new HttpResponse("200 OK", token);
            }
            else
            {
                return new HttpResponse("401 Unauthorized", "Invalid credentials");
            }
        }

    }
}
