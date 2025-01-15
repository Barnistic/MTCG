using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories.Interfaces
{
    public interface ICardRepository
    {
        void CreateCard(Card card);
        void ChangeCardOwner(string userId, string cardId);
        List<Card> GetUserStack(string ownerid);
        void UpdateDeck(string userId, List<string> cardId);
        Card GetCardById(string cardId);
    }
}
