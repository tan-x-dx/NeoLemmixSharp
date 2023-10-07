using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public static class HitBoxHelpers
{
    private const int ItemAbsent = 0;
    private const int ItemPresent = 1;
    private const int ItemRemoved = 2;
    private const int ItemStillPresent = 3;

    /// <summary>
    /// Is the item currently present?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsItemPresent(int i) => (i & ItemPresent) == ItemPresent;

    /// <summary>
    /// Is the item currently absent?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsItemAbsent(int i) => (i & ItemRemoved) == ItemAbsent;

    /// <summary>
    /// Is the item currently present, but it was previously absent?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsItemAdded(int i) => i == ItemPresent;

    /// <summary>
    /// Is the item currently absent, but it was previously present?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsItemRemoved(int i) => i == ItemRemoved;

    /// <summary>
    /// Is the item currently present and it was previously present?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsItemStillPresent(int i) => i == ItemStillPresent;
}