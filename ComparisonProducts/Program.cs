using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvModels;
using CsvSharp;

namespace ComparisonProducts
{
    internal class Program
    {
        class ComparerProducts : IEqualityComparer<ProductCsv>
        {
            public bool Equals(ProductCsv x, ProductCsv y)
            {
                return (x.sku_name.Equals(y.sku_name)
                        && x.sku.Equals(y.sku)
                        && x.code.Equals(y.code));
            }

            public int GetHashCode(ProductCsv obj)
            {
                return obj.code.GetHashCode() + obj.sku_name.GetHashCode() ^ obj.sku.GetHashCode();
            }
        }

        public static void Main(string[] args)
        {
            //if (args == null && args.Length > 1 && (args[0] == null || args[1] == null)) return;

            var file_old = /* args[0] ??*/ "distinctProduct_old.csv";
            var file_new = /* args[1] ??*/ "distinctProduct_new.csv";
            var separator =/* args[2] ??*/ ";";

            var lines_old = File.ReadAllLines(file_old);
            var lines_new = File.ReadAllLines(file_new);
            
            var listNotPassedLine = new List<string>();

            // Processing old products
            var listProducts_old = ConvertProduct(lines_old, listNotPassedLine, separator);
            // Processing new products
            var listProducts_new = ConvertProduct(lines_new, listNotPassedLine, separator);

            var exceprtProducts = listProducts_old.Except(listProducts_new, new ComparerProducts());
            Console.WriteLine(exceprtProducts.Count());
            File.WriteAllText("delta.csv", CsvConvert.Serialize(exceprtProducts, CultureInfo.InvariantCulture, separator));
        }

        private static List<ProductCsv> ConvertProduct(string[] items, List<string> notPassedLines, string separator)
        {
            var result = new List<ProductCsv>();
            foreach (var item in items)
            {
                try
                {
                    var csv = CsvConvert.Deserialize<ProductCsv>(item, CultureInfo.InvariantCulture, separator);
                    if (csv?.Count() == 1)
                    {
                        var temp = csv.ToList()[0];
                        if (temp.TryParseNumber())
                            result.Add(temp.Trim());
                        else
                            notPassedLines.Add(item);
                    }
                }
                catch (Exception e)
                {
                    notPassedLines.Add(item);
                }
            }

            return result;
        }
    }
}