using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Util;

internal sealed class SectionIdentifierValidator<TPerfectHasher, TEnum> : IComparer<Interval>
    where TPerfectHasher : struct, IEnumIdentifierHelper<BitBuffer32, TEnum>
    where TEnum : unmanaged, Enum
{
    [SkipLocalsInit]
    internal void AssertSectionsAreContiguous(BitArrayDictionary<TPerfectHasher, BitBuffer32, TEnum, Interval> dictionary)
    {
        Span<Interval> intervals = stackalloc Interval[dictionary.Count];
        dictionary.CopyValuesTo(intervals);

        intervals.Sort(this);

        Debug.Assert(intervals.Length >= 1);

        var firstInterval = intervals.At(0);

        for (var i = 1; i < intervals.Length; i++)
        {
            var secondInterval = intervals.At(i);

            AssertSectionsAreContiguous(firstInterval, secondInterval);

            firstInterval = secondInterval;
        }
    }

    private static void AssertSectionsAreContiguous(Interval firstInterval, Interval secondInterval)
    {
        if (firstInterval.End != secondInterval.Start)
            throw new InvalidOperationException("Sections are not contiguous!");
    }

    int IComparer<Interval>.Compare(Interval x, Interval y)
    {
        int gt = x.Start > y.Start ? 1 : 0;
        int lt = x.Start < y.Start ? 1 : 0;
        return gt - lt;
    }
}
