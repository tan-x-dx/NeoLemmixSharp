using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Common.Util;

public readonly struct DihedralTransformation
{
	public static DihedralTransformation GetForTransformation(
		bool flipHorizontally,
		bool flipVertically,
		bool rotate)
	{
		var rotNum = rotate
			? 1
			: 0;

		if (flipVertically)
		{
			flipHorizontally = !flipHorizontally;
			rotNum += 2;
		}

		return new DihedralTransformation(rotNum, flipHorizontally);
	}

	private readonly int _r;
	private readonly int _a;
	private readonly int _b;

	private readonly int _f;
	private readonly int _m;

	public DihedralTransformation()
		: this(0, false)
	{
	}

	public DihedralTransformation(int r, bool flip)
	{
		_r = r & 3;

		switch (_r)
		{
			case 0:
				_a = 1;
				_b = 0;
				break;
			case 1:
				_a = 0;
				_b = 1;
				break;
			case 2:
				_a = -1;
				_b = 0;
				break;
			case 3:
				_a = 0;
				_b = -1;
				break;
		}

		if (flip)
		{
			_f = 1;
			_m = -1;
		}
		else
		{
			_f = 0;
			_m = 1;
		}
	}

	public override string ToString() => $"Rot {_r}|{FlipString}";

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string FlipString => _f == 0 ? string.Empty : "Flip";

	public void Transform(
		int x,
		int y,
		int width,
		int height,
		out int x0,
		out int y0)
	{
		var w = W(width, height);
		var h = H(width, height);
		var s = _f * Choose(width, height);

		x0 = s + _m * (_a * x - _b * y + w);
		y0 = _b * x + _a * y + h;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int W(int w, int h) => _r switch
	{
		1 => h,
		2 => w,
		_ => 0
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int H(int w, int h) => _r switch
	{
		2 => h,
		3 => w,
		_ => 0
	};

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int Choose(int w, int h) => (_r & 1) == 1
		? h
		: w;
}