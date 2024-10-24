﻿using Microsoft.Xna.Framework.Graphics;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.Buttons;
using System.Collections.Generic;

namespace Solitaire.UI.BaseScreens;

abstract internal class MenuScreen : GameScreen
{
    // round cards logo top margin
    public static readonly int LogoMarginTop = 50;

    readonly Vector2 _logoPosition;
    public AnimatedTitle Title { get; }

    /// <summary>
    /// List of buttons in this menu screen.
    /// </summary>
    public List<Button> Buttons { get; } = new();

    public MenuScreen(GameManager gm, Texture2D titleTexture, Direction defaultTitlePosition) : base(gm)
    {
        _logoPosition = new Vector2(GetCenteredPosition(Art.CardsLogo.Bounds).X, LogoMarginTop);
        Title = new AnimatedTitle(this, titleTexture, defaultTitlePosition);
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();
        sb.Draw(Art.CardsLogo, _logoPosition, Color.White);
        sb.End();
    }

    public override void Initialize()
    {
        foreach (var button in Buttons)
            Game.Components.Add(button);
        Game.Components.Add(Title);
        base.Initialize();
    }

    public override void Hide()
    {
        base.Hide();
        foreach (var button in Buttons)
            button.Hide();
    }

    public override void Show()
    {
        base.Show();
        foreach (var button in Buttons)
            button.Show();
    }

    protected void AddButton(Button button)
    {
        Buttons.Add(button);
        AdjustButtonPositions();
    }

    protected void AddButton(string text) =>
        AddButton(new Button(text, GameManager));

    void AdjustButtonPositions()
    {
        // calculate left most x
        int width = (Buttons.Count) * (Button.Width + Button.Spacing) - Button.Spacing;
        int x = (SolitaireGame.Width - width) / 2;

        // adjust positions of the previous buttons
        foreach (var button in Buttons)
        {
            button.SetPosX(x);
            x += Button.Width + Button.Spacing;
        }
    }

    public Button GetButton(string text) => Buttons.Find(b => b.Text == text);
}