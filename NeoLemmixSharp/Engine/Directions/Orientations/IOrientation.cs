using Microsoft.Xna.Framework;
using NeoLemmixSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public interface IOrientation : IEquatable<IOrientation>
{
    public static ReadOnlyCollection<IOrientation> AllOrientations { get; } = GenerateRotationCollection();

    private static ReadOnlyCollection<IOrientation> GenerateRotationCollection()
    {
        var list = new List<IOrientation>
        {
            DownOrientation.Instance,
            LeftOrientation.Instance,
            UpOrientation.Instance,
            RightOrientation.Instance
        };

        return new ReadOnlyCollection<IOrientation>(list);
    }

    protected static PixelManager Terrain => LevelScreen.CurrentLevel!.Terrain;

    int RotNum { get; }

    Point TopLeftCornerOfLevel();
    Point TopRightCornerOfLevel();
    Point BottomLeftCornerOfLevel();
    Point BottomRightCornerOfLevel();

    Point MoveRight(Point position, int step);
    Point MoveUp(Point position, int step);
    Point MoveLeft(Point position, int step);
    Point MoveDown(Point position, int step);

    /// <summary>
    /// Note for the relativeDirection parameter - Positive x -> right, positive y -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="relativeDirection"></param>
    /// <returns></returns>
    Point Move(Point position, Point relativeDirection);
    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    Point Move(Point position, int dx, int dy);

    bool MatchesHorizontally(Point firstPosition, Point secondPosition);
    bool MatchesVertically(Point firstPosition, Point secondPosition);
    bool FirstIsAboveSecond(Point firstPosition, Point secondPosition);
    bool FirstIsBelowSecond(Point firstPosition, Point secondPosition);
    bool FirstIsToLeftOfSecond(Point firstPosition, Point secondPosition);
    bool FirstIsToRightOfSecond(Point firstPosition, Point secondPosition);

    ActionSprite GetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle);
    ActionSprite GetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle);
    void SetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite leftSprite);
    void SetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite rightSprite);
}