using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    internal class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Password { get; set; }
        public int ELO { get; set; } = 500;
        public int Coins { get; set; } = 20;

        public List<Card> Deck { get; set; } = [];
        public List<Card> Stack { get; set; } = [];

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public void AddCard(Card card)
        {
            Stack.Add(card);
        }

        public void RemoveCard(Card card)
        {
            Stack.Remove(card);
        }

        public static void Register(string username, string password)
        {
            // Check if user already exists in the database, handle errors
            // Add user to the database
            User newUser = new(username, password);
            Console.WriteLine(newUser);
        }

        public override string ToString()
        {
            return $"{Username}, {Password}";
        }
    }
}
