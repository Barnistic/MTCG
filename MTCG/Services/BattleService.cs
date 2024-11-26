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
        UserService userService = new();
        int result; // 0 = P1 wins, 1 = P2 wins, 2 = max rounds reached
        public void Battle(User user)
        {
            Console.WriteLine("The battle begins!");
            Console.WriteLine();

            User player1 = user;
            User player2 = new();
            cardService.AddRandomCard(5, player2);
            cardService.UpdateDeck(player2);

            while (true)
            {
                Random random = new();

                int randomP1 = random.Next(player1.Deck.Count-1);
                int randomP2 = random.Next(player2.Deck.Count-1);

                double DamageP1 = CalculateDamage(player1.Deck[randomP1], player2.Deck[randomP2]);
                double DamageP2 = CalculateDamage(player2.Deck[randomP2], player1.Deck[randomP1]);

                Console.WriteLine("Round " + CurrentRound + ":");
                Console.WriteLine("Player1: " + player1.Deck[randomP1].Name);
                Console.WriteLine("Player2: " + player2.Deck[randomP2].Name);

                if (DamageP1 > DamageP2)
                {
                    player1.Deck.Add(player2.Deck[randomP2]);
                    player2.Deck.RemoveAt(0);
                    Console.WriteLine("Player1 won this round!" + DamageP1 + " vs " + DamageP2);
                }
                else if (DamageP2 > DamageP1)
                {
                    player2.Deck.Add(player1.Deck[randomP1]);
                    player1.Deck.RemoveAt(0);
                    Console.WriteLine("Player2 won this round! " + DamageP1 + " vs " + DamageP2);
                }

                CurrentRound++;
                Console.WriteLine();

                if (player1.Deck.Count == 0)
                {
                    result = 0;
                    break;
                } 
                else if (player2.Deck.Count == 0)
                {
                    result = 1;
                    break;
                } 
                else if (CurrentRound >= 100)
                {
                    result = 2;
                    break;
                }
            }
        }

        public void ProcessResult(User user)
        {
            switch (result)
            {
                case 0:
                    Console.WriteLine(user.Username + " won after " + CurrentRound + " rounds!");
                    userService.Win(user);
                    // Second player loses;
                    break;
                case 1:
                    Console.WriteLine("Player 2 won after " + CurrentRound + " rounds!");
                    userService.Lose(user);
                    //Second player wins
                    break;
                case 2:
                    Console.WriteLine("It's a tie!");
                    break;
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
