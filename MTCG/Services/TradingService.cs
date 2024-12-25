using MTCG.Interfaces;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MTCG.Services
{
    public class TradingService : ITradingService
    {
        

        public List<TradeEntry>? Market { get; set; } = new List<TradeEntry>();

        public void AddListing(Card card, string type, int minDamage, string element)
        {
            TradeEntry newOrder = new(card, type, minDamage, element);
            Market?.Add(newOrder);
        }

        public bool AcceptTrade(Card offeredCard, TradeEntry requestedOrder)
        {
            //Check if card type (Monster or Spell) is matching
            bool TypeMatch = (requestedOrder.type == "Monster" && offeredCard is MonsterCard) || (requestedOrder.type == "SpellCard" && offeredCard is SpellCard);

            if (TypeMatch && offeredCard.Damage >= requestedOrder.minDamage)
            {
                if (requestedOrder.element != "" || requestedOrder.element == offeredCard.Type)
                {
                    Market?.Remove(requestedOrder);
                    return true;
                }
            }
            return false;
        }

        public void PrintMarket()
        {
            int i = 1;
            foreach (TradeEntry order in Market)
            {
                if (order.element == "")
                {
                    Console.WriteLine(i + ": [H] " + order.card.Name + " [W] " + order.type + " , min. dmg: " + order.minDamage);
                } 
                else
                {
                    Console.WriteLine(i + ": [H] " + order.card.Name + " [W] " + order.element + " " + order.type + " , min. dmg: " + order.minDamage);
                }
                i++;
            }
        }

        public TradeEntry GetListing(int i)
        {
            return Market[i];
        }

        public int GetMarketCount()
        {
            return Market?.Count ?? 0;
        }
    }
}
