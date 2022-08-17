namespace Validation
{
    public static class NumberValidation
    {
        public static bool IsInt(string input)
        {
            return int.TryParse(input, out _);
        }
        public static bool IsDecimal(string input)
        {
            return decimal.TryParse(input, out _);
        }
        public static bool IsDouble(string input)
        {
            return double.TryParse(input, out _);
        }
    }
}
