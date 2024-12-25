using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public abstract class Card
    {
        public Guid Id = new();
        public string Name { get; set; }
        public int Damage { get; set; }
        public string Type { get; set; }

        protected static readonly string[] Elements = { "Water", "Normal", "Fire" };
        public Card(string cardName, int cardDamage, string cardType)
        {
            Name = cardName;
            Damage = cardDamage;
            Type = cardType;
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
