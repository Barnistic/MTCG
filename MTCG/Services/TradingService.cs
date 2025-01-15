using MTCG.Models;
using MTCG.Services.Interfaces;
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

        public void AddListing(User owner, Card card, string type, int minDamage, string element)
        {
            //TradeEntry newOrder = new(owner, card, type, minDamage, element);
            //Market?.Add(newOrder);
        }

        public bool ValidateTrade(Card offeredCard, TradeEntry requestedOrder)
        {
            //Check if card type (Monster or Spell) is matching
            /*bool TypeMatch = (requestedOrder.type == "Monster" && offeredCard is MonsterCard) || (requestedOrder.type == "Spell" && offeredCard is SpellCard);

            if (TypeMatch && offeredCard.Damage >= requestedOrder.minDamage)
            {
                if (requestedOrder.element == "" || requestedOrder.element == offeredCard.Type)
                {
                    return true;
                }
            }*/
            return false;
        }

        public void PrintMarket()
        {
            int i = 1;
            foreach (TradeEntry order in Market)
            {
                Console.WriteLine(order);
                i++;
            }
        }

        public TradeEntry GetListing(int i)
        {
            return Market[i];
        }

        public void RemoveListing(TradeEntry listing)
        {
            Market?.Remove(listing);
        }

        public int GetMarketCount()
        {
            return Market?.Count ?? 0;
        }
    }
}
