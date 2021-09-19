using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace MGen.Tests.SerializationSupport.DotNet
{
    [Generate]
    public interface ICollectionSerializable : ISerializable
    {
        Dictionary<int, string> GenericDictionary { get; set; }
        Hashtable Hashtable { get; set; }
        LinkedList<int> GenericLinkedList { get; set; }
        OrderedDictionary OrderedDictionary { get; set; }
        SortedSet<int> GenericSortedSet { get; set; }
    }

    public class CollectionSupport
    {
        public void Init(ICollectionSerializable instance)
        {
            var values = 0;

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
            instance.GenericLinkedList = new LinkedList<int>();
            instance.GenericLinkedList.AddFirst(new LinkedListNode<int>(values++));
            instance.GenericLinkedList.AddFirst(new LinkedListNode<int>(values++));
            instance.GenericLinkedList.AddFirst(new LinkedListNode<int>(values++));
            instance.OrderedDictionary = new OrderedDictionary
            {
                { values++, values++.ToString() },
                { values++, values++.ToString() },
                { values++, values++.ToString() }
            };
            instance.GenericSortedSet = new SortedSet<int>
            {
                values++, values++, values++
            };
        }

        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<ICollectionSerializable>();
            Assert.IsNotNull(type);

            var defaultCtor = type.GetConstructor(Type.EmptyTypes);
            Assert.IsNotNull(defaultCtor);

            var serializationCtor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                null,
                new[] { typeof(SerializationInfo), typeof(StreamingContext) },
                null);
            Assert.IsNotNull(serializationCtor);

            var instanceA = defaultCtor.Invoke(Array.Empty<object>()) as ICollectionSerializable;
            Assert.IsNotNull(instanceA);

            Init(instanceA);

            var instanceB = instanceA.CloneViaDotNetSerialization();
            AreEqual(instanceA, instanceB);
        }

        public void AreEqual(ICollectionSerializable a, ICollectionSerializable b)
        {
            Assert.IsNotNull(b);
            AreDictionariesEqual(a.GenericDictionary, b.GenericDictionary);
            AreDictionariesEqual(a.Hashtable.GetEnumerator(), b.Hashtable.GetEnumerator());
            AreEqual(a.GenericLinkedList, b.GenericLinkedList);
            AreDictionariesEqual(a.OrderedDictionary.GetEnumerator(), b.OrderedDictionary.GetEnumerator());
            AreEqual(a.GenericSortedSet, b.GenericSortedSet);
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
