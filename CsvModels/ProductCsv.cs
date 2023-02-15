using CsvSharp.Attributes;

namespace CsvModels
{
    public class ProductCsv
    {
        private static int LengthSku = 8;
        public string sku_name { get; set; }
        public string sku { get; set; }

        public string code { get; set; }
         

        public string segment_code { get; set; }

        public string segment_name { get; set; }
        
        [CsvIgnore]
        public long BarCode {
            get
            {
                if (string.IsNullOrEmpty(code)) return 0;
                if (long.TryParse(code, out long _barCode))
                {
                    return _barCode;
                }

                return 0;
            }} 


        public ProductCsv Trim()
        {
            sku_name = sku_name.Trim();
            sku_name = sku_name.Replace("\"", string.Empty);
            
            sku = sku.Trim();
            sku = sku.Replace("\\", string.Empty).Replace("\"", string.Empty);

            code = code.Trim();
            code = code.Replace("\"", string.Empty);

            segment_code = segment_code?.Trim();
            segment_code = GetSegmentNumber();
            
            segment_name = segment_name?.Trim();

            return this;
        }

        public bool TryParseNumber()
        {
            return int.TryParse(sku.Trim(), out var _) && long.TryParse(code.Trim(), out var _) && int.TryParse(segment_code, out var _);
        }

        public string GetSegmentNumber()
        {
            if (string.IsNullOrEmpty(segment_code)) return "";
            if (int.TryParse(segment_code, out var result))
            {
                return result.ToString();
            }

            return "";
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

            return (o.GetSku().Equals(GetSku()) && o.BarCode.Equals(BarCode));
            //return o.sku.Equals(sku) && o.code.Equals(code);
        }

        public override int GetHashCode()
        {
            var skuCode = sku.GetHashCode();
            var codeCode = code.GetHashCode();
            //return skuCode ^ codeCode;
            return GetSku().GetHashCode() ^ BarCode.GetHashCode();
        }

        public override string ToString()
        {
            return $"Name: {sku_name}, Art: {GetSku()}, Code: {code}";
        }
    }
}