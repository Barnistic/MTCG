using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MTCG.Services.TradingService;

namespace MTCG.Interfaces
{
    public interface ITradingService
    {
        void AddListing(User owner, Card card, string type, int minDamage, string element);
        bool ValidateTrade(Card offeredCard, TradeEntry requestedOrder);
        void PrintMarket();
        TradeEntry GetListing(int i);
        void RemoveListing(TradeEntry listing);
        int GetMarketCount();
    }
}
