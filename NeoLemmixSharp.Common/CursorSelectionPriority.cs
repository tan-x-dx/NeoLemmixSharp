namespace NeoLemmixSharp.Common;

public static class CursorSelectionPriority
{
    public const int NoneActionPriority = 0;
    public const int NoPriority = 1;
    public const int WalkerMovementPriority = 2;
    public const int NonWalkerMovementPriority = 3;
    public const int PermanentSkillPriority = 4;
    public const int NonPermanentSkillPriority = 5;
}
