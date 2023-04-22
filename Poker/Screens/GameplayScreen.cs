using Poker.UI;
using System;

namespace Poker.Screens;

internal class GameplayScreen : MenuScreen
{
    PokerCardGame _pokerCardGame;

    public GameplayScreen() : base("card_outlines", 5)
    {

    }

    public override void Initialize()
    {
        // create the card game
        _pokerCardGame = new(ScreenManager.Game);

        // create buttons
        var raise = new Button("Raise", -Button.Width, ButtonRow, ScreenManager.Game);
        var check = new Button("Check", -Button.Width, ButtonRow, ScreenManager.Game);
        var fold = new Button("Fold", -Button.Width, ButtonRow, ScreenManager.Game);
        var call = new Button("Call", -Button.Width, ButtonRow, ScreenManager.Game);
        var allin = new Button("All In", -Button.Width, ButtonRow, ScreenManager.Game);

        // event handlers
        raise.Click += RaiseButton_OnClick;
        check.Click += CheckButton_OnClick;
        fold.Click += FoldButton_OnClick;
        call.Click += CallButton_OnClick;

        // add buttons
        Buttons.AddRange(new Button[] { raise, check, fold, call, allin });

        base.Initialize();
    }

    public override void LoadContent()
    {
        _pokerCardGame.LoadContent();
        base.LoadContent();
    }

    public override void Show()
    {
        _pokerCardGame.GameTable.Show();
        base.Show();
    }

    public override void Hide()
    {
        _pokerCardGame.GameTable.Hide();
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