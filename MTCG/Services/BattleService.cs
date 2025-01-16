using MTCG.Models;
using MTCG.Services.Interfaces;
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
        int result; // 0 = P1 wins, 1 = P2 wins, 2 = max rounds reached
        private StringBuilder battleLog;

        public BattleService()
        {
            battleLog = new StringBuilder();
        }

        public void Battle(User user1, User user2)
        {
            User player1 = user1;
            User player2 = user2;

            battleLog.Clear();
            battleLog.AppendLine("The battle begins!");

            while (true)
            {
                Random random = new();

                int randomP1 = random.Next(player1.Deck.Count - 1);
                int randomP2 = random.Next(player2.Deck.Count - 1);

                Card player1Card = player1.Deck[randomP1];
                Card player2Card = player2.Deck[randomP2];

                double DamageP1;
                double DamageP2;

                //Pure monster fights
                if (player1Card.Type == "Monster" && player2Card.Type == "Monster")
                {
                    DamageP1 = player1Card.Damage;
                    DamageP2 = player2Card.Damage;
                }
                else
                {
                    DamageP1 = CalculateDamage(player1.Deck[randomP1], player2.Deck[randomP2]);
                    DamageP2 = CalculateDamage(player2.Deck[randomP2], player1.Deck[randomP1]);
                }

                battleLog.AppendLine($"Round {CurrentRound}:");
                battleLog.AppendLine($"{player1.Username}: {player1Card.Name}");
                battleLog.AppendLine($"{player2.Username}: {player2Card.Name}");

                if (DamageP1 > DamageP2)
                {
                    player1.Deck.Add(player2.Deck[randomP2]);
                    player2.Deck.RemoveAt(randomP2);
                    battleLog.AppendLine($"{player1.Username} won this round! {DamageP1} vs {DamageP2}");
                }
                else if (DamageP2 > DamageP1)
                {
                    player2.Deck.Add(player1.Deck[randomP1]);
                    player1.Deck.RemoveAt(randomP1);
                    battleLog.AppendLine($"{player2.Username} won this round! {DamageP1} vs {DamageP2}");
                }
                else if (DamageP1 == DamageP2)
                {
                    battleLog.AppendLine($"It's a tie! {DamageP1} vs {DamageP2}");
                }

                CurrentRound++;
                battleLog.AppendLine();

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
                    battleLog.AppendLine($"{player1.Username} won after {CurrentRound} rounds!");
                    player1.Win++;
                    player1.Coins += 2;
                    player1.ELO += 3;

                    player2.Loss++;
                    break;
                case 1:
                    battleLog.AppendLine($"{player2.Username} won after {CurrentRound} rounds!");
                    player2.Win++;
                    player2.Coins += 2;
                    player2.ELO += 3;

                    player1.Loss++;
                    break;
                case 2:
                    battleLog.AppendLine("It's a tie!");
                    player1.ELO += 1;
                    player2.ELO += 1;
                    break;
            }
        }

        public double CalculateDamage(Card attacker, Card defender)
        {
            if (attacker.Type == "Water" && defender.Type == "Fire")
            {
                return attacker.Damage * 2;
            }
            else if (attacker.Type == "Water" && defender.Type == "Regular")
            {
                return attacker.Damage * 0.5;
            }
            else if (attacker.Type == "Fire" && defender.Type == "Regular")
            {
                return attacker.Damage * 2;
            }
            else if (attacker.Type == "Fire" && defender.Type == "Water")
            {
                return attacker.Damage * 0.5;
            }
            else if (attacker.Type == "Regular" && defender.Type == "Water")
            {
                return attacker.Damage * 2;
            }
            else if (attacker.Type == "Regular" && defender.Type == "Fire")
            {
                return attacker.Damage * 0.5;
            }
            else return attacker.Damage;
        }

        public string GetBattleLog()
        {
            return battleLog.ToString();
        }
    }
}
