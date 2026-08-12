using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public readonly unsafe struct LemmingData : IPointerData<LemmingData>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LemmingData Create(nint dataRef) => new(dataRef);

    public static int SizeInBytes => LemmingDataSize;

    /// <summary>
    /// The raw size of a LemmingData struct is 184 bytes.
    /// We pad this with an extra 8 bytes to make 192.
    /// This is because the Span.Copy methods work best on blocks
    /// that are multiples of 64 bytes in size. 192 = 64 * 3.
    /// </summary>
    private const int LemmingDataSize = 192;

    [InlineArray(JumperAction.JumperPositionCount)]
    private struct JumperPositionBuffer
    {
        private Point _0;
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

        public JumperPositionBuffer JumperPositionBuffer;

        public Point DehoistPin;
        public Point LaserHitLevelPosition;
        public Point AnchorPosition;
        public Point PreviousAnchorPosition;

        public bool ConstructivePositionFreeze;
        public bool IsStartingAction;
        public bool PlacedBrick;
        public bool StackLow;

        public bool InitialFall;
        public bool EndOfAnimation;
        public bool LaserHit;
        public bool JumpToHoistAdvance;

        public int AnimationFrame;
        public int PhysicsFrame;
        public int AscenderProgress;
        public int NumberOfBricksLeft;
        public int DisarmingFrames;
        public int DistanceFallen;
        public int JumpProgress;
        public int TrueDistanceFallen;
        public int LaserRemainTime;

        public int FastForwardTime;
        public uint CountDownTimer;
        public int ParticleTimer;
    }

    private readonly LemmingDataRaw* _data;

    public void* GetPointer() => _data;

    private LemmingData(nint pointerHandle)
    {
        LemmingDataRaw* p = (LemmingDataRaw*)pointerHandle;
        _data = (LemmingDataRaw*)pointerHandle;

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

    public ref bool ConstructivePositionFreeze => ref Unsafe.AsRef<bool>(&_data->ConstructivePositionFreeze);
    public ref bool IsStartingAction => ref Unsafe.AsRef<bool>(&_data->IsStartingAction);
    public ref bool PlacedBrick => ref Unsafe.AsRef<bool>(&_data->PlacedBrick);
    public ref bool StackLow => ref Unsafe.AsRef<bool>(&_data->StackLow);

    public ref bool InitialFall => ref Unsafe.AsRef<bool>(&_data->InitialFall);
    public ref bool EndOfAnimation => ref Unsafe.AsRef<bool>(&_data->EndOfAnimation);
    public ref bool LaserHit => ref Unsafe.AsRef<bool>(&_data->LaserHit);
    public ref bool JumpToHoistAdvance => ref Unsafe.AsRef<bool>(&_data->JumpToHoistAdvance);

    public ref int AnimationFrame => ref Unsafe.AsRef<int>(&_data->AnimationFrame);
    public ref int PhysicsFrame => ref Unsafe.AsRef<int>(&_data->PhysicsFrame);
    public ref int AscenderProgress => ref Unsafe.AsRef<int>(&_data->AscenderProgress);
    public ref int NumberOfBricksLeft => ref Unsafe.AsRef<int>(&_data->NumberOfBricksLeft);
    public ref int DisarmingFrames => ref Unsafe.AsRef<int>(&_data->DisarmingFrames);
    public ref int DistanceFallen => ref Unsafe.AsRef<int>(&_data->DistanceFallen);
    public ref int JumpProgress => ref Unsafe.AsRef<int>(&_data->JumpProgress);
    public ref int TrueDistanceFallen => ref Unsafe.AsRef<int>(&_data->TrueDistanceFallen);
    public ref int LaserRemainTime => ref Unsafe.AsRef<int>(&_data->LaserRemainTime);

    public ref int FastForwardTime => ref Unsafe.AsRef<int>(&_data->FastForwardTime);
    public ref uint CountDownTimer => ref Unsafe.AsRef<uint>(&_data->CountDownTimer);
    public ref int ParticleTimer => ref Unsafe.AsRef<int>(&_data->ParticleTimer);

    public Span<Point> GetJumperPositions()
    {
        void* p = &_data->JumperPositionBuffer;
        return new Span<Point>(p, JumperAction.JumperPositionCount);
    }
}
