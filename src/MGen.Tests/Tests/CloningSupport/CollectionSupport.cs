using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MGen.Tests.CloningSupport
{
    [Generate]
    public interface ICollectionClone : ICloneable
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

    public class CollectionSupport
    {
        public void Init(ICollectionClone instance)
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
            var type = AssemblyScanner.FindImplementationFor<ICollectionClone>();
            Assert.IsNotNull(type);

            var constructors = type.GetConstructors();
            Assert.AreEqual(2, constructors.Length);

            var defaultCtor = constructors.SingleOrDefault(it => it.GetParameters().Length == 0);
            Assert.IsNotNull(defaultCtor);

            var cloneCtor = constructors.SingleOrDefault(it => it.GetParameters().Length == 1);
            Assert.IsNotNull(cloneCtor);

            var parameters = cloneCtor.GetParameters();
            Assert.AreEqual(type, parameters[0].ParameterType);

            Assert.IsTrue(Attribute.IsDefined(parameters[0], typeof(NotNullAttribute)));

            var instanceA = defaultCtor.Invoke(new object[0]) as ICollectionClone;
            Assert.IsNotNull(instanceA);

            Init(instanceA);

            var instanceB = instanceA.Clone() as ICollectionClone;
            AreEqual(instanceA, instanceB);

            var instanceC = cloneCtor.Invoke(new object[] { instanceA }) as ICollectionClone;
            AreEqual(instanceA, instanceC);
        }

        public void AreEqual(ICollectionClone a, ICollectionClone b)
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
            AreEqual(a.SortedList.Cast<object>(), b.SortedList.Cast<object>());
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
                aKeys.Cast<object>().Zip(aValues.Cast<object>()).Select(pair => new KeyValuePair<object, object>(pair.First, pair.Second)),
                bKeys.Cast<object>().Zip(bValues.Cast<object>()).Select(pair => new KeyValuePair<object, object>(pair.First, pair.Second)));
        }

        public void AreDictionariesEqual(IDictionaryEnumerator a, IDictionaryEnumerator b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));

            var aPairs = a == null ? null : new Dictionary<object, object>();
            if (a != null)
            {
                while (a.MoveNext())
                {
                    aPairs.Add(a.Key, a.Value);
                }
            }

            var bPairs = b == null ? null : new Dictionary<object, object>();
            if (b != null)
            {
                while (b.MoveNext())
                {
                    bPairs.Add(b.Key, b.Value);
                }
            }

            AreDictionariesEqual(aPairs, bPairs);
        }

        public void AreDictionariesEqual<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> a, IEnumerable<KeyValuePair<TKey, TValue>> b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));

            var aPairs = a?.ToDictionary(it => it.Key, it => it.Value);
            var bPairs = b?.ToDictionary(it => it.Key, it => it.Value);

            Assert.AreEqual(aPairs?.Count, bPairs?.Count);
            if (aPairs != null)
            {
                foreach (var pair in aPairs)
                {
                    Assert.IsTrue(bPairs.TryGetValue(pair.Key, out var value));
                    Assert.AreEqual(pair.Value, value);
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
                    Assert.AreEqual(a[index], b[index]);
                }
            }
        }
    }
}
