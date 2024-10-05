using System;
using Framework.Assets;
using Microsoft.Xna.Framework.Graphics;
using Poker.Gameplay;
using Poker.UI.BaseGameScreens;

namespace Poker.UI.Screens;

class GameplayScreen : ButtonGameScreen
{
    public GameplayScreen(ScreenManager screenManager) : base(screenManager, 4)
    { }

    public override void Initialize()
    {
        // create buttons
        var raise = new Button(Constants.ButtonRaiseText, Game);
        var check = new Button(Constants.ButtonCheckText, Game);
        var fold = new Button(Constants.ButtonFoldText, Game);
        var call = new Button(Constants.ButtonCallText, Game);
        var allin = new Button(Constants.ButtonAllInText, Game);

        // event handlers
        raise.Click += RaiseButton_OnClick;
        check.Click += CheckButton_OnClick;
        fold.Click += FoldButton_OnClick;
        call.Click += CallButton_OnClick;

        // add buttons
        Buttons.AddRange(new Button[] { raise, check, fold, call, allin });

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Texture = Art.TableCardOutlines;
        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        var gm = Game.Services.GetService<GameManager>();
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        // draw player names
        for (int i = 0; i < Constants.MaxPlayers; i++)
        {
            var player = gm[i];
            sb.DrawString(gm.Font, player.Name, player.NamePosition, Color.WhiteSmoke);
        }

        sb.End();
    }

    public override void Show()
    {
        Game.Services.GetService<GameManager>().GameTable.Show();
        base.Show();
    }

    public override void Hide()
    {
        Game.Services.GetService<GameManager>().GameTable.Hide();
        base.Hide();
    }

    void RaiseButton_OnClick(object o, EventArgs e)
    {
        throw new NotImplementedException();
    }

    void CheckButton_OnClick(object o, EventArgs e)
    {
        throw new NotImplementedException();
    }

    void FoldButton_OnClick(object o, EventArgs e)
    {
        throw new NotImplementedException();
    }

    void CallButton_OnClick(object o, EventArgs e)
    {
        throw new NotImplementedException();
    }
}