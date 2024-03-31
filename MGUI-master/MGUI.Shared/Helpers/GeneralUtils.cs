using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MGUI.Shared.Helpers
{
    public enum RelativePosition : byte
    {
        TopLeft = 0b_1000_0000,
        TopCenter = 0b_0100_0000,
        TopRight = 0b_0010_0000,
        MiddleLeft = 0b_0001_0000,
        MiddleCenter = 0b_1111_1111,
        MiddleRight = 0b_0000_1000,
        BottomLeft = 0b_0000_0100,
        BottomCenter = 0b_0000_0010,
        BottomRight = 0b_0000_0001
    }

    public static class GeneralUtils
    {
        public static bool IsAlmostEqual(this double d1, double d2, double epsilon = 1e-12) => Math.Abs(d1 - d2) <= epsilon;
        public static bool IsAlmostZero(this double value, double epsilon = 1e-12) => value.IsAlmostEqual(0.0, epsilon);
        public static bool IsGreaterThan(this double d1, double d2, double epsilon = 1e-12) => d1 > d2 && !d1.IsAlmostEqual(d2, epsilon);
        public static bool IsGreaterThanOrEqual(this double d1, double d2, double epsilon = 1e-12) => d1 >= d2 || d1.IsAlmostEqual(d2, epsilon);
        public static bool IsLessThan(this double d1, double d2, double epsilon = 1e-12) => d1 < d2 && !d1.IsAlmostEqual(d2, epsilon);
        public static bool IsLessThanOrEqual(this double d1, double d2, double epsilon = 1e-12) => d1 <= d2 || d1.IsAlmostEqual(d2, epsilon);

        public static bool IsAlmostEqual(this float f1, float f2, double epsilon = 1e-9) => Math.Abs(f1 - f2) <= epsilon;
        public static bool IsAlmostZero(this float value, double epsilon = 1e-9) => value.IsAlmostEqual(0.0f, epsilon);
        public static bool IsGreaterThan(this float f1, float f2, double epsilon = 1e-9) => f1 > f2 && !f1.IsAlmostEqual(f2, epsilon);
        public static bool IsGreaterThanOrEqual(this float f1, float f2, double epsilon = 1e-9) => f1 >= f2 || f1.IsAlmostEqual(f2, epsilon);
        public static bool IsLessThan(this float f1, float f2, double epsilon = 1e-9) => f1 < f2 && !f1.IsAlmostEqual(f2, epsilon);
        public static bool IsLessThanOrEqual(this float f1, float f2, double epsilon = 1e-9) => f1 <= f2 || f1.IsAlmostEqual(f2, epsilon);

        //Taken from: https://stackoverflow.com/questions/6219614/convert-a-long-to-two-int-for-the-purpose-of-reconstruction
        /// <summary>Packs 2 ints into 1 long via bitwise operations. To go from 1 long back to 2 ints, use <see cref="UnpackInts(long)"/></summary>
        public static long PackInts(int first, int second)
        {
            long b = second;
            b = b << 32;
            b = b | (uint)first;
            return b;
        }

        //Taken from: https://stackoverflow.com/questions/6219614/convert-a-long-to-two-int-for-the-purpose-of-reconstruction
        /// <summary>Unpacks 2 ints from 1 long via bitwise operations. To go from 2 ints back to 1 long, use <see cref="PackInts(int, int)"/></summary>
        public static (int First, int Second) UnpackInts(long value)
        {
            var first = (int)(value & uint.MaxValue);
            var second = (int)(value >> 32);
            return (first, second);
        }

        public static IEnumerable<T> AsEnumerable<T>(params T[] values) => values;

        public static T Max<T>(params T[] values) => values.Max();
        public static T Min<T>(params T[] values) => values.Min();

        public record Permutation<T>(T First, T Second);

        /// <summary>Returns every possible permutation pair of elements in this <see cref="IList{T}"/> (order matters, I.E. Permutation(1, 2) is not the same as Permutation(2, 1))</summary>
        public static IEnumerable<Permutation<T>> GetPermutations<T>(this IList<T> @this)
        {
            for (var i = 0; i < @this.Count; i++)
            {
                for (var j = 0; j < @this.Count; j++)
                {
                    if (i == j)
                        continue;

                    yield return new(@this[i], @this[j]);
                }
            }
        }

        //Adapted from: https://stackoverflow.com/a/364993/11689514
        /// <summary>Returns a power of 2 whose value is >= the given <paramref name="value"/>. Examples:<para/>
        /// <code>Input     Output<br/>
        /// 31.9f       32<br/>
        /// 32.0f       32<br/>
        /// 32.1f       64<br/>
        /// 200f        256</code></summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int NextPowerOf2(float value) => 1 << (int)Math.Ceiling(Math.Log2(value));

        //Taken from: https://stackoverflow.com/questions/35327255/c-sharp-linq-select-two-consecutive-items-at-once
        public static IEnumerable<TResult> SelectConsecutivePairs<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> selector)
            => Enumerable.Zip(source, source.Skip(1), selector);

        /// <summary>Enumerates every consecutive pair in <paramref name="this"/> enumerable.</summary>
        /// <param name="includeLastToFirst">If true, will attempt to pair the last element in <paramref name="this"/> with the first element in <paramref name="this"/>, 
        /// provided they are not the same element. (I.E. <paramref name="this"/>'s length is > 2)</param>
        public static IEnumerable<(TSource, TSource)> SelectConsecutivePairs<TSource>(this IEnumerable<TSource> @this, bool includeLastToFirst)
        {
            if (!includeLastToFirst || !@this.Skip(2).Any())
                return Enumerable.Zip(@this, @this.Skip(1));
            else
                return Enumerable.Zip(@this, @this.Skip(1).Append(@this.First()));
        }

        public static IEnumerable<(T Current, T Next, bool HasNext)> EnumerateWithLookahead<T>(this IEnumerable<T> items)
        {
            if (!items.Any())
                yield break;

            var first = true;
            T previous = default;

            foreach (var item in items)
            {
                if (first)
                    first = false;
                else
                    yield return (previous, item, true);
                previous = item;
            }

            yield return new(previous, default, false);
        }

        //Taken from: https://stackoverflow.com/questions/3041946/how-to-integer-divide-round-negative-numbers-down
        public static int DivideRoundDown(int numerator, int denominator) => (numerator / denominator) + ((numerator % denominator) >> 31);

        public static bool ContainsMoreThanOneElement<TSource>(this IEnumerable<TSource> source)
        {
            using var enumerator = source.GetEnumerator();
            return enumerator.MoveNext() && enumerator.MoveNext();
        }

        public static bool ContainsExactlyOneElement<TSource>(this IEnumerable<TSource> source)
        {
            using var enumerator = source.GetEnumerator();
            return enumerator.MoveNext() && !enumerator.MoveNext();
        }

        /// <summary>Computes where the topleft position would be if the given <paramref name="position"/> is at the given <see cref="RelativePosition"/> of an object with the given <paramref name="size"/>.<para/>
        /// EX:<br/>Inputs: <paramref name="position"/>=(5,20), <paramref name="anchor"/>=<see cref="RelativePosition.MiddleRight"/>, <paramref name="size"/>=(30, 22)<br/>
        /// Returns: (5,20)-(30,22/2)=(-25,9)</summary>
        public static Vector2 GetTopleft(Vector2 position, RelativePosition anchor, System.Drawing.SizeF size)
        {
            var offset = anchor switch
            {
                RelativePosition.TopLeft => Vector2.Zero,
                RelativePosition.TopCenter => new Vector2(size.Width / 2, 0),
                RelativePosition.TopRight => new Vector2(size.Width, 0),
                RelativePosition.MiddleLeft => new Vector2(0, size.Height / 2),
                RelativePosition.MiddleCenter => new Vector2(size.Width / 2, size.Height / 2),
                RelativePosition.MiddleRight => new Vector2(size.Width, size.Height / 2),
                RelativePosition.BottomLeft => new Vector2(0, size.Height),
                RelativePosition.BottomCenter => new Vector2(size.Width / 2, size.Height),
                RelativePosition.BottomRight => new Vector2(size.Width, size.Height),
                _ => throw new NotImplementedException($"Unrecognized {nameof(RelativePosition)}: {anchor}"),
            };

            return position - offset;
        }

        /// <summary>Computes where the topleft position would be if the given <paramref name="position"/> is at the given <see cref="RelativePosition"/> of an object with the given <paramref name="size"/>.<para/>
        /// EX:<br/>Inputs: <paramref name="position"/>=(5,20), <paramref name="anchor"/>=<see cref="RelativePosition.MiddleRight"/>, <paramref name="size"/>=(30, 22)<br/>
        /// Returns: (5,20)-(30,22/2)=(-25,9)</summary>
        public static Point GetTopleft(Point position, RelativePosition anchor, Size size)
        {
            var offset = anchor switch
            {
                RelativePosition.TopLeft => Point.Zero,
                RelativePosition.TopCenter => new Point(size.Width / 2, 0),
                RelativePosition.TopRight => new Point(size.Width, 0),
                RelativePosition.MiddleLeft => new Point(0, size.Height / 2),
                RelativePosition.MiddleCenter => new Point(size.Width / 2, size.Height / 2),
                RelativePosition.MiddleRight => new Point(size.Width, size.Height / 2),
                RelativePosition.BottomLeft => new Point(0, size.Height),
                RelativePosition.BottomCenter => new Point(size.Width / 2, size.Height),
                RelativePosition.BottomRight => new Point(size.Width, size.Height),
                _ => throw new NotImplementedException($"Unrecognized {nameof(RelativePosition)}: {anchor}"),
            };

            return position - offset;
        }

        public static float DegreesToRadians(float degrees) => (float)(degrees * Math.PI / 180.0f);
        public static float RadiansToDegrees(float radians) => (float)(radians / Math.PI * 180.0f);

        public static int PreviousLoopedIndex<T>(this IList<T> @this, int current)
        {
            if (current < 0)
                current = (current % @this.Count) + @this.Count;
            return current == 0 ? @this.Count - 1 : current - 1;
        }
        public static int NextLoopedIndex<T>(this IList<T> @this, int current) => (current + 1) % @this.Count;
        public static T PreviousLoopedItem<T>(this IList<T> @this, int current) => @this[@this.PreviousLoopedIndex(current)];
        public static T NextLoopedItem<T>(this IList<T> @this, int current) => @this[@this.NextLoopedIndex(current)];

        /// <summary>Groups the elements in <see cref="IEnumerable{T}"/> into chunks of <paramref name="groupLength"/> consecutive elements.<para/>
        /// Example:<br/>
        /// Input: { 20, 12, 16, 4, 55, 123, 17, 4 }, <paramref name="groupLength"/>=3<br/>
        /// Output: { { 20, 12, 16 }, { 4, 55, 123 }, { 17, 4 } }</summary>
        /// <param name="groupLength">How many elements should be in each group.</param>
        public static IEnumerable<IGrouping<int, TValue>> AsGroupsOfLength<TValue>(this IEnumerable<TValue> @this, int groupLength)
            => @this.Select((s, i) => new { Value = s, Index = i }).GroupBy(item => item.Index / groupLength, item => item.Value);

        private static readonly ReadOnlyCollection<int> UnixPlatformIds = new List<int>() { 4, 6, 128, (int)PlatformID.Unix }.AsReadOnly();
        //Adapted from: https://mono.fandom.com/wiki/Detecting_the_execution_platform
        public static bool IsPlatformUnix => UnixPlatformIds.Contains((int)Environment.OSVersion.Platform);

        /// <summary>This method is intended to replace the usage of <see cref="Collection{T}.Clear"/> on <see cref="ObservableCollection{T}"/>s<br/> 
        /// because Clear does not include the old items that were removed in the <see cref="ObservableCollection{T}.CollectionChanged"/> event args.<para/>
        /// See also: <see href="https://stackoverflow.com/questions/224155/when-clearing-an-observablecollection-there-are-no-items-in-e-olditems"/></summary>
        public static void ClearOneByOne<T>(this ObservableCollection<T> @this)
        {
            while (@this.Count > 0)
            {
                @this.RemoveAt(@this.Count - 1);
            }
        }

        public static T RemoveLast<T>(IList<T> items, T defaultValue)
        {
            if (items.Count == 0)
                return defaultValue;


            var value = items[^1];
            items.RemoveAt(items.Count - 1);
            return value;
        }

        public static string ReadEmbeddedResourceAsString(Assembly currentAssembly, string resourceName)
        {
            using var resourceStream = currentAssembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(resourceStream);
            return reader.ReadToEnd();
        }
    }
}
