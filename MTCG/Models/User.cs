using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; private set; }
        public string Password { get; private set; }
        public int ELO { get; private set; } = 500;
        public int Coins { get; set; } = 4;

        public List<Card> Deck { get; set; } = [];
        public List<Card> Stack { get; set; } = [];

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
