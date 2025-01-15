using MTCG.Models;
using MTCG.Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTCG.Infrastructure
{
    public partial class RequestHandler
    {
        private void HandleTradingsGet(NetworkStream stream)
        {
            Console.WriteLine("\nHandleTradingsGet");
            if (_tradingDeals.Any())
            {
                List<TradeEntry> deals = _tradingRepository.GetTradingDeals();
                string dealsJson = JsonSerializer.Serialize(_tradingDeals);
                SendResponse(stream, "200 OK", dealsJson);
            }
            else
            {
                SendResponse(stream, "204 No Content", "There are no trading deals available");
            }
        }

        private void HandleTradingPost(User requester, string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandleTradingPost");
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                var tradingDeal = JsonSerializer.Deserialize<TradeEntry>(body);
                if (tradingDeal == null || tradingDeal.CardToTrade == null)
                {
                    SendResponse(stream, "400 Bad Request", "Invalid request payload");
                    return;
                }
                if (_tradingDeals.Any(d => d.CardToTrade == tradingDeal.CardToTrade))
                {
                    SendResponse(stream, "409 Conflict", "A deal with this card ID already exists");
                    return;
                }
                if (!requester.Stack.Any(c => c.Id == tradingDeal.CardToTrade) || requester.Deck.Any(c => c.Id == tradingDeal.CardToTrade))
                {
                    SendResponse(stream, "403 Forbidden", "The deal contains a card that is not owned by the user or locked in the deck");
                    return;
                }

                _tradingDeals.Add(tradingDeal);
                _tradingRepository.AddTradingDeal(tradingDeal, requester.Id);
                SendResponse(stream, "201 Created", "Trading deal successfully created");
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
        }

        private void HandleTradingDelete(string tradingDealId, User requester, NetworkStream stream)
        {
            Console.WriteLine("\nHandleTradingDelete");
            var deal = _tradingDeals.FirstOrDefault(d => d.Id == tradingDealId);
            if (deal == null)
            {
                SendResponse(stream, "404 Not Found", "The provided deal ID was not found");
                return;
            }
            if (!requester.Stack.Any(c => c.Id == deal.CardToTrade))
            {
                SendResponse(stream, "403 Forbidden", "The deal contains a card that is not owned by the user");
                return;
            }
            _tradingDeals.Remove(deal);
            _tradingRepository.DeleteTradingDeal(tradingDealId);
            SendResponse(stream, "200 OK", "Trading deal successfully deleted");
        }

        private void HandleTradingDealPost(string tradingDealId, User requester, string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandleTradingDealPost");
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                var offeredCardId = JsonSerializer.Deserialize<string>(body);
                Card offeredCard = _cardRepository.GetCardById(offeredCardId);
                var deal = _tradingDeals.FirstOrDefault(d => d.Id == tradingDealId);
                string dealOwnerId = _tradingRepository.GetTradeOwnerId(tradingDealId);
                
                if (deal == null)
                {
                    SendResponse(stream, "404 Not Found", "The provided deal ID was not found");
                    return;
                }

                //Check if type matches
                bool typeMatch;
                if (deal.Type == "monster" && !offeredCard.Name.ToLower().Contains("spell"))
                {
                    typeMatch = true;
                }
                else if (deal.Type == "spell" && offeredCard.Name.ToLower().Contains("spell"))
                {
                    typeMatch = true;
                }
                else { typeMatch = false; }

                if (requester.Stack.Any(c => c.Id == deal.CardToTrade) || !(offeredCard.Damage >= deal.MinimumDamage && typeMatch))
                {
                    SendResponse(stream, "403 Forbidden", "The offered card is not owned by the user, or the requirements are not met, or the offered card is locked in the deck, or the user tries to trade with self");
                    return;
                }

                _cardRepository.ChangeCardOwner(dealOwnerId, offeredCardId);
                _cardRepository.ChangeCardOwner(requester.Id, deal.CardToTrade);
                _tradingRepository.DeleteTradingDeal(tradingDealId);
                _tradingDeals.Remove(deal);
                SendResponse(stream, "200 OK", "Trading deal successfully executed");
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
        }
    }
}
