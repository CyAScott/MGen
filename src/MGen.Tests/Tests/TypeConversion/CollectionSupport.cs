using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace MGen.Tests.TypeConversion
{
    [Generate]
    public interface ICollectionConversion : ISupportConversion
    {
        BlockingCollection<int> GenericBlockingCollection { get; set; }
        Collection<int> GenericCollection { get; set; }
        ConcurrentBag<int> GenericConcurrentBag { get; set; }
        ConcurrentDictionary<int, string> GenericConcurrentDictionary { get; set; }
        ConcurrentQueue<int> GenericConcurrentQueue { get; set; }
        ConcurrentStack<int> GenericConcurrentStack { get; set; }
        Dictionary<int, string> GenericDictionary { get; set; }
        Hashtable Hashtable { get; set; }
        ICollection<int> GenericCollectionInterface { get; set; }
        IDictionary<int, string> GenericDictionaryInterface { get; set; }
        IDictionary DictionaryInterface { get; set; }
        IEnumerable<int> GenericEnumerableInterface { get; set; }
        IList<int> GenericListInterface { get; set; }
        IOrderedDictionary OrderedDictionaryInterface { get; set; }
        IReadOnlyCollection<int> GenericReadOnlyCollectionInterface { get; set; }
        IReadOnlyDictionary<int, string> GenericReadOnlyDictionaryInterface { get; set; }
        IReadOnlyList<int> GenericReadOnlyListInterface { get; set; }
        LinkedList<int> GenericLinkedList { get; set; }
        List<int> GenericList { get; set; }
        NameValueCollection NameValueCollection { get; set; }
        ObservableCollection<int> GenericObservableCollection { get; set; }
        OrderedDictionary OrderedDictionary { get; set; }
        Queue<int> GenericQueue { get; set; }
        Queue Queue { get; set; }
        ReadOnlyCollection<int> GenericReadOnlyCollection { get; set; }
        ReadOnlyObservableCollection<int> GenericReadOnlyObservableCollection { get; set; }
        SortedDictionary<int, string> GenericSortedDictionary { get; set; }
        SortedList<int, string> GenericSortedList { get; set; }
        SortedList SortedList { get; set; }
        SortedSet<int> GenericSortedSet { get; set; }
        Stack<int> GenericStack { get; set; }
        Stack Stack { get; set; }
        StringCollection StringCollection { get; set; }
        StringDictionary StringDictionary { get; set; }
    }

    [Generate]
    public interface ICollectionAsStringsConversion : ISupportConversion
    {
        BlockingCollection<string> GenericBlockingCollection { get; set; }
        Collection<string> GenericCollection { get; set; }
        ConcurrentBag<string> GenericConcurrentBag { get; set; }
        ConcurrentDictionary<string, int> GenericConcurrentDictionary { get; set; }
        ConcurrentQueue<string> GenericConcurrentQueue { get; set; }
        ConcurrentStack<string> GenericConcurrentStack { get; set; }
        Dictionary<string, int> GenericDictionary { get; set; }
        Hashtable Hashtable { get; set; }
        ICollection<string> GenericCollectionInterface { get; set; }
        IDictionary<string, int> GenericDictionaryInterface { get; set; }
        IDictionary DictionaryInterface { get; set; }
        IEnumerable<string> GenericEnumerableInterface { get; set; }
        IList<string> GenericListInterface { get; set; }
        IOrderedDictionary OrderedDictionaryInterface { get; set; }
        IReadOnlyCollection<string> GenericReadOnlyCollectionInterface { get; set; }
        IReadOnlyDictionary<string, int> GenericReadOnlyDictionaryInterface { get; set; }
        IReadOnlyList<string> GenericReadOnlyListInterface { get; set; }
        LinkedList<string> GenericLinkedList { get; set; }
        List<string> GenericList { get; set; }
        NameValueCollection NameValueCollection { get; set; }
        ObservableCollection<string> GenericObservableCollection { get; set; }
        OrderedDictionary OrderedDictionary { get; set; }
        Queue<string> GenericQueue { get; set; }
        Queue Queue { get; set; }
        ReadOnlyCollection<string> GenericReadOnlyCollection { get; set; }
        ReadOnlyObservableCollection<string> GenericReadOnlyObservableCollection { get; set; }
        SortedDictionary<string, int> GenericSortedDictionary { get; set; }
        SortedList<string, int> GenericSortedList { get; set; }
        SortedList SortedList { get; set; }
        SortedSet<string> GenericSortedSet { get; set; }
        Stack<string> GenericStack { get; set; }
        Stack Stack { get; set; }
        StringCollection StringCollection { get; set; }
        StringDictionary StringDictionary { get; set; }
    }

    public class CollectionSupport
    {
        public void Init(ICollectionConversion instance)
        {
            var values = 0;

            instance.DictionaryInterface = new Hashtable
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.GenericBlockingCollection = new BlockingCollection<int>
            {
                values++, values++, values++
            };
            instance.GenericCollection = new Collection<int>
            {
                values++, values++, values++
            };
            instance.GenericConcurrentBag = new ConcurrentBag<int>
            {
                values++, values++, values++
            };
            instance.GenericConcurrentDictionary = new ConcurrentDictionary<int, string>();
            instance.GenericConcurrentDictionary[values++] = values++.ToString();
            instance.GenericConcurrentDictionary[values++] = values++.ToString();
            instance.GenericConcurrentDictionary[values++] = values++.ToString();
            instance.GenericConcurrentQueue = new ConcurrentQueue<int>();
            instance.GenericConcurrentQueue.Enqueue(values++);
            instance.GenericConcurrentQueue.Enqueue(values++);
            instance.GenericConcurrentQueue.Enqueue(values++);
            instance.GenericConcurrentStack = new ConcurrentStack<int>();
            instance.GenericConcurrentStack.Push(values++);
            instance.GenericConcurrentStack.Push(values++);
            instance.GenericConcurrentStack.Push(values++);
            instance.GenericDictionary = new Dictionary<int, string>
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.Hashtable = new Hashtable
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.GenericCollectionInterface = new[]
            {
                values++, values++, values++
            };
            instance.GenericDictionaryInterface = new Dictionary<int, string>
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.GenericEnumerableInterface = new[]
            {
                values++, values++, values++
            };
            instance.GenericListInterface = new[]
            {
                values++, values++, values++
            };
            instance.OrderedDictionaryInterface = new OrderedDictionary
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.GenericReadOnlyCollectionInterface = new[]
            {
                values++, values++, values++
            };
            instance.GenericReadOnlyDictionaryInterface = new Dictionary<int, string>
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.GenericReadOnlyListInterface = new[]
            {
                values++, values++, values++
            };
            instance.GenericLinkedList = new LinkedList<int>();
            instance.GenericLinkedList.AddFirst(new LinkedListNode<int>(values++));
            instance.GenericLinkedList.AddFirst(new LinkedListNode<int>(values++));
            instance.GenericLinkedList.AddFirst(new LinkedListNode<int>(values++));
            instance.GenericList = new List<int>
            {
                values++, values++, values++
            };
            instance.NameValueCollection = new NameValueCollection
            {
                { values++.ToString(), values++.ToString() },
                { values++.ToString(), values++.ToString() },
                { values++.ToString(), values++.ToString() }
            };
            instance.GenericObservableCollection = new ObservableCollection<int>
            {
                values++, values++, values++
            };
            instance.OrderedDictionary = new OrderedDictionary
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.GenericQueue = new Queue<int>();
            instance.GenericQueue.Enqueue(values++);
            instance.GenericQueue.Enqueue(values++);
            instance.GenericQueue.Enqueue(values++);
            instance.Queue = new Queue();
            instance.Queue.Enqueue(values++);
            instance.Queue.Enqueue(values++);
            instance.Queue.Enqueue(values++);
            instance.GenericReadOnlyCollection = new ReadOnlyCollection<int>(new[]
            {
                values++, values++, values++
            });
            instance.GenericReadOnlyObservableCollection = new ReadOnlyObservableCollection<int>(new ObservableCollection<int>
            {
                values++, values++, values++
            });
            instance.GenericSortedDictionary = new SortedDictionary<int, string>
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.GenericSortedList = new SortedList<int, string>
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.SortedList = new SortedList
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.GenericSortedSet = new SortedSet<int>
            {
                values++, values++, values++
            };
            instance.GenericStack = new Stack<int>();
            instance.GenericStack.Push(values++);
            instance.GenericStack.Push(values++);
            instance.GenericStack.Push(values++);
            instance.Stack = new Stack();
            instance.Stack.Push(values++);
            instance.Stack.Push(values++);
            instance.Stack.Push(values++);
            instance.StringCollection = new StringCollection
            {
                values++.ToString(), values++.ToString(), values++.ToString()
            };
            instance.StringDictionary = new StringDictionary
            {
                { values++.ToString(), values++.ToString() },
                { values++.ToString(), values++.ToString() },
                { values++.ToString(), values++.ToString() }
            };
        }

        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<ICollectionConversion>();
            Assert.IsNotNull(type);

            var instanceA = Activator.CreateInstance(type) as ICollectionConversion;
            Assert.IsNotNull(instanceA);

            Init(instanceA);

            var typeAsStrings = AssemblyScanner.FindImplementationFor<ICollectionAsStringsConversion>();
            Assert.IsNotNull(typeAsStrings);

            var instanceB = Convert.ChangeType(instanceA, typeAsStrings) as ICollectionAsStringsConversion;
            AreEqual(instanceA, instanceB);
        }

        public void AreEqual(ICollectionConversion a, ICollectionAsStringsConversion b)
        {
            Assert.IsNotNull(b);
            AreEqual(a.GenericBlockingCollection, b.GenericBlockingCollection);
            AreEqual(a.GenericCollection, b.GenericCollection);
            AreEqual(a.GenericConcurrentBag.OrderBy(it => it), b.GenericConcurrentBag.OrderBy(it => it));
            AreDictionariesEqual(a.GenericConcurrentDictionary, b.GenericConcurrentDictionary);
            AreEqual(a.GenericConcurrentQueue, b.GenericConcurrentQueue);
            AreEqual(a.GenericConcurrentStack, b.GenericConcurrentStack);
            AreDictionariesEqual(a.GenericDictionary, b.GenericDictionary);
            AreDictionariesEqual(a.Hashtable.GetEnumerator(), b.Hashtable.GetEnumerator());
            AreEqual(a.GenericCollectionInterface, b.GenericCollectionInterface);
            AreDictionariesEqual(a.GenericDictionaryInterface, b.GenericDictionaryInterface);
            AreDictionariesEqual(a.DictionaryInterface.GetEnumerator(), b.DictionaryInterface.GetEnumerator());
            AreEqual(a.GenericEnumerableInterface, b.GenericEnumerableInterface);
            AreEqual(a.GenericListInterface, b.GenericListInterface);
            AreDictionariesEqual(a.OrderedDictionaryInterface.GetEnumerator(), b.OrderedDictionaryInterface.GetEnumerator());
            AreEqual(a.GenericReadOnlyCollectionInterface, b.GenericReadOnlyCollectionInterface);
            AreDictionariesEqual(a.GenericReadOnlyDictionaryInterface, b.GenericReadOnlyDictionaryInterface);
            AreEqual(a.GenericReadOnlyListInterface, b.GenericReadOnlyListInterface);
            AreEqual(a.GenericLinkedList, b.GenericLinkedList);
            AreEqual(a.GenericList, b.GenericList);
            AreDictionariesEqual(
                a.NameValueCollection.Keys,
                a.NameValueCollection,
                b.NameValueCollection.Keys,
                b.NameValueCollection);
            AreEqual(a.GenericObservableCollection, b.GenericObservableCollection);
            AreDictionariesEqual(a.OrderedDictionary.GetEnumerator(), b.OrderedDictionary.GetEnumerator());
            AreEqual(a.GenericQueue, b.GenericQueue);
            AreEqual(a.Queue.Cast<object>(), b.Queue.Cast<object>());
            AreEqual(a.GenericReadOnlyCollection, b.GenericReadOnlyCollection);
            AreEqual(a.GenericReadOnlyObservableCollection, b.GenericReadOnlyObservableCollection);
            AreDictionariesEqual(a.GenericSortedDictionary, b.GenericSortedDictionary);
            AreDictionariesEqual(a.GenericSortedList, b.GenericSortedList);
            AreDictionariesEqual(a.SortedList
                    .Cast<DictionaryEntry>()
                    .Select(it => new KeyValuePair<int, string>(Convert.ToInt32(it.Key), it.Value.ToString())),
                b.SortedList
                    .Cast<DictionaryEntry>()
                    .Select(it => new KeyValuePair<string, int>(it.Key.ToString(), Convert.ToInt32(it.Value))));
            AreEqual(a.GenericSortedSet, b.GenericSortedSet);
            AreEqual(a.GenericStack, b.GenericStack);
            AreEqual(a.Stack.Cast<object>(), b.Stack.Cast<object>());
            AreEqual(a.StringCollection.Cast<object>(), b.StringCollection.Cast<object>());
            AreDictionariesEqual(
                a.StringDictionary.Keys,
                a.StringDictionary.Values,
                b.StringDictionary.Keys,
                b.StringDictionary.Values);
        }

        public void AreDictionariesEqual(
            IEnumerable aKeys, IEnumerable aValues,
            IEnumerable bKeys, IEnumerable bValues)
        {
            Assert.IsFalse(ReferenceEquals(aKeys, aValues));
            Assert.IsFalse(ReferenceEquals(bKeys, bValues));

            AreDictionariesEqual(
                aKeys.Cast<object>().Zip(aValues.Cast<object>()).Select(pair => new KeyValuePair<int, string>(Convert.ToInt32(pair.First), pair.Second.ToString())),
                bKeys.Cast<object>().Zip(bValues.Cast<object>()).Select(pair => new KeyValuePair<string, int>(pair.First.ToString(), Convert.ToInt32(pair.Second))));
        }

        public void AreDictionariesEqual(IDictionaryEnumerator a, IDictionaryEnumerator b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));

            var aPairs = a == null ? null : new Dictionary<int, string>();
            if (a != null)
            {
                while (a.MoveNext())
                {
                    aPairs.Add(Convert.ToInt32(a.Key), a.Value.ToString());
                }
            }

            var bPairs = b == null ? null : new Dictionary<string, int>();
            if (b != null)
            {
                while (b.MoveNext())
                {
                    bPairs.Add(b.Key.ToString(), Convert.ToInt32(b.Value));
                }
            }

            AreDictionariesEqual(aPairs, bPairs);
        }

        public void AreDictionariesEqual(IEnumerable<KeyValuePair<int, string>> a, IEnumerable<KeyValuePair<string, int>> b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));

            var aPairs = a?.ToDictionary(it => it.Key, it => it.Value);
            var bPairs = b?.ToDictionary(it => it.Key, it => it.Value);

            Assert.AreEqual(aPairs?.Count, bPairs?.Count);
            if (aPairs != null)
            {
                foreach (var pair in aPairs)
                {
                    Assert.IsTrue(bPairs.TryGetValue(pair.Key.ToString(), out var value));
                    Assert.AreEqual(pair.Value, value.ToString());
                }
            }
        }

        public void AreEqual(IEnumerable a, IEnumerable b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));
            AreEqual(a?.Cast<object>().ToArray(), b?.Cast<object>().ToArray());
        }

        public void AreEqual<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));
            AreEqual(a?.ToArray(), b?.ToArray());
        }

        public void AreEqual<T>(T[] a, T[] b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));
            Assert.AreEqual(a?.Length, b?.Length);
            if (a != null)
            {
                for (var index = 0; index < a.Length; index++)
                {
                    Assert.AreEqual(Convert.ToInt32(a[index]), Convert.ToInt32(b[index]));
                }
            }
        }
    }
}
