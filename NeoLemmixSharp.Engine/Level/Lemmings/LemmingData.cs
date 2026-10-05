using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public readonly unsafe struct LemmingData : IPointerData<LemmingData>
{
    private const int ConstructivePositionFreezeBitIndex = 31;
    private const int IsStartingActionBitIndex = 30;
    private const int PlacedBrickBitIndex = 29;
    private const int StackLowBitIndex = 28;
    private const int InitialFallBitIndex = 27;
    private const int EndOfAnimationBitIndex = 26;
    private const int LaserHitBitIndex = 25;
    private const int JumpToHoistAdvanceBitIndex = 24;

    private const int AnimationFrameByteIndex = 2;
    private const int PhysicsFrameByteIndex = 1;
    private const int NumberOfBricksLeftByteIndex = 0;

    private const int AscenderProgressByteIndex = 3;
    private const int DisarmingFramesByteIndex = 2;
    private const int JumpProgressByteIndex = 1;
    private const int ParticleTimerByteIndex = 0;

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

        public PackedBytes PackedBytes1;
        public PackedBytes PackedBytes2;

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
        get => _data->PackedBytes1.GetBit(ConstructivePositionFreezeBitIndex);
        set => _data->PackedBytes1.SetBit(ConstructivePositionFreezeBitIndex, value);
    }
    public bool IsStartingAction
    {
        get => _data->PackedBytes1.GetBit(IsStartingActionBitIndex);
        set => _data->PackedBytes1.SetBit(IsStartingActionBitIndex, value);
    }
    public bool PlacedBrick
    {
        get => _data->PackedBytes1.GetBit(PlacedBrickBitIndex);
        set => _data->PackedBytes1.SetBit(PlacedBrickBitIndex, value);
    }
    public bool StackLow
    {
        get => _data->PackedBytes1.GetBit(StackLowBitIndex);
        set => _data->PackedBytes1.SetBit(StackLowBitIndex, value);
    }
    public bool InitialFall
    {
        get => _data->PackedBytes1.GetBit(InitialFallBitIndex);
        set => _data->PackedBytes1.SetBit(InitialFallBitIndex, value);
    }
    public bool EndOfAnimation
    {
        get => _data->PackedBytes1.GetBit(EndOfAnimationBitIndex);
        set => _data->PackedBytes1.SetBit(EndOfAnimationBitIndex, value);
    }
    public bool LaserHit
    {
        get => _data->PackedBytes1.GetBit(LaserHitBitIndex);
        set => _data->PackedBytes1.SetBit(LaserHitBitIndex, value);
    }
    public bool JumpToHoistAdvance
    {
        get => _data->PackedBytes1.GetBit(JumpToHoistAdvanceBitIndex);
        set => _data->PackedBytes1.SetBit(JumpToHoistAdvanceBitIndex, value);
    }

    public int AnimationFrame
    {
        get => _data->PackedBytes1.GetByte(AnimationFrameByteIndex);
        set => _data->PackedBytes1.SetByte(AnimationFrameByteIndex, value);
    }
    public int PhysicsFrame
    {
        get => _data->PackedBytes1.GetByte(PhysicsFrameByteIndex);
        set => _data->PackedBytes1.SetByte(PhysicsFrameByteIndex, value);
    }
    public int NumberOfBricksLeft
    {
        get => _data->PackedBytes1.GetByte(NumberOfBricksLeftByteIndex);
        set => _data->PackedBytes1.SetByte(NumberOfBricksLeftByteIndex, value);
    }

    public int AscenderProgress
    {
        get => _data->PackedBytes2.GetByte(AscenderProgressByteIndex);
        set => _data->PackedBytes2.SetByte(AscenderProgressByteIndex, value);
    }
    public int DisarmingFrames
    {
        get => _data->PackedBytes2.GetByte(DisarmingFramesByteIndex);
        set => _data->PackedBytes2.SetByte(DisarmingFramesByteIndex, value);
    }
    public int JumpProgress
    {
        get => _data->PackedBytes2.GetByte(JumpProgressByteIndex);
        set => _data->PackedBytes2.SetByte(JumpProgressByteIndex, value);
    }
    public int ParticleTimer
    {
        get => _data->PackedBytes2.GetByte(ParticleTimerByteIndex);
        set => _data->PackedBytes2.SetByte(ParticleTimerByteIndex, value);
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
}
