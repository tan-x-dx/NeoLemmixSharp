using Gum.DataTypes;
using Gum.Managers;
using MonoGameGum.GueDeriving;
using RenderingLibrary.Graphics;

namespace NeoLemmixSharp.Menu.Components;

public sealed class TextureButton : ContainerRuntime
{
    public TextureButton(TextureInfo textureInfo, int scale = 1)
    {
        ChildrenLayout = ChildrenLayout.Regular;
        Width = textureInfo.TextureWidth * scale;
        Height = textureInfo.TextureHeight * scale;

        var texture = new SpriteRuntime
        {
            Name = textureInfo.TextureName,
            SourceFileName = textureInfo.TextureName,
            X = 0f,
            Y = 0f,
            XOrigin = HorizontalAlignment.Center,
            YOrigin = VerticalAlignment.Center,
            Width = textureInfo.TextureWidth * scale,
            Height = textureInfo.TextureHeight * scale,
            WidthUnits = DimensionUnitType.Absolute,
            HeightUnits = DimensionUnitType.Absolute
        };

        Children.Add(texture);
    }
}
