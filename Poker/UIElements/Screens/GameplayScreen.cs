using System;

namespace Poker.UIElements.Screens;

class GameplayScreen : MenuScreen
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

    public override void Show()
    {
        ((PokerGame)Game).GameManager.GameTable.Show();
        base.Show();
    }

    public override void Hide()
    {
        ((PokerGame)Game).GameManager.GameTable.Hide();
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