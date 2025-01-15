using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class UserProfile
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }
    }
}
