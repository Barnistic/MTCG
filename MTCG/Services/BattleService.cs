using MTCG.Interfaces;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    public class BattleService : IBattleService
    {
        const int ROUNDS_CAP = 100;
        int CurrentRound = 1;
        CardService cardService = new();
        public void StartBattle(User user)
        {
            User player1 = user;
            User player2 = new();
            cardService.AddRandomCard(5, player2);
            int result;

            while (player1.Deck.Count > 0 || player2.Deck.Count > 0 || CurrentRound >= 100)
            {
                Random random = new();

                int randomP1 = random.Next(player1.Deck.Count);
                int randomP2 = random.Next(player2.Deck.Count);

                double DamageP1 = CalculateDamage(player1.Deck[randomP1], player2.Deck[randomP2]);
                double DamageP2 = CalculateDamage(player2.Deck[randomP2], player1.Deck[randomP1]);

                if (DamageP1 > DamageP2)
                {
                    player1.Deck.Add(player2.Deck[randomP2]);
                    player2.Deck.RemoveAt(0);
                }
                else if (DamageP2 > DamageP1)
                {
                    player2.Deck.Add(player1.Deck[randomP1]);
                    player1.Deck.RemoveAt(0);
                }

                CurrentRound++;

                if (player1.Deck.Count == 0)
                {

                }

            }
        }

        public double CalculateDamage(Card attacker, Card defender)
        {
            if (attacker.Type == "Water" && defender.Type == "Fire")
            {
                return attacker.Damage * 2;
            }
            else if (attacker.Type == "Water" && defender.Type == "Normal")
            {
                return attacker.Damage * 0.5;
            }
            else if (attacker.Type == "Fire" && defender.Type == "Normal")
            {
                return attacker.Damage * 2;
            }
            else if (attacker.Type == "Fire" && defender.Type == "Water")
            {
                return attacker.Damage * 0.5;
            }
            else if (attacker.Type == "Normal" && defender.Type == "Water")
            {
                return attacker.Damage * 2;
            }
            else if (attacker.Type == "Normal" && defender.Type == "Fire")
            {
                return attacker.Damage * 0.5;
            }
            else return attacker.Damage;
        }
    }
}
