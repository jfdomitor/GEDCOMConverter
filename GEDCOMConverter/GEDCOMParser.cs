using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace GEDCOMConverter
{
    public static class GEDCOMParser
    {
        private static string[] pGEDCOMSections = null;
        private static List<Individual> pIndividuals = null;

        public static void LoadGEDCOMFile(string path_and_filename)
        {

           StreamReader SR = new StreamReader(@path_and_filename);
           pGEDCOMSections = SR.ReadToEnd().Replace("0 @", "\u0646").Split('\u0646');
           pIndividuals = new List<Individual>();

            //SET INDIVIDUALS
            foreach (string rec in pGEDCOMSections)
            {

                string[] sub_records = rec.Replace("\r\n", "\r").Split('\r');

                if (sub_records[0].Contains("INDI"))
                {
                    Individual ind = new Individual();

                    ind.ID = sub_records[0].Substring(0, sub_records[0].IndexOf(" ") - 1);

                    string parent_tagname = string.Empty;
                    string tagtype = string.Empty;


                    foreach (var info in sub_records)
                    {
                        if (!IsValidLine(info))
                            continue;

                        var p = new GenericProperty();
                        p.Type = "GEDCOM";

                        //FIND END INDEX OF TAGNAME
                        var endoftag = -1;
                        if (info.LastIndexOf(" ") < 4)
                        {
                            tagtype = "HEADERTAG";
                            endoftag = info.Length;
                        }
                        else
                        {
                            tagtype = "VALUETAG";
                            endoftag = info.IndexOf(" ", 3);
                        }

                        if (endoftag < 0)
                            continue;

                        p.PropertyName = info.Substring(0, endoftag);

                        if (info.Length > endoftag)
                            p.PropertyValue = info.Substring(endoftag + 1).Replace("/", "").Trim();



                        if (parent_tagname == string.Empty)
                            parent_tagname = p.PropertyName;
                        else
                        {
                            try
                            {
                                if (Convert.ToInt32(p.PropertyName.Substring(0, 1)) <= Convert.ToInt32(parent_tagname.Substring(0, 1)))
                                {
                                    parent_tagname = p.PropertyName;
                                }
                            }
                            catch { }
                        }

                        ind.Properties.Add(p);

                        if (p.PropertyName == "1 NAME")
                            ind.FullName = p.PropertyValue;

                        if (p.PropertyName == "2 GIVN")
                            ind.FirstName = p.PropertyValue;

                        if (p.PropertyName == "2 SURN")
                            ind.LastName = p.PropertyValue;

                        if (p.PropertyName == "1 DEAT")
                            ind.IsDeceased = p.PropertyValue;

                        if (p.PropertyName == "1 SEX")
                            ind.Gender = p.PropertyValue;

                        if (p.PropertyName == "2 DATE" && parent_tagname == "1 BIRT")
                            ind.BirthDate = p.PropertyValue;

                        if (p.PropertyName == "2 PLAC" && parent_tagname == "1 BIRT")
                            ind.BirthLocation = p.PropertyValue;

                        if (p.PropertyName == "2 DATE" && parent_tagname == "1 DEAT")
                            ind.DeceasedDate = p.PropertyValue;

                    }



                    if (ind.IsValid)
                        pIndividuals.Add(ind);
                }

            }



            //SET RELATIONS FOR INDIVIDUALS
            foreach (string rec in pGEDCOMSections)
            {

                string[] fam_lines = rec.Replace("\r\n", "\r").Split('\r');

                if (fam_lines[0].Contains("FAM"))
                {
                    string parent_tagname = string.Empty;
                    string tagtype = string.Empty;

                    var pl = new List<GenericProperty>();

                    foreach (var info in fam_lines)
                    {
                        if (!IsValidLine(info))
                            continue;

                        var gp = new GenericProperty();

                        //FIND END INDEX OF TAGNAME
                        var endoftag = -1;
                        if (info.LastIndexOf(" ") < 4)
                        {
                            tagtype = "HEADERTAG";
                            endoftag = info.Length;
                        }
                        else
                        {
                            tagtype = "VALUETAG";
                            endoftag = info.IndexOf(" ", 3);
                        }

                        if (endoftag < 0)
                            continue;

                        gp.PropertyName = info.Substring(0, endoftag);
                        if (info.Length > endoftag)
                            gp.PropertyValue = info.Substring(endoftag + 1).Replace("/", "").Replace("@", "").Trim();

                        pl.Add(gp);

                    }


                    foreach (var prop in pl)
                    {

                        if (prop.PropertyName == "1 HUSB")
                        {

                            var husb = pIndividuals.Find(p => p.ID == prop.PropertyValue);
                            if (husb != null)
                            {
                                var wife_prop = pl.Find(p => p.PropertyName == "1 WIFE");
                                if (wife_prop != null)
                                {

                                    var wife = pIndividuals.Find(p => p.ID == wife_prop.PropertyValue);
                                    if (wife != null)
                                    {
                                        var rel = new Relation();
                                        rel.Type = "HUSB";
                                        rel.Subject = husb;
                                        wife.Relations.Add(rel);

                                        rel = new Relation();
                                        rel.Type = "WIFE";
                                        rel.Subject = wife;
                                        husb.Relations.Add(rel);

                                    }
                                }
                            }
                        }


                        if (prop.PropertyName == "1 CHIL")
                        {
                            var child = pIndividuals.Find(p => p.ID == prop.PropertyValue);
                            if (child != null)
                            {

                                var mother_prop = pl.Find(p => p.PropertyName == "1 WIFE");
                                if (mother_prop != null)
                                {
                                    var mother = pIndividuals.Find(p => p.ID == mother_prop.PropertyValue);
                                    if (mother != null)
                                    {
                                        var rel = new Relation();
                                        rel.Type = "CHILD";
                                        rel.Subject = child;
                                        mother.Relations.Add(rel);

                                        rel = new Relation();
                                        rel.Type = "MOTHER";
                                        rel.Subject = mother;
                                        child.Relations.Add(rel);

                                    }
                                }

                                var father_prop = pl.Find(p => p.PropertyName == "1 HUSB");
                                if (father_prop != null)
                                {
                                    var father = pIndividuals.Find(p => p.ID == father_prop.PropertyValue);
                                    if (father != null)
                                    {
                                        var rel = new Relation();
                                        rel.Type = "CHILD";
                                        rel.Subject = child;
                                        father.Relations.Add(rel);

                                        rel = new Relation();
                                        rel.Type = "FATHER";
                                        rel.Subject = father;
                                        child.Relations.Add(rel);

                                    }
                                }

                            }
                        }
                    }
                }
            }

            SR.Close();
        }

        public static List<Individual> GetIndividuals()
        {
            return pIndividuals;
        }

        public static void CreateJSONFile(string path_and_filename)
        {

            var l = GetIndividuals();

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(@path_and_filename))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, l.AsEnumerable());
            }

        }

        public static void CreateCSVFile(string path_and_filename)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Individual.GetCSVHeader());
            var l = GetIndividuals();
            foreach (var i in l)
            {
                sb.AppendLine(i.GetAsCSV());
            }

            System.IO.File.WriteAllText(@path_and_filename, sb.ToString(), Encoding.Unicode);

        }



        private static bool IsValidLine(string line)
        {
            if (line == null)
                return false;

            if (line == string.Empty)
                return false;

            if (line.Length < 3)
                return false;


            int level = 0;
            bool canConvert = int.TryParse(line.Substring(0, 1), out level);
            if (!canConvert)
                return false;


            return true;

        }



    }

}

