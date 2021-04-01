using System.Collections.Generic;

namespace Essentials
{
    public static class ArrayExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> collection, T search)
        {
            var index = 0;
            foreach (var element in collection)
            {
                if (Equals(element, search)) return index;
                index++;
            }

            return -1;
        }
    }
}