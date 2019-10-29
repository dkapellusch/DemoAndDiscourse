using System.Collections.Generic;

namespace DemoAndDiscourse.Utils
{
    public static class Extensions
    {
        public static bool IsNullOrDefault<T>(this T possiblyNullOrDefaultObject)
            => possiblyNullOrDefaultObject is null || EqualityComparer<T>.Default.Equals(possiblyNullOrDefaultObject, default);

        public static bool IsNotNullOrDefault<T>(this T possiblyNullOrDefaultObject) => !possiblyNullOrDefaultObject.IsNullOrDefault();
    }
}