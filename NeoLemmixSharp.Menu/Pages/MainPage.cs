using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using NeoLemmixSharp.Menu.Rendering;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : IPage
{
    private readonly MenuInputController _inputController;

    public MainPage(
        ContentManager contentManager,
        MenuInputController inputController,
        BackgroundBrush backgroundBrush)
    {
        _inputController = inputController;
        RootWidget = SetUpPage(contentManager, backgroundBrush);
    }

    private Widget SetUpPage(
        ContentManager contentManager,
        BackgroundBrush backgroundBrush)
    {
        var grid = new Grid
        {
            ShowGridLines = true,
            ColumnSpacing = 8,
            RowSpacing = 8,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        // Set partitioning configuration
        grid.ColumnsProportions.Add(new Proportion());
        grid.ColumnsProportions.Add(new Proportion());
        grid.RowsProportions.Add(new Proportion());
        grid.RowsProportions.Add(new Proportion());

        // Add widgets
        var button = new ImageTextButton
        {
            Image = new TextureRegion(contentManager.Load<Texture2D>("menu/sign_play")),
            Scale = new Vector2(2, 2)
        };
        grid.Widgets.Add(button);

        var longButton = new Button
        {
            Content = new Label { Text = "Long Button" }
        };
        Grid.SetColumn(longButton, 1);
        grid.Widgets.Add(longButton);

        var veryLongButton = new Button
        {
            Content = new Label { Text = "Very Long Button" }
        };
        Grid.SetRow(veryLongButton, 1);
        Grid.SetColumnSpan(veryLongButton, 2);
        grid.Widgets.Add(veryLongButton);

        return grid;
    }

    public Widget RootWidget { get; }

    public void Tick()
    {

    }

    public void Dispose()
    {

    }
}