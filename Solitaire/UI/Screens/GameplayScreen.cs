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
    readonly List<TextBox> _cardCounters = new();

    public GameplayScreen(GameManager gm) : base(gm)
    {
        
    }

    public override void Initialize()
    {
        base.Initialize();
        _cardCounters.Add(new CardCounter(GameManager.Stock, GameManager));
        foreach (var tableau in GameManager.Tableaus)
            _cardCounters.Add(new CardCounter(tableau, GameManager));
        foreach (var foundation in GameManager.Foundations)
            _cardCounters.Add(new CardCounter(foundation, GameManager));
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        return;
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();
        sb.End();
    }

    protected override void OnVisibleChanged(object sender, EventArgs args)
    {
        base.OnVisibleChanged(sender, args);
        foreach (var textBox in _cardCounters)
            textBox.Visible = Visible;
    }
}