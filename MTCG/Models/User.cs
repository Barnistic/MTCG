using MTCG.Repositories.DTOs;
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
        public string Username { get; set; }
        public string Password { get; set; }
        public int ELO { get; set; } = 100;
        public int Coins { get; set; } = 20;

        //The deck will be the first 5 card of the Stack
        public List<Card> Deck { get; set; } = new();
        public List<Card> Stack { get; set; } = new();

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public User() { }

        public User(UserDTO userDto)
        {
            if(Guid.TryParse(userDto.id, out Guid parsedId))
            {
                this.Id = parsedId;
            }
            this.Username = userDto.name;
            this.Password = userDto.password;
            this.ELO = userDto.elo;
            this.Coins = userDto.coins;

        }
        public override string ToString()
        {
            return $"{Username}";
        }
    }
}
