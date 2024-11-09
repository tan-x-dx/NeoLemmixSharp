using Gum.DataTypes;
using Gum.Managers;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;

namespace NeoLemmixSharp.Menu.Components;

public sealed class TextureButton : ContainerRuntime
{
    public SpriteRuntime Texture { get; }
    public TextRuntime Text { get; }

    public TextureButton(TextureInfo textureInfo)
    {
        ChildrenLayout = ChildrenLayout.Regular;
        Width = textureInfo.TextureWidth;
        Height = textureInfo.TextureHeight;

        Texture = new SpriteRuntime
        {
            Name = "Texture",
            SourceFileName = textureInfo.TextureName,
            X = 0f,
            Y = 0f,
            XOrigin = HorizontalAlignment.Left,
            YOrigin = VerticalAlignment.Top,
            Width = 100f,
            Height = 100f,
            WidthUnits = DimensionUnitType.PercentageOfSourceFile,
            HeightUnits = DimensionUnitType.PercentageOfSourceFile
        };
        Text = new TextRuntime
        {
            Name = "Text",
            Width = 64f,
            X = 0f,
            Y = 0f
        };

        Children.Add(Texture);
        Children.Add(Text);
    }
}
