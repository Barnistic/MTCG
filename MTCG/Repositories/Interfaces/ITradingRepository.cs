using MTCG.Models;
using MTCG.Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories.Interfaces
{
    public interface ITradingRepository
    {
        void AddTradingDeal(TradeEntry tradeDeal, string ownerId);
        void DeleteTradingDeal(string dealId);
        List<TradeEntry> GetTradingDeals();
        string GetTradeOwnerId(string tradeId);
    }
}
