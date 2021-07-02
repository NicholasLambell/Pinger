using System;
using System.Collections.Generic;
using System.Linq;

namespace Pinger.Extensions {
    public static class EnumerableExtensions {
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> collection, int count) {
            // ReSharper disable once PossibleMultipleEnumeration
            return collection.Skip(Math.Max(0, collection.Count() - count));
        }
    }
}