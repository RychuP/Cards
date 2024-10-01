using Poker.UI;
using System;

namespace Poker.Screens;

class GameplayScreen : MenuScreen
{
    PokerCardGame _pokerCardGame;

    public GameplayScreen(ScreenManager screenManager) : base(screenManager, 4)
    { }

    public override void Initialize()
    {
        // create the card game
        _pokerCardGame = new(Game);

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