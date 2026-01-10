using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

[StructLayout(LayoutKind.Sequential, Size = SizeOfLemmingDataInBytes)]
public struct LemmingData
{
    public const int SizeOfLemmingDataInBytes = 192;

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

public unsafe readonly struct LemmingDataPointer
{
    private readonly LemmingData* _pointer;

    public void* GetPointer() => _pointer;

    public LemmingDataPointer(void* pointer) => _pointer = (LemmingData*)pointer;
    public LemmingDataPointer(nint pointerHandle) => _pointer = (LemmingData*)pointerHandle;

    public ref Orientation Orientation => ref Unsafe.AsRef<Orientation>(&_pointer->Orientation);
    public ref FacingDirection FacingDirection => ref Unsafe.AsRef<FacingDirection>(&_pointer->FacingDirection);

    public ref int TribeId => ref Unsafe.AsRef<int>(&_pointer->TribeId);
    public ref uint State => ref Unsafe.AsRef<uint>(&_pointer->State);

    public ref RectangularRegion CurrentBounds => ref Unsafe.AsRef<RectangularRegion>(&_pointer->CurrentBounds);

    public ref int PreviousActionId => ref Unsafe.AsRef<int>(&_pointer->PreviousActionId);
    public ref int CurrentActionId => ref Unsafe.AsRef<int>(&_pointer->CurrentActionId);
    public ref int NextActionId => ref Unsafe.AsRef<int>(&_pointer->NextActionId);
    public ref int CountDownActionId => ref Unsafe.AsRef<int>(&_pointer->CountDownActionId);

    public ref JumperPositionBuffer JumperPositionBuffer => ref Unsafe.AsRef<JumperPositionBuffer>(&_pointer->JumperPositionBuffer);

    public ref Point DehoistPin => ref Unsafe.AsRef<Point>(&_pointer->DehoistPin);
    public ref Point LaserHitLevelPosition => ref Unsafe.AsRef<Point>(&_pointer->LaserHitLevelPosition);
    public ref Point AnchorPosition => ref Unsafe.AsRef<Point>(&_pointer->AnchorPosition);
    public ref Point PreviousAnchorPosition => ref Unsafe.AsRef<Point>(&_pointer->PreviousAnchorPosition);

    public ref bool ConstructivePositionFreeze => ref Unsafe.AsRef<bool>(&_pointer->ConstructivePositionFreeze);
    public ref bool IsStartingAction => ref Unsafe.AsRef<bool>(&_pointer->IsStartingAction);
    public ref bool PlacedBrick => ref Unsafe.AsRef<bool>(&_pointer->PlacedBrick);
    public ref bool StackLow => ref Unsafe.AsRef<bool>(&_pointer->StackLow);

    public ref bool InitialFall => ref Unsafe.AsRef<bool>(&_pointer->InitialFall);
    public ref bool EndOfAnimation => ref Unsafe.AsRef<bool>(&_pointer->EndOfAnimation);
    public ref bool LaserHit => ref Unsafe.AsRef<bool>(&_pointer->LaserHit);
    public ref bool JumpToHoistAdvance => ref Unsafe.AsRef<bool>(&_pointer->JumpToHoistAdvance);

    public ref int AnimationFrame => ref Unsafe.AsRef<int>(&_pointer->AnimationFrame);
    public ref int PhysicsFrame => ref Unsafe.AsRef<int>(&_pointer->PhysicsFrame);
    public ref int AscenderProgress => ref Unsafe.AsRef<int>(&_pointer->AscenderProgress);
    public ref int NumberOfBricksLeft => ref Unsafe.AsRef<int>(&_pointer->NumberOfBricksLeft);
    public ref int DisarmingFrames => ref Unsafe.AsRef<int>(&_pointer->DisarmingFrames);
    public ref int DistanceFallen => ref Unsafe.AsRef<int>(&_pointer->DistanceFallen);
    public ref int JumpProgress => ref Unsafe.AsRef<int>(&_pointer->JumpProgress);
    public ref int TrueDistanceFallen => ref Unsafe.AsRef<int>(&_pointer->TrueDistanceFallen);
    public ref int LaserRemainTime => ref Unsafe.AsRef<int>(&_pointer->LaserRemainTime);

    public ref int FastForwardTime => ref Unsafe.AsRef<int>(&_pointer->FastForwardTime);
    public ref uint CountDownTimer => ref Unsafe.AsRef<uint>(&_pointer->CountDownTimer);
    public ref int ParticleTimer => ref Unsafe.AsRef<int>(&_pointer->ParticleTimer);

    public Span<Point> GetJumperPositions()
    {
        void* p = &_pointer->JumperPositionBuffer;
        return new Span<Point>(p, JumperAction.JumperPositionCount * sizeof(Point));
    }
}
