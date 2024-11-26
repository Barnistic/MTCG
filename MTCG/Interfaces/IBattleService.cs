using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Interfaces
{
    public interface IBattleService
    {
        void Battle(User user);
        void ProcessResult(User user);
        double CalculateDamage(Card attacker, Card defender);
    }
}
