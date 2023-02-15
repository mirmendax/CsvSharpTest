using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvSharp;

namespace NewCsvTest
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var fileLines = File.ReadAllLines("181.csv");
            foreach (var item in fileLines)
            {
                var csv = CsvConvert.Deserialize<ContactDTO>(item, CultureInfo.InvariantCulture);
                if (csv.Count() > 0)
                {
                    var c = csv.ToList().FirstOrDefault();
                    if (c != null)
                        Console.WriteLine(c);
                    else
                        Console.WriteLine("Error " + item);
                }
            }
        }
    }

    public class ContactDTO
    {
        public string FIO { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string ContactId { get; set; }
        public string Date { get; set; }

        public override string ToString()
        {
            return $"{nameof(FIO)}: {FIO}, {nameof(Phone)}: {Phone}, {nameof(Description)}: {Description}, {nameof(ContactId)}: {ContactId}, {nameof(Date)}: {Date}";
        }
    }
}