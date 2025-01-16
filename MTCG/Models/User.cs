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
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int ELO { get; set; } = 100;
        public int Coins { get; set; } = 20;
        public int Win { get; set; } = 0;
        public int Loss { get; set; } = 0;
        public UserProfile Profile;

        //The deck will be the first 5 card of the Stack
        public List<Card> Deck { get; set; } = new();
        public List<Card> Stack { get; set; } = new();

        public User(string username, string password)
        {
            Username = username;
            Password = password;
            Id = Guid.NewGuid().ToString();
        }

        public User() { }

        public User(UserDTO userDto)
        {

            this.Id = userDto.Id;
            this.Username = userDto.Name;
            this.Password = userDto.Password;
            this.ELO = userDto.Elo;
            this.Coins = userDto.Coins;

        }
        public override string ToString()
        {
            return $"{Username}";
        }
    }
}
