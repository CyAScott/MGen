using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MGen
{
    /// <summary>
    /// Exention methods for collections.
    /// </summary>
    public static class CollectionHelper
    {
        static IEnumerable<KeyValuePair<object?, object?>> AsEnumerable(this IDictionaryEnumerator dictionaryEnumerator)
        {
            while (dictionaryEnumerator.MoveNext())
            {
                yield return new KeyValuePair<object?, object?>(dictionaryEnumerator.Key, dictionaryEnumerator.Value);
            }
        }

        static IEnumerable<KeyValuePair<object?, object?>> AsEnumerable(this IEnumerator enumerable)
        {
            while (enumerable.MoveNext())
            {
                yield return new KeyValuePair<object?, object?>(null, enumerable.Current);
            }
        }

        static IEnumerable<KeyValuePair<object?, object?>> AsEnumerable(this NameValueCollection nameValueCollection)
        {
            foreach (var key in nameValueCollection.AllKeys)
            {
                yield return new KeyValuePair<object?, object?>(key, nameValueCollection[key]);
            }
        }

        /// <summary>
        /// Attempts to get an <see cref="IEnumerable"/> from an object that might be a collection.
        /// </summary>
        public static bool TryToGetEnumerable(this object collection, out IEnumerable<KeyValuePair<object?, object?>>? enumerable)
        {
            if (collection is NameValueCollection nameValueCollection)
            {
                enumerable = nameValueCollection.AsEnumerable();
                return true;
            }

            if (collection is IDictionary dictionary)
            {
                enumerable = dictionary.GetEnumerator().AsEnumerable();
                return true;
            }

            if (collection is not IEnumerable collectionAsEnumerable)
            {
                enumerable = null;
                return false;
            }

            var enumerator = collectionAsEnumerable.GetEnumerator();

            if (enumerator is IDictionaryEnumerator dictionaryEnumerator)
            {
                enumerable = dictionaryEnumerator.AsEnumerable();
                return true;
            }

            enumerable = enumerator.AsEnumerable();

            return true;
        }
    }
}
