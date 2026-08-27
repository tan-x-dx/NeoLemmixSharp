namespace NeoLemmixSharp.Common;

public enum CursorSelectionPriority : byte
{
    NoneActionPriority,
    NoPriority,
    WalkerMovementPriority,
    NonWalkerMovementPriority,
    PermanentSkillPriority,
    NonPermanentSkillPriority,
}
