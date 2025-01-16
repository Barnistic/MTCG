using MTCG.Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Damage { get; set; }
        public string Type { get; set; }

        protected static readonly string[] Elements = { "Water", "Normal", "Fire" };
        public Card(string cardName, int cardDamage, string cardType)
        {
            Name = cardName;
            Damage = cardDamage;
            Type = cardType;
            Id = Guid.NewGuid().ToString();
        }

        public Card(CardDTO cardDto)
        {
            this.Name = cardDto.Name;
            if (cardDto.Name.StartsWith("Fire"))
            {
                this.Type = "Fire";
            }
            else if (cardDto.Name.StartsWith("Water"))
            {
                this.Type = "Water";
            }
            else
            {
                this.Type = "Regular";
            }
            this.Damage = cardDto.Damage;
            this.Id = cardDto.Id;
        }

        private static readonly Random random = new();

        public static Card CreateRandomCard()
        {
            string randomElement = Elements[random.Next(Elements.Length)];
            int randomStrength = random.Next(4, 16);
            if (random.Next(2) == 0)
            {
                return new MonsterCard(randomElement + " " + randomStrength, randomStrength, randomElement);
            } else
            {
                return new SpellCard(randomElement + " " + randomStrength, randomStrength, randomElement);
            }
        }
    }
}
