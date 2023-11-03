using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Classes
{
    public class CSV
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public bool Searchable { get; set; }
        
        public bool Filtable { get; set; }

        public bool Visible { get; set; }
    }
}
