using NUnit.Framework;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace MGen.Tests.SerializationSupport.DotNet
{
    [Generate]
    public interface IArraySerializable : ISerializable
    {
        Array Values { get; set; }
        Array ValueArrays { get; set; }
        Array ValueTable { get; set; }

        DateTime[] DateTimes { get; set; }
        DateTime[][] DateTimeArrays { get; set; }
        DateTime[,] DateTimeTable { get; set; }

        Guid[] Ids { get; set; }
        Guid[][] IdArrays { get; set; }
        Guid[,] IdTable { get; set; }

        IArrayElement[] Array { get; set; }
        IArrayElement[][] ArrayArrays { get; set; }
        IArrayElement[,] Table { get; set; }

        SimpleEnumForAnArray[] SimpleEnums { get; set; }
        SimpleEnumForAnArray[][] SimpleEnumArrays { get; set; }
        SimpleEnumForAnArray[,] SimpleEnumTable { get; set; }

        int[] Integers { get; set; }
        int[][] IntegerArrays { get; set; }
        int[,] IntegerTable { get; set; }

        string[] Strings { get; set; }
        string[][] StringArrays { get; set; }
        string[,] StringTable { get; set; }
    }

    public interface IArrayElement : ISerializable
    {
        Guid Id { get; set; }
    }

    public enum SimpleEnumForAnArray
    {
        Zero = 0,
        One = 1
    }

    public class ArraySupport
    {
        public void Init(IArraySerializable instance)
        {
            instance.Values = new[] { DateTime.UtcNow };
            instance.ValueArrays = new[] { new[] { DateTime.UtcNow } };
            instance.ValueTable = new[,] { { DateTime.UtcNow } };
            instance.DateTimes = new[] { DateTime.UtcNow };
            instance.DateTimeArrays = new[] { new[] { DateTime.UtcNow } };
            instance.DateTimeTable = new[,] { { DateTime.UtcNow } };
            instance.Ids = new[] { Guid.NewGuid() };
            instance.IdArrays = new[] { new[] { Guid.NewGuid() } };
            instance.IdTable = new[,] { { Guid.NewGuid() } };
            instance.SimpleEnums = new[] { SimpleEnumForAnArray.One };
            instance.SimpleEnumArrays = new[] { new[] { SimpleEnumForAnArray.One } };
            instance.SimpleEnumTable = new[,] { { SimpleEnumForAnArray.One } };
            instance.Integers = new[] { 3 };
            instance.IntegerArrays = new[] { new[] { 3 } };
            instance.IntegerTable = new[,] { { 3 } };
            instance.Strings = new[] { "Hello World" };
            instance.StringArrays = new[] { new[] { "Hello World" } };
            instance.StringTable = new[,] { { "Hello World" } };
        }

        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<IArraySerializable>();
            Assert.IsNotNull(type);

            var defaultCtor = type.GetConstructor(Type.EmptyTypes);
            Assert.IsNotNull(defaultCtor);

            var serializationCtor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance,
                null,
                new[] { typeof(SerializationInfo), typeof(StreamingContext) },
                null);
            Assert.IsNotNull(serializationCtor);

            var instanceA = defaultCtor.Invoke(new object[0]) as IArraySerializable;
            Assert.IsNotNull(instanceA);

            Init(instanceA);

            var instanceB = instanceA.Clone();
            AreEqual(instanceA, instanceB);
        }

        public void AreEqual(IArraySerializable a, IArraySerializable b)
        {
            Assert.IsNotNull(b);
            AreEqual(a.Values, b.Values);
            AreEqual(a.ValueArrays, b.ValueArrays);
            AreEqual(a.ValueTable, b.ValueTable);
            AreEqual(a.DateTimes, b.DateTimes);
            AreEqual(a.DateTimeArrays, b.DateTimeArrays);
            AreEqual(a.Ids, b.Ids);
            AreEqual(a.IdArrays, b.IdArrays);
            AreEqual(a.SimpleEnums, b.SimpleEnums);
            AreEqual(a.SimpleEnumArrays, b.SimpleEnumArrays);
            AreEqual(a.Integers, b.Integers);
            AreEqual(a.IntegerArrays, b.IntegerArrays);
            AreEqual(a.Strings, b.Strings);
            AreEqual(a.StringArrays, b.StringArrays);
        }

        public void AreEqual(Array a, Array b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));
            Assert.AreEqual(a?.Rank, b?.Rank);

            if (a == null)
            {
                return;
            }

            var indcies = new int[a.Rank];

            bool TryToScanDimension(int dimension)
            {
                if (dimension < indcies.Length)
                {
                    for (indcies[dimension] = a.GetLowerBound(dimension); indcies[dimension] < a.GetLength(dimension); indcies[dimension]++)
                    {
                        if (!TryToScanDimension(dimension + 1))
                        {
                            Assert.AreEqual(a.GetValue(indcies), b.GetValue(indcies));
                        }
                    }
                    return true;
                }
                return false;
            }

            TryToScanDimension(0);
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

        public void AreEqual<T>(T[][] a, T[][] b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));
            Assert.AreEqual(a?.Length, b?.Length);

            if (a != null)
            {
                for (var index = 0; index < a.Length; index++)
                {
                    AreEqual(a[index], b[index]);
                }
            }
        }

        public void AreEqual<T>(T[,] a, T[,] b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));
            Assert.AreEqual(a?.GetLength(0), b?.GetLength(0));
            Assert.AreEqual(a?.GetLength(1), b?.GetLength(1));

            if (a != null)
            {
                for (var index0 = 0; index0 < a.GetLength(0); index0++)
                {
                    for (var index1 = 0; index1 < a.GetLength(1); index1++)
                    {
                        Assert.AreEqual(a[index0, index1], b[index0, index1]);
                    }
                }
            }
        }
    }
}
