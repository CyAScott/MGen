using NUnit.Framework;
using System;

namespace MGen.Tests.TypeConversion
{
    [Generate]
    public interface IArrayConversion : ISupportConversion
    {
        DateTime[] DateTimes { get; set; }
        DateTime[][] DateTimeArrays { get; set; }

        Guid[] Ids { get; set; }
        Guid[][] IdArrays { get; set; }

        SimpleEnumForAnArray[] SimpleEnums { get; set; }
        SimpleEnumForAnArray[][] SimpleEnumArrays { get; set; }

        int[] Integers { get; set; }
        int[][] IntegerArrays { get; set; }

        string[] Strings { get; set; }
        string[][] StringArrays { get; set; }
    }

    public enum SimpleEnumForAnArray
    {
        Zero = 0,
        One = 1
    }

    [Generate]
    public interface IArrayAsStringsConversion : ISupportConversion
    {
        string[] DateTimes { get; set; }
        string[][] DateTimeArrays { get; set; }

        string[] Ids { get; set; }
        string[][] IdArrays { get; set; }

        string[] SimpleEnums { get; set; }
        string[][] SimpleEnumArrays { get; set; }

        string[] Integers { get; set; }
        string[][] IntegerArrays { get; set; }

        string[] Strings { get; set; }
        string[][] StringArrays { get; set; }
    }

    public class ArraySupport
    {
        public void Init(IArrayConversion instance)
        {
            instance.DateTimes = new[] { DateTime.UtcNow };
            instance.DateTimeArrays = new[] { new[] { DateTime.UtcNow } };
            instance.Ids = new[] { Guid.NewGuid() };
            instance.IdArrays = new[] { new[] { Guid.NewGuid() } };
            instance.SimpleEnums = new[] { SimpleEnumForAnArray.One };
            instance.SimpleEnumArrays = new[] { new[] { SimpleEnumForAnArray.One } };
            instance.Integers = new[] { 3 };
            instance.IntegerArrays = new[] { new[] { 3 } };
            instance.Strings = new[] { "Hello World" };
            instance.StringArrays = new[] { new[] { "Hello World" } };
        }

        [Test]
        public void Test()
        {
            var type = AssemblyScanner.FindImplementationFor<IArrayConversion>();
            Assert.IsNotNull(type);

            var instanceA = Activator.CreateInstance(type) as IArrayConversion;
            Assert.IsNotNull(instanceA);

            Init(instanceA);

            var typeAsStrings = AssemblyScanner.FindImplementationFor<IArrayAsStringsConversion>();
            Assert.IsNotNull(typeAsStrings);

            var instanceB = Convert.ChangeType(instanceA, typeAsStrings) as IArrayAsStringsConversion;
            AreEqual(instanceA, instanceB);
        }

        public void AreEqual(IArrayConversion a, IArrayAsStringsConversion b)
        {
            Assert.IsNotNull(b);
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

        public void AreEqual<T>(T[] a, string[] b)
        {
            Assert.IsFalse(ReferenceEquals(a, b));
            Assert.AreEqual(a?.Length, b?.Length);
            if (a != null)
            {
                for (var index = 0; index < a.Length; index++)
                {
                    Assert.AreEqual(a[index].ToString(), b[index]);
                }
            }
        }

        public void AreEqual<T>(T[][] a, string[][] b)
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
    }
}
