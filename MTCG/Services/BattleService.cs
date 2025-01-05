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
        private readonly ICardService cardService;
        private readonly IUserService userService;
        int result; // 0 = P1 wins, 1 = P2 wins, 2 = max rounds reached

        public BattleService(ICardService cardService, IUserService userService)
        {
            this.cardService = cardService;
            this.userService = userService;
        }
        public void Battle(User user)
        {
            User player1 = user;
            User player2 = ChooseEnemy();

            Console.WriteLine("The battle begins!");
            Console.WriteLine();


            while (true)
            {
                Random random = new();

                int randomP1 = random.Next(player1.Deck.Count-1);
                int randomP2 = random.Next(player2.Deck.Count-1);

                Card player1Card = player1.Deck[randomP1];
                Card player2Card = player2.Deck[randomP2];

                double DamageP1;
                double DamageP2;

                //Pure monster fights
                if (player1Card is MonsterCard && player2Card is MonsterCard)
                {
                    DamageP1 = player1Card.Damage;
                    DamageP2 = player2Card.Damage;
                } 
                else
                {
                    DamageP1 = CalculateDamage(player1.Deck[randomP1], player2.Deck[randomP2]);
                    DamageP2 = CalculateDamage(player2.Deck[randomP2], player1.Deck[randomP1]);
                }
                
                //Need to implement specific cards here: Goblin, Dragon, Wizard, Ork, Knight, Kraken, FireElf

                Console.WriteLine("Round " + CurrentRound + ":");
                Console.WriteLine($"{player1}: {player1Card}");
                Console.WriteLine($"{player2}: {player2Card}");

                if (DamageP1 > DamageP2)
                {
                    player1.Deck.Add(player2.Deck[randomP2]);
                    player2.Deck.RemoveAt(randomP2);
                    Console.WriteLine($"{player1} won this round! {DamageP1} vs {DamageP2}");
                }
                else if (DamageP2 > DamageP1)
                {
                    player2.Deck.Add(player1.Deck[randomP1]);
                    player1.Deck.RemoveAt(randomP1);
                    Console.WriteLine($"{player2} won this round! {DamageP1} vs {DamageP2}");
                }
                else if (DamageP1 == DamageP2)
                {
                    Console.WriteLine($"It's a tie! {DamageP1} vs {DamageP2}");
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
                else if (CurrentRound >= ROUNDS_CAP)
                {
                    result = 2;
                    break;
                }
            }
            ProcessResult(player1, player2);
        }

        private void ProcessResult(User player1, User player2)
        {
            switch (result)
            {
                case 0:
                    Console.WriteLine($"{player1} won after {CurrentRound} rounds!");
                    userService.Win(player1);
                    userService.Lose(player2);
                    // Second player loses;
                    break;
                case 1:
                    Console.WriteLine($"{player2} won after {CurrentRound} rounds!");
                    userService.Lose(player1);
                    userService.Win(player2);
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

        private User ChooseEnemy()
        {
            bool playerFound = false;

            while (!playerFound)
            {
                Console.WriteLine("Type the name of the player who you want to fight!");
                string input = Console.ReadLine();

                List<User> users = userService.GetUsers();

                foreach (User user in users)
                {
                    if (user.Username == input)
                    {
                        //Check if enemy has at least 5 cards (a deck)
                        if (user.Stack.Count() >= 5)
                        {
                            playerFound = true;
                            return user;
                        }
                    }
                }
            }
            return null;
        }
    }
}
