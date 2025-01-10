using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Repositories.DTOs
{
    public class UserDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int elo { get; set; }
        public int coins { get; set; }
    }
}
