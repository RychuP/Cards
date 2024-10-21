using Microsoft.Xna.Framework.Graphics;
using Solitaire.Managers;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.TextBoxes;
using System.Collections.Generic;

namespace Solitaire.UI.Screens;

internal class GameplayScreen : GameScreen
{
    /// <summary>
    /// Card counters displayed above each pile.
    /// </summary>
    readonly List<TextBox> _textBoxes = new();

    public GameplayScreen(GameManager gm) : base(gm)
    { }

    public override void Initialize()
    {
        base.Initialize();
        _textBoxes.Add(new CardCounter(GameManager.Stock, GameManager));
        foreach (var tableau in GameManager.Tableaus)
            _textBoxes.Add(new CardCounter(tableau, GameManager));
        foreach (var foundation in GameManager.Foundations)
            _textBoxes.Add(new CardCounter(foundation, GameManager));
        _textBoxes.Add(new MoveCounter(GameManager.Waste));
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        return;
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();
        if (GameManager.InputManager.IsDragging)
        {
            var pos = GameManager.InputManager.MousePosition.ToVector2();
            sb.DrawString(GameManager.Font, "X", pos , Color.Pink);
        }
        sb.End();
    }

    protected override void OnVisibleChanged(object sender, EventArgs args)
    {
        base.OnVisibleChanged(sender, args);
        foreach (var textBox in _textBoxes)
            textBox.Visible = Visible;
    }
}