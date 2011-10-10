using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bendb.Collections
{
    /// <summary>
    /// Represents a sequence of potentially computationally-expensive elements
    /// whose results are cached in memory upon execution such that repeated
    /// enumeration only incurs the cost of the computations once.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <threadsafety>
    /// Not thread-safe at all.  Don't say I didn't warn you.
    /// </threadsafety>
    public class MemoizingEnumerable<T> : IEnumerable<T>
    {
        private readonly List<T> results;
        private IEnumerable<T> source;
        private bool isFirstRun = true;

        public MemoizingEnumerable(IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            this.source = source;
            results = new List<T>();
        }

        private IEnumerator<T> GetSourceEnumerator()
        {
            foreach (var item in source)
            {
                results.Add(item);
                yield return item;
            }

            // We don't want to hold on to source any longer than necessary.
            isFirstRun = false;
            source = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return isFirstRun ? GetSourceEnumerator() : results.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class MemoizingEnumerableExtensions
    {
        /// <summary>
        /// Wraps a <paramref name="collection"/> of items in a memoizing
        /// container such that repeatedly enumerating the collection only
        /// incurs the cost of computing the elements once.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements contained in
        /// <paramref name="collection"/>.
        /// </typeparam>
        /// <param name="collection">
        /// The collection whose elements are to be memoized.
        /// </param>
        /// <returns>
        /// Returns an <see cref="IEnumerable&lt;T&gt;"/> which caches the
        /// elements of the source <see cref="collection"/> in memory upon
        /// the first enumeration.
        /// </returns>
        public static IEnumerable<T> WithMemoization<T>(this IEnumerable<T> collection)
        {
            return new MemoizingEnumerable<T>(collection);
        }
    }
}
