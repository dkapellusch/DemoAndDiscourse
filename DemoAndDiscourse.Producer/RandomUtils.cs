using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoAndDiscourse.Producer
{
    public static class RandomUtils
    {
        private static readonly char[] VinChars =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K',
            'L', 'M', 'N', 'P', 'R', 'S', 'T', 'V', 'W', 'X',
            'Y', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        public static readonly Random RandomGenerator = new Random();

        public static string RandomVin => string.Join("", Enumerable.Range(0, 17).Select(_ => VinChars.RandomChoice()));

        public static DateTime RandomDateTime =>
            DateTime.UtcNow
                .AddYears(RandomInt(-100, 0))
                .AddMonths(RandomInt(-12, 0))
                .AddDays(RandomInt(-365, 0))
                .AddHours(RandomInt(-24, 0))
                .AddMinutes(RandomInt(-60, 0))
                .AddSeconds(RandomInt(-60, 0))
                .AddMilliseconds(RandomInt(-1000, 0));

        public static T RandomChoice<T>(this ICollection<T> collection) => collection.Skip(RandomGenerator.Next(0, collection.Count)).First();

        public static int RandomInt(int min, int maxInclusive) => RandomGenerator.Next(min, maxInclusive + 1);

        public static double RandomDouble(double min, double max) => RandomGenerator.NextDouble() * (max - min) + min;
    }
}