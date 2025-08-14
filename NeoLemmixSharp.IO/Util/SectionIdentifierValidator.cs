using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Util;

internal sealed class SectionIdentifierValidator<TPerfectHasher, TEnum> : IComparer<Interval>
    where TPerfectHasher : struct, IEnumIdentifierHelper<BitBuffer32, TEnum>
    where TEnum : unmanaged, Enum
{
    [SkipLocalsInit]
    internal void AssertSectionsAreContiguous(BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval> result)
    {
        Span<Interval> intervals = stackalloc Interval[result.Count];
        result.CopyValuesTo(intervals);

        intervals.Sort(this);

        Debug.Assert(intervals.Length >= 1);

        var firstInterval = intervals[0];

        for (var i = 1; i < intervals.Length; i++)
        {
            var secondInterval = intervals[i];

            AssertSectionsAreContiguous(firstInterval, secondInterval);

            firstInterval = secondInterval;
        }
    }

    private static void AssertSectionsAreContiguous(Interval firstInterval, Interval secondInterval)
    {
        if (firstInterval.Start + firstInterval.Length != secondInterval.Start)
            throw new InvalidOperationException("Sections are not contiguous!");
    }

    int IComparer<Interval>.Compare(Interval x, Interval y)
    {
        int gt = x.Start > y.Start ? 1 : 0;
        int lt = x.Start < y.Start ? 1 : 0;
        return gt - lt;
    }
}
