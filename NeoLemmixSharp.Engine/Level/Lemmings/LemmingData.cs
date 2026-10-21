using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public readonly unsafe struct LemmingData : IPointerData<LemmingData>
{
    private const int ConstructivePositionFreezeBitIndex = 63;
    private const int IsStartingActionBitIndex = 62;
    private const int PlacedBrickBitIndex = 61;
    private const int StackLowBitIndex = 60;
    private const int InitialFallBitIndex = 59;
    private const int EndOfAnimationBitIndex = 58;
    private const int LaserHitBitIndex = 57;
    private const int JumpToHoistAdvanceBitIndex = 56;

    private const int NumberOfBricksLeftByteIndex = 6;
    private const int DisarmingFramesByteIndex = 5;
    private const int JumpProgressByteIndex = 4;

    private const int ParticleTimerByteIndex = 3;
    private const int AscenderProgressByteIndex = 2;
    private const int AnimationFrameByteIndex = 1;
    private const int PhysicsFrameByteIndex = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LemmingData Create(nint dataRef) => new(dataRef);

    public static int SizeInBytes => LemmingDataSize;

    /// <summary>
    /// The raw size of a LemmingData struct is 164 bytes.
    /// We pad this with an extra 28 bytes to make 192.
    /// This is because the Span.Copy methods work best on blocks
    /// that are multiples of 64 bytes in size. 192 = 64 * 3.
    /// </summary>
    private const int LemmingDataSize = 192;

    [InlineArray(JumperAction.JumperPositionCount)]
    private struct JumperPositionBuffer
    {
        private Point _0;
    }

    [InlineArray(EngineConstants.NumberOfCountDownChars)]
    private struct CountDownCharBuffer
    {
        private char _0;
    }

    [StructLayout(LayoutKind.Sequential, Size = LemmingDataSize)]
    private struct LemmingDataRaw
    {
        public DihedralTransformation DihedralTransformation;

        public uint State;
        public int TribeId;

        public RectangularRegion CurrentBounds;

        public LemmingActionType PreviousActionType;
        public LemmingActionType CurrentActionType;
        public LemmingActionType NextActionType;
        public LemmingActionType CountDownActionType;

        public Point DehoistPin;
        public Point LaserHitLevelPosition;
        public Point AnchorPosition;
        public Point PreviousAnchorPosition;

        public JumperPositionBuffer JumperPositionBuffer;
        public CountDownCharBuffer CountDownCharBuffer;
        public uint CountDownTimer;

        public PackedBytes64 PackedBytes;

        public int DistanceFallen;
        public int TrueDistanceFallen;
        public int LaserRemainTime;

        public uint FastForwardTime;
        public LemmingRemovalReason LemmingRemovalReason;
    }

    private readonly LemmingDataRaw* _data;

    public void* GetPointer() => _data;

    private LemmingData(nint pointerHandle)
    {
        LemmingDataRaw* p = (LemmingDataRaw*)pointerHandle;
        _data = p;

        p->PreviousActionType = LemmingActionType.NoneAction;
        p->CurrentActionType = LemmingActionType.NoneAction;
        p->NextActionType = LemmingActionType.NoneAction;
        p->CountDownActionType = LemmingActionType.NoneAction;
        p->DehoistPin = new(-1, -1);
        p->LaserHitLevelPosition = new(-1, -1);
        p->AnchorPosition = new(-1, -1);
        p->PreviousAnchorPosition = new(-1, -1);
    }

    public ref DihedralTransformation DihedralTransformation => ref Unsafe.AsRef<DihedralTransformation>(&_data->DihedralTransformation);
    public ref Orientation Orientation => ref Unsafe.AsRef<Orientation>(&_data->DihedralTransformation.Orientation);
    public ref FacingDirection FacingDirection => ref Unsafe.AsRef<FacingDirection>(&_data->DihedralTransformation.FacingDirection);

    public ref uint State => ref Unsafe.AsRef<uint>(&_data->State);
    public ref int TribeId => ref Unsafe.AsRef<int>(&_data->TribeId);

    public ref RectangularRegion CurrentBounds => ref Unsafe.AsRef<RectangularRegion>(&_data->CurrentBounds);

    public ref LemmingActionType PreviousActionType => ref Unsafe.AsRef<LemmingActionType>(&_data->PreviousActionType);
    public ref LemmingActionType CurrentActionType => ref Unsafe.AsRef<LemmingActionType>(&_data->CurrentActionType);
    public ref LemmingActionType NextActionType => ref Unsafe.AsRef<LemmingActionType>(&_data->NextActionType);
    public ref LemmingActionType CountDownActionType => ref Unsafe.AsRef<LemmingActionType>(&_data->CountDownActionType);

    public ref Point DehoistPin => ref Unsafe.AsRef<Point>(&_data->DehoistPin);
    public ref Point LaserHitLevelPosition => ref Unsafe.AsRef<Point>(&_data->LaserHitLevelPosition);
    public ref Point AnchorPosition => ref Unsafe.AsRef<Point>(&_data->AnchorPosition);
    public ref Point PreviousAnchorPosition => ref Unsafe.AsRef<Point>(&_data->PreviousAnchorPosition);

    public bool ConstructivePositionFreeze
    {
        get => _data->PackedBytes.GetBit(ConstructivePositionFreezeBitIndex);
        set => _data->PackedBytes.SetBit(ConstructivePositionFreezeBitIndex, value);
    }
    public bool IsStartingAction
    {
        get => _data->PackedBytes.GetBit(IsStartingActionBitIndex);
        set => _data->PackedBytes.SetBit(IsStartingActionBitIndex, value);
    }
    public bool PlacedBrick
    {
        get => _data->PackedBytes.GetBit(PlacedBrickBitIndex);
        set => _data->PackedBytes.SetBit(PlacedBrickBitIndex, value);
    }
    public bool StackLow
    {
        get => _data->PackedBytes.GetBit(StackLowBitIndex);
        set => _data->PackedBytes.SetBit(StackLowBitIndex, value);
    }
    public bool InitialFall
    {
        get => _data->PackedBytes.GetBit(InitialFallBitIndex);
        set => _data->PackedBytes.SetBit(InitialFallBitIndex, value);
    }
    public bool EndOfAnimation
    {
        get => _data->PackedBytes.GetBit(EndOfAnimationBitIndex);
        set => _data->PackedBytes.SetBit(EndOfAnimationBitIndex, value);
    }
    public bool LaserHit
    {
        get => _data->PackedBytes.GetBit(LaserHitBitIndex);
        set => _data->PackedBytes.SetBit(LaserHitBitIndex, value);
    }

    public bool JumpToHoistAdvance
    {
        get => _data->PackedBytes.GetBit(JumpToHoistAdvanceBitIndex);
        set => _data->PackedBytes.SetBit(JumpToHoistAdvanceBitIndex, value);
    }

    public int NumberOfBricksLeft
    {
        get
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += NumberOfBricksLeftByteIndex;
            return *p;
        }
        set
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += NumberOfBricksLeftByteIndex;
            *p = (byte)value;
        }
    }
    public int DisarmingFrames
    {
        get
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += DisarmingFramesByteIndex;
            return *p;
        }
        set
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += DisarmingFramesByteIndex;
            *p = (byte)value;
        }
    }
    public int JumpProgress
    {
        get
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += JumpProgressByteIndex;
            return *p;
        }
        set
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += JumpProgressByteIndex;
            *p = (byte)value;
        }
    }

    public int ParticleTimer
    {
        get
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += ParticleTimerByteIndex;
            return *p;
        }
        set
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += ParticleTimerByteIndex;
            *p = (byte)value;
        }
    }
    public int AscenderProgress
    {
        get
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += AscenderProgressByteIndex;
            return *p;
        }
        set
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += AscenderProgressByteIndex;
            *p = (byte)value;
        }
    }
    public int AnimationFrame
    {
        get
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += AnimationFrameByteIndex;
            return *p;
        }
        set
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += AnimationFrameByteIndex;
            *p = (byte)value;
        }
    }
    public int PhysicsFrame
    {
        get
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += PhysicsFrameByteIndex;
            return *p;
        }
        set
        {
            byte* p = (byte*)&_data->PackedBytes;
            p += PhysicsFrameByteIndex;
            *p = (byte)value;
        }
    }

    public ref int DistanceFallen => ref Unsafe.AsRef<int>(&_data->DistanceFallen);
    public ref int TrueDistanceFallen => ref Unsafe.AsRef<int>(&_data->TrueDistanceFallen);
    public ref int LaserRemainTime => ref Unsafe.AsRef<int>(&_data->LaserRemainTime);

    public ref uint FastForwardTime => ref Unsafe.AsRef<uint>(&_data->FastForwardTime);
    public ref uint CountDownTimer => ref Unsafe.AsRef<uint>(&_data->CountDownTimer);
    public ref LemmingRemovalReason LemmingRemovalReason => ref Unsafe.AsRef<LemmingRemovalReason>(&_data->LemmingRemovalReason);

    public Span<Point> GetJumperPositions()
    {
        void* p = &_data->JumperPositionBuffer;
        return new Span<Point>(p, JumperAction.JumperPositionCount);
    }

    public Span<char> GetCountDownCharsSpan()
    {
        void* p = &_data->CountDownCharBuffer;
        return new Span<char>(p, EngineConstants.NumberOfCountDownChars);
    }

    public void ResetFlagsOnNewAction() => _data->PackedBytes.SetRawValue(1UL << IsStartingActionBitIndex);
}
