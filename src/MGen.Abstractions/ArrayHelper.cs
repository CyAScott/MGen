using System;
using System.Collections.Generic;

namespace MGen
{
    public static class ArrayHelper
    {
        public static int[] GetLengths(this Array array)
        {
            var lengths = new int[array.Rank];

            for (var dimension = 0; dimension < lengths.Length; dimension++)
            {
                lengths[dimension] = array.GetLength(dimension);
            }

            return lengths;
        }

        public static int[] GetLowerBounds(this Array array)
        {
            var lowerBounds = new int[array.Rank];

            for (var dimension = 0; dimension < lowerBounds.Length; dimension++)
            {
                lowerBounds[dimension] = array.GetLowerBound(dimension);
            }

            return lowerBounds;
        }

        public static IEnumerable<int[]> GetIndices(this Array array) => array.GetIndices(new int[array.Rank]);

        public static IEnumerable<int[]> GetIndices(this Array array, int[] indices, int dimension = 0)
        {
            for (var index = array.GetLowerBound(dimension); index <= array.GetUpperBound(dimension); index++)
            {
                indices[dimension] = index;

                if (dimension == array.Rank - 1)
                {
                    yield return indices;
                }
                else
                {
                    foreach (var _ in array.GetIndices(indices, dimension + 1))
                    {
                        yield return indices;
                    }
                }
            }
        }
    }
}
