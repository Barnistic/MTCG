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
        public Card(string cardName, int cardDamage, string cardType)
        {
            Name = cardName;
            Damage = cardDamage;
            Type = cardType;
        }

        private static readonly Random random = new();

        public static Card CreateRandomCard()
        {
            if (random.Next(2) == 0)
            {
                return new MonsterCard("MonsterCard1", 5, "Water");
            } else
            {
                return new SpellCard("SpellCard1", 3, "Fire");
            }
        }

        public override string ToString()
        {
            return $"CardName: {Name}, CardDamage: {Damage}, CardType: {Type}";
        }
    }
}
