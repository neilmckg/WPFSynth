using System;
using System.Collections.Generic;
using System.Linq;

namespace Synth.Util
{
    public static class EnumerableExtensions
    {
        public static void Execute<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source != null)
            {
                foreach (T item in source.ToArray())
                    action(item);
            }
        }
    }
}
