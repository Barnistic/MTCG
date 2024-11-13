using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    internal class MonsterCard : Card
    {
        public MonsterCard(string name, int damage, string type) : base(name, damage, type)
        {
        }
    }
}
