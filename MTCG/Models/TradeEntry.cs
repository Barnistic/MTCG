using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class TradeEntry
    {
        public Card card;
        public string type;
        public int minDamage;
        public string element;

        public TradeEntry(Card card, string type, int minDamage, string element)
        {
            this.card = card;
            this.type = type;
            this.minDamage = minDamage;
            this.element = element;
        }
    }
}
