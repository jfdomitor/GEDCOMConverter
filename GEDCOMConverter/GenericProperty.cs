using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GEDCOMConverter
{
    public class GenericProperty
    {

        public GenericProperty()
        {
            this.Type = string.Empty;
            this.PropertyName = string.Empty;
            this.PropertyValue = string.Empty;
        }

        public string Type { get; set; }

        public string PropertyName { get; set; }

        public string PropertyValue { get; set; }
    }
}
