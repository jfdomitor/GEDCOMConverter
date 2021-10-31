using System;
using GEDCOMConverter;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            try {

                GEDCOMParser.LoadGEDCOMFile("test.ged");

                Console.WriteLine("Create excel file (J/N)");
                var k = Console.ReadKey();
                if (k.Key == ConsoleKey.J)
                    GEDCOMParser.CreateCSVFile("test_out.csv");

                Console.Clear();
                Console.WriteLine("Create JSON file (J/N)");
                k = Console.ReadKey();
                if (k.Key == ConsoleKey.J)
                    GEDCOMParser.CreateJSONFile("test_out.json");

                Console.Clear();
                Console.WriteLine("List individuals (J/N)");
                k = Console.ReadKey();
                if (k.Key == ConsoleKey.J)
                {


                    var l = GEDCOMParser.GetIndividuals();
                    foreach (var i in l)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Individual " + i.ID);
                        Console.WriteLine("Name: " + i.FullName);
                        Console.WriteLine("First Name: " + i.FirstName);
                        Console.WriteLine("Last Name: " + i.LastName);
                        Console.WriteLine("Gender: " + i.Gender);
                        Console.WriteLine("Birth Date: " + i.BirthDate);
                        Console.WriteLine("Birth Location: " + i.BirthLocation);
                        Console.WriteLine("Is Deceased: " + i.IsDeceased);
                        Console.WriteLine("Deceased Date: " + i.DeceasedDate);


                        foreach (var r in i.Relations)
                        {
                            Console.WriteLine(r.Type + " " + r.Subject.FullName);
                        }

                        Console.WriteLine();
                        Console.WriteLine();

                        foreach (var r in i.Properties)
                        {
                            Console.WriteLine(r.Type + " " + r.PropertyName + " " + r.PropertyValue);
                        }

                        //Console.ReadKey();
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }
    }
}
