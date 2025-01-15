using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories.DTOs
{
    public class CardDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Damage { get; set; }
    }
}
