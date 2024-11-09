using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{

    

    internal class Card
    {
        public string CardName { get; set; }
        public string CardType { get; set; }
        public Card(string cardName, string cardType)
        {
            CardName = cardName;
            CardType = cardType;
        }

        public override string ToString()
        {
            return $"CardName: {CardName}, CardType: {CardType}";
        }
    }
}
