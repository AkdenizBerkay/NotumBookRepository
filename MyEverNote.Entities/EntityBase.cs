using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEverNote.Entities
{
    public class EntityBase
    {
        public string ModifiedUser { get; set;  }
        public DateTime ModifieOn { get; set; }
        public DateTime CreateOn { get; set; }
    }
}
