namespace CsvModels
{
    public class ProductCsv
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
}