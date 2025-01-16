using MTCG.Models;
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
        public void HandleCardsGet(User requester, NetworkStream stream)
        {
            Console.WriteLine("\nHandleCardsGet");
            if (requester.Stack.Any())
            {
                string cardsJson = JsonSerializer.Serialize(requester.Stack);
                SendResponse(stream, "200 OK", cardsJson);
            }
            else
            {
                SendResponse(stream, "204 No Content", "The user doesn't have any cards");
            }
        }

        public void HandleDeckGet(User requester, string[] requestLines, NetworkStream stream)
        {
            Console.WriteLine("\nHandleDeckGet");
            string format = "json";
            string endpoint = requestLines[0].Split(' ')[1];
            if (endpoint.Contains("format="))
            {
                format = endpoint.Split("format=")[1].Split(' ')[0];
            }
            if (requester.Deck.Any())
            {
                if (format == "plain")
                {
                    string deckDescription = string.Join("\n", requester.Deck.Select(c => c.Name));
                    SendResponse(stream, "200 OK", deckDescription);
                }
                else
                {
                    string deckJson = JsonSerializer.Serialize(requester.Deck);
                    SendResponse(stream, "200 OK", deckJson);
                }
            }
            else
            {
                SendResponse(stream, "204 No Content", "The deck doesn't have any cards");
            }
        }

        public void HandleDeckPut(User requester, string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandleDeckPut");
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                var cardIds = JsonSerializer.Deserialize<List<string>>(body);
                if (cardIds == null || cardIds.Count != 4)
                {
                    SendResponse(stream, "400 Bad Request", "The provided deck did not include the required amount of cards");
                    return;
                }
                var newDeck = requester.Stack.Where(c => cardIds.Contains(c.Id)).ToList();
                if (newDeck.Count != 4)
                {
                    //Check missing if card belongs to the user
                    SendResponse(stream, "403 Forbidden", "At least one of the provided cards does not belong to the user or is not available");
                    return;
                }
                requester.Deck = newDeck;
                _cardRepository.UpdateDeck(requester.Id, cardIds);
                SendResponse(stream, "200 OK", "The deck has been successfully configured");
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
        }
    }
}
