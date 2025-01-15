using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories.DTOs
{
    public class StatDTO
    {
        public string Name { get; set; }
        public int Elo { get; set; }
        public int Win { get; set; }
        public int Loss { get; set; }
    }
}
