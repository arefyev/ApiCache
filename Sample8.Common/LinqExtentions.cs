using System.Collections.Generic;
using System.Linq;

namespace Sample8Common
{
    public static class LinqExtentions
    {
        /// <summary>
        /// Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrderByName<T>(this IEnumerable<T> enumerable, string property)
        {
            return enumerable.OrderBy(x => GetProperty(x, property));
        }
        /// <summary>
        /// Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrderByDescendingName<T>(this IEnumerable<T> enumerable, string property)
        {
            return enumerable.OrderByDescending(x => GetProperty(x, property));
        }

        private static object GetProperty(object o, string propertyName)
        {
            var properties = o.GetType().GetProperties();

            var pro = properties.First(x => x.Name.ToLower() == propertyName.ToLower());
            return pro.GetValue(o, null);
        }
    }
}
