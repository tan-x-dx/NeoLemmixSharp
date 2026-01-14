using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public unsafe readonly struct LemmingData
{
    /// <summary>
    /// The raw size of a LemmingData struct is 184 bytes.
    /// We pad this with an extra 8 bytes to make 192.
    /// This is because the Span.Copy methods work best on blocks
    /// that are multiples of 64 bytes in size. 192 = 64 * 3.
    /// </summary>
    public const int LemmingDataSize = 192;

    [StructLayout(LayoutKind.Sequential, Size = LemmingDataSize)]
    private struct LemmingDataRaw
    {
        public Orientation Orientation;
        public FacingDirection FacingDirection;

        public int TribeId;
        public uint State;

        public RectangularRegion CurrentBounds;

        public int PreviousActionId;
        public int CurrentActionId;
        public int NextActionId;
        public int CountDownActionId;

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

    public LemmingData(void* pointer) => _data = (LemmingDataRaw*)pointer;
    public LemmingData(nint pointerHandle) => _data = (LemmingDataRaw*)pointerHandle;

    public LemmingState CreateLemmingState(Lemming lemming)
    {
        var tribeIdRef = new PointerWrapper(&_data->TribeId);
        var stateRef = new PointerWrapper(&_data->State);

        return new LemmingState(lemming, tribeIdRef, stateRef);
    }

    public ref Orientation Orientation => ref Unsafe.AsRef<Orientation>(&_data->Orientation);
    public ref FacingDirection FacingDirection => ref Unsafe.AsRef<FacingDirection>(&_data->FacingDirection);

    public ref int TribeId => ref Unsafe.AsRef<int>(&_data->TribeId);
    public ref uint State => ref Unsafe.AsRef<uint>(&_data->State);

    public ref RectangularRegion CurrentBounds => ref Unsafe.AsRef<RectangularRegion>(&_data->CurrentBounds);

    public ref int PreviousActionId => ref Unsafe.AsRef<int>(&_data->PreviousActionId);
    public ref int CurrentActionId => ref Unsafe.AsRef<int>(&_data->CurrentActionId);
    public ref int NextActionId => ref Unsafe.AsRef<int>(&_data->NextActionId);
    public ref int CountDownActionId => ref Unsafe.AsRef<int>(&_data->CountDownActionId);

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
