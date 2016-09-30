using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDCOMConverter
{
    public class Relation
    {

        public Relation()
        {
            this.Type = string.Empty;
            this.Description = string.Empty;
        }

        public string Type { get; set; }

        public string Description { get; set; }

        public Individual Subject { get; set; }

    }

}
