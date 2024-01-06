﻿using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Common.Util.Collections.BitArrays;

public sealed class UintArrayWrapper : IUintWrapper
{
	public const int Mask = (1 << BitArray.Shift) - 1;

	private readonly uint[] _bits;
	private readonly int _offset;

	public int Size { get; }

	public UintArrayWrapper(int length, bool initialBitFlag = false)
	{
		var arraySize = (length + Mask) >> BitArray.Shift;
		_bits = new uint[arraySize];
		_offset = 0;
		Size = arraySize;

		if (!initialBitFlag)
			return;

		Array.Fill(_bits, uint.MaxValue);

		var shift = length & Mask;
		var mask = (1U << shift) - 1U;
		_bits[^1] = mask;
	}

	public UintArrayWrapper(uint[] array, int offset, int length)
	{
		_bits = array;
		_offset = offset;
		Size = length;
	}

	public UintArrayWrapper(uint[] array)
	{
		_bits = array;
		_offset = 0;
		Size = array.Length;
	}

	[Pure]
	public Span<uint> AsSpan() => new(_bits, _offset, Size);

	[Pure]
	public ReadOnlySpan<uint> AsReadOnlySpan() => new(_bits, _offset, Size);
}