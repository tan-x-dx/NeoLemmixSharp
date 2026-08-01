namespace NeoLemmixSharp.Common;

public enum CursorSelectionPriority
{
    NoneActionPriority = -1,
    NoPriority,
    WalkerMovementPriority,
    NonWalkerMovementPriority,
    PermanentSkillPriority,
    NonPermanentSkillPriority,
}
