using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    internal class User
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public List<Card> Deck { get; set; } = new List<Card>();
        public List<Card> Stack { get; set; } = new List<Card>();

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            return $"{Username}, {Password}";
        }
    }
}
