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
        public void HandlePackagePost(User requester, string[] requestLines, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandlePackagePost");
                if (requester.Username != "admin")
                {
                    SendResponse(stream, "403 Forbidden", "Provided user is not \"admin\"");
                    return;
                }
                string body = string.Join("\r\n", requestLines).Split("\r\n\r\n")[1];
                var cardDTOs = JsonSerializer.Deserialize<List<CardDTO>>(body);
                if (cardDTOs == null || cardDTOs.Count != 5)
                {
                    SendResponse(stream, "400 Bad Request", "Invalid request payload");
                    return;
                }

                //Convert CardDTOs to Cards
                var cards = cardDTOs.Select(dto => new Card(dto)).ToList();

                foreach (var card in cards)
                {
                    if (_cardPackages.Any(c => c.Id == card.Id))
                    {
                        SendResponse(stream, "409 Conflict", "At least one card in the package already exists");
                        return;
                    }
                    //Add new card to the DB
                    _cardRepository.CreateCard(card);
                }

                _cardPackages.AddRange(cards);
                SendResponse(stream, "201 Created", "Package and cards successfully created");
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
        }

        public void HandleTransactionPost(User requester, NetworkStream stream)
        {
            try
            {
                Console.WriteLine("\nHandleTransactionPost");
                if (_cardPackages.Count < 5)
                {
                    SendResponse(stream, "404 Not Found", "No card package available for buying");
                    return;
                }
                if (requester.Coins < 5)
                {
                    SendResponse(stream, "403 Forbidden", "Not enough money for buying a card package");
                    return;
                }
                var package = _cardPackages.Take(5).ToList();
                _cardPackages.RemoveRange(0, 5);
                requester.Coins -= 5;
                requester.Stack.AddRange(package);
                //Change the ownerid of the cards in the database
                foreach (Card card in package)
                {
                    _cardRepository.ChangeCardOwner(requester.Id, card.Id);
                }
                SendResponse(stream, "200 OK", JsonSerializer.Serialize(package));
            }
            catch (Exception)
            {
                SendResponse(stream, "400 Bad Request", "Invalid JSON payload");
            }
        }
    }
}
