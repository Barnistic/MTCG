using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class TradeEntry
    {
        public User owner;
        public Card card;
        public string type;
        public int minDamage;
        public string element;

        public TradeEntry(User owner, Card card, string type, int minDamage, string element)
        {
            this.owner = owner;
            this.card = card;
            this.type = type;
            this.minDamage = minDamage;
            this.element = element;
        }

        public override string ToString()
        {
            if (element == "")
            {
                return $"{owner}: [H] {card} [W] {type}, {minDamage}";
            }
            else
            {
                return $"{owner}: [H] {card} [W] {type}, {minDamage}, {element}";
            }
            
        }
    }
}
