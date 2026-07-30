namespace Textile.Core.Entities
{
    public static class Helpers
    {
        private static readonly Random _random = new Random();

        public static string GenerateRandomPrefixedSuffixedNumber(this int baseNumber)
        {
            int prefix = _random.Next(1, 10); // 1-digit random
            int suffix = _random.Next(1, 10); // 1-digit random

            return $"{prefix}{baseNumber}{suffix}";
        }

        public static int RoundFF(this int value)
        {
            int intValue = value;
            int lastDigit = Math.Abs(intValue) % 10;
            int result;

            if (lastDigit >= 1 && lastDigit <= 4)
            {
                result = intValue - lastDigit;
            }
            else if (lastDigit >= 5 && lastDigit <= 9)
            {
                result = intValue + (10 - lastDigit);
            }
            else
            {
                result = intValue; // when last digit = 0
            }

            return result;
        }
    }

}
