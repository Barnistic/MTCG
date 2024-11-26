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

        public List<Card> Deck { get; set; } = new();
        public List<Card> Stack { get; set; } = new();

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Coins = 20;
        }

        public User()
        {
            Coins = 20;
        }
        public override string ToString()
        {
            return $"{Username}, {Password}";
        }
    }
}
