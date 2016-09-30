using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace GEDCOMConverter
{

    public class Individual
    {
        private List<Relation> pRelations = new List<Relation>();
        private List<GenericProperty> pProperties = new List<GenericProperty>();

        public Individual()
        {
            ID = string.Empty;
            FullName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            BirthDate = string.Empty;
            BirthLocation = string.Empty;
            IsDeceased = string.Empty;
            DeceasedDate = string.Empty;
            Gender = string.Empty;


        }

        public string ID { get; set; }


        public string FullName { get; set; }


        public string FirstName { get; set;  }


        public string LastName { get; set; }


        public string BirthDate { get; set; }


        public string BirthLocation { get; set; }


        public string IsDeceased { get; set; }


        public string DeceasedDate { get; set; }


        public string Gender { get; set; }

        public object[] Relatives
        {
            get
            {

                return Relations.Select(p =>  new { p.Type, p.Subject.ID, p.Subject.FullName, p.Subject.FirstName, p.Subject.LastName, p.Subject.BirthDate, p.Subject.Gender, p.Subject.IsDeceased, p.Subject.DeceasedDate }).ToArray();

            }
        }

        [JsonIgnore]
        public List<Relation> Relations
        {
            get { return pRelations;  }
        }

        [JsonIgnore]
        public List<GenericProperty> Properties
        {
            get { return pProperties; }
        }


        [JsonIgnore]
        public bool IsValid
        {
            get { return ID != string.Empty && FullName != string.Empty; }
        }

        public static string GetCSVHeader()
        {
            return "ID,First Name,Last Name, Birth Date,Birth Location,Gender,Mother,Father";
        }

        public string GetAsCSV()
        {
            var s = ID.Replace(",", "") + "," + FirstName.Replace(",", "") + "," + LastName.Replace(",", "") + "," + BirthDate.Replace(",", "") + "," + BirthLocation.Replace(",", "") + "," + Gender.Replace(",", "") + ",";
            var m = Relations.Find(p => p.Type == "MOTHER");
            if (m != null)
                s += m.Subject.FullName.Replace(",", "") + ",";

            var f = Relations.Find(p => p.Type == "FATHER");
            if (f != null)
                s += f.Subject.FullName.Replace(",", "");

            return s;
        }

    }


   


   

     


}
