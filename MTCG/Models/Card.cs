using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    enum CardType
    {
        WATER,
        FIRE,
        EARTH,
        AIR
    }
    internal abstract class Card
    {
        public Guid Id = new();
        public string Name { get; set; }
        public int Damage { get; set; }
        public CardType Type { get; set; }
        public Card(string cardName, int cardDamage, CardType cardType)
        {
            Name = cardName;
            Damage = cardDamage;
            Type = cardType;
        }

        private static readonly Random random = new();

        public static Card CreateRandomCard()
        {
            int randomDamage = random.Next(3, 7);
            CardType randomType = (CardType)random.Next(4);

            if (random.Next(2) == 0)
            {
                return new MonsterCard("MonsterCard1", randomDamage, randomType);
            } else
            {
                return new SpellCard("SpellCard1", randomDamage, randomType);
            }
        }

        public override string ToString()
        {
            return $"CardName: {Name}, CardDamage: {Damage}, CardType: {Type.ToString()}";
        }
    }
}
