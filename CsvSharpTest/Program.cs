using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvSharp;
using CsvModels;




namespace CsvSharpTest
{
    internal class Program
    {
        
        

        static void Main(string[] args)
        {
            if (args[0] == null) return;            
            
            var file = args[0];
            var partTemp = args[1] ?? "50000";
            var separator = ";";//args[2] ?? ";";
            var head = args[3] ?? "false";
            //var isDistinctOnly = args[4] ?? "false";
            //var isCompareArtAndName = args[5] ?? "false";
            
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
            
            

            /*
            if (!isDistinctOnly.Equals("false"))
            {
                File.WriteAllText("distinctProduct.csv", CsvConvert.Serialize(distinctList, CultureInfo.InvariantCulture, separator));
                ListNotPassedProcessing(listNotPassedLine);
                if (!isCompareArtAndName.Equals("false"))
                {
                    //var comparerProduct = new List<ProductCsv>();
                    var comparerNameAndSku = new List<ProductCsv>();
                    var groupBySku = distinctList.GroupBy(p => p.sku);
                    foreach (var item in groupBySku)
                    {
                        if (item.Count() > 1)
                        {
                            var list = item.ToList();
                            if (!list[0].sku_name.Equals(list[1].sku_name) && list[0].sku.Equals(list[1].sku))
                            {
                                comparerNameAndSku.AddRange(item);
                            }
                            
                            //comparerProduct.AddRange(item);
                        }
                    }
                    File.WriteAllText("comparerNameAndSku.csv", CsvConvert.Serialize(comparerNameAndSku, CultureInfo.InvariantCulture, separator));
                    return;
                }
                return;
            }
            */
            var splitsList = distinctList.Partition(part);
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
            ListNotPassedProcessing(listNotPassedLine);
            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        private static void ListNotPassedProcessing(List<string> listNotPassedLine)
        {
            if (listNotPassedLine.Count > 0)
            {
                Console.WriteLine("Lines not parse sku and code to number {0}", listNotPassedLine.Count);
                var filenameNotPassed = "notPassedline.csv";
                File.WriteAllLines(filenameNotPassed, listNotPassedLine.ToArray());
                Console.WriteLine($"Saved not passed lines: {filenameNotPassed}");
            }
        }
    }

    internal class CompareProductArtAndName : IEqualityComparer<ProductCsv>
    {
        public bool Equals(ProductCsv x, ProductCsv y)
        {
            var a = x.sku.Equals(y.sku);
            var b = !x.sku_name.Equals(y.sku_name);
            if (x.code.Contains("2332311000000") || x.code.Contains("2332311000003"))
            {
                var t = 0;
            }
            if (a && b)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(ProductCsv obj)
        {
            return obj.sku.GetHashCode() * obj.sku_name.GetHashCode();
        }
    }
}
