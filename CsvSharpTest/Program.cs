using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvSharp;


namespace CsvSharpTest
{
    class ProductCsv
    {
        private static int LengthSku = 8;
        public string sku_name { get; set; }
        public string sku { get; set; }

        public string code { get; set; }


        public ProductCsv Trim()
        {
            sku_name = sku_name.Trim();
            sku_name = sku_name.Replace("\"", string.Empty);
            
            sku = sku.Trim();
            sku = sku.Replace("\\", string.Empty).Replace("\"", string.Empty);

            code = code.Trim();
            code = code.Replace("\"", string.Empty);

            return this;
        }

        public bool TryParseNumber()
        {
            Trim();
            
            return int.TryParse(sku, out var _) && long.TryParse(code, out var _);
        }

        public string GetSku()
        {
            
            if (int.TryParse(sku, out var result))
            {
                return result.ToString($"D{LengthSku}");
            }
            return sku;
        }

        public override bool Equals(object obj)
        {
            var o = obj as ProductCsv;
            if (o==null) return false;

            return (o.sku.Equals(sku) && o.code.Equals(code));
        }

        public override int GetHashCode()
        {
            var skuCode = sku.GetHashCode();
            var codeCode = code.GetHashCode();
            return skuCode ^ codeCode;
        }

        public override string ToString()
        {
            return $"Name: {sku_name}, Art: {GetSku()}, Code: {code}";
        }
    }

    internal class Program
    {
        public static IEnumerable<IEnumerable<T>> Partition<T>(IEnumerable<T> instance, int partitionSize)
        {
            return instance
                .Select((value, index) => new { Index = index, Value = value })
                .GroupBy(i => i.Index / partitionSize)
                .Select(i => i.Select(i2 => i2.Value));
        }
        

        static void Main(string[] args)
        {
            if (args[0] == null) return;            
            
            var file = args[0];
            var partTemp = args[1] ?? "50000";
            var separator = args[2] ?? ";";
            var head = args[3] ?? "false";
            
            var skipHead = 0;
            if (head.Equals("true"))
                skipHead = 1;
            var part = int.Parse(partTemp);
            //var part = 50000;
            //var lines = File.ReadAllLines(@"C:\Users\Разработчик#1\Downloads\products.csv");
            var lines = File.ReadAllLines(file);
            var listProduct = new List<ProductCsv>(lines.Length);
            var index1 = 1;
            var listNotPassedLine = new List<string>();
            foreach (var item in lines.Skip(skipHead))
            {
                index1++;
                try
                {
                    var csv = CsvConvert.Deserialize<ProductCsv>(item, CultureInfo.InvariantCulture, separator);
                    if (csv?.Count() == 1)
                    {
                        var temp = csv.ToList()[0];
                        if (temp.TryParseNumber())
                            listProduct.Add(temp.Trim());
                        else
                            listNotPassedLine.Add(item);
                    }
                }
                catch
                {
                    listNotPassedLine.Add(item);
                }
            }
            //listProduct.ForEach((x) => Console.WriteLine(x));
            
            Console.WriteLine($"Products read {listProduct.Count()} count");
            var distinctList = listProduct.Distinct();
            Console.WriteLine($"Distincts products count: {distinctList.Count()}");
            
            //var checkedDistinctProducts = distinctList.Where(item => item.TryParseNumber());
            
            var splitsList = Partition(distinctList, part);
            Console.WriteLine($"Parts: {splitsList.Count()}");
            var index2 = 1;
            foreach (var item in splitsList)
            {
                var fileName = $"splitCSV_part{index2}.csv";
                index2++;
                File.WriteAllText(fileName, CsvConvert.Serialize(item, CultureInfo.InvariantCulture, separator));
                Console.WriteLine($"Created part: {fileName} rows: {item.Count()}");
            }
            Console.WriteLine(new string('=', 20));
            if (listNotPassedLine.Count > 0)
            {
                Console.WriteLine("Lines not parse sku and code to number {0}", listNotPassedLine.Count);
                var filenameNotPassed = "notPassedline.csv";
                File.WriteAllLines(filenameNotPassed, listNotPassedLine.ToArray());
                Console.WriteLine($"Saved not passed lines: {filenameNotPassed}");
            }
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
