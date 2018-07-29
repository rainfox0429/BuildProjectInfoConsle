namespace Utilities.Extension
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The CollectionExtension class
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// Determines whether [is null or empty].
        /// </summary>
        /// <typeparam name="T"> The collection type </typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified collection]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// Check current collection is null or empty
        /// </summary>
        /// <typeparam name="T"> The collection type </typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified collection]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

    }
}
