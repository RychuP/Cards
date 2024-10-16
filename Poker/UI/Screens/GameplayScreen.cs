using System.Collections.Generic;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Poker.Gameplay;
using Poker.Gameplay.Players;
using Poker.UI.AnimatedGameComponents;
using Poker.UI.BaseScreens;

namespace Poker.UI.Screens;

class GameplayScreen : MenuGameScreen
{
    public Button CheckButton { get; private set; }
    public Button RaiseButton { get; private set; }
    public Button CallButton { get; private set; }
    public Button FoldButton { get; private set; }
    public Button ClearButton { get; private set; }
    public Button AllInButton { get; private set; }
    public Button RestartButton { get; private set; }
    public Button DealButton {  get; private set; }
    public Button ExitButton {  get; private set; }

    public GameplayScreen(ScreenManager screenManager) : base(screenManager, 6)
    { }

    public override void Initialize()
    {
        var gm = Game.Services.GetService<GameManager>();
        var humanPlayer = gm[0] as HumanPlayer;

        // create buttons
        CheckButton = new Button(Strings.Check, Game);
        RaiseButton = new Button(Strings.Raise, Game);
        CallButton = new Button(Strings.Call, Game);
        FoldButton = new Button(Strings.Fold, Game);
        ClearButton = new Button(Strings.Clear, Game);
        AllInButton = new Button(Strings.AllIn, Game);
        RestartButton = new Button(Strings.Restart, Game);
        DealButton = new Button(Strings.Deal, Game);
        ExitButton = new Button(Strings.Exit, Game);
        Buttons.AddRange(new[] { AllInButton, CheckButton, RaiseButton, CallButton, FoldButton, 
            ClearButton, RestartButton, DealButton, ExitButton });
        humanPlayer.AssignClickHandlers();
        gm.StateChanged += GameManager_OnStateChanged;

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
        for (int i = 0; i < gm.PlayerCount; i++)
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

    /// <summary>
    /// Shows available buttons for the player depending on the bet values.
    /// </summary>
    /// <param name="currentTableBet">Current highest bet on the table.</param>
    /// <param name="currentPlayerBet">Current player bet.</param>
    /// <param name="startingPlayerBet">Bet that the player started with during their turn.</param>
    /// <param name="playerBalance">Player's balance.</param>
    public void ShowButtons(int currentTableBet, int currentPlayerBet, int startingPlayerBet, 
        int playerBalance, bool checkAvailable)
    {
        List<Button> buttonsToShow = new(Buttons.Count)
        {
            AllInButton, FoldButton
        };

        if (startingPlayerBet != currentPlayerBet)
            buttonsToShow.Add(ClearButton);

        if (currentPlayerBet == currentTableBet && checkAvailable)
        {
            buttonsToShow.Add(CheckButton);
        }
        else if (currentPlayerBet <= currentTableBet && currentPlayerBet + playerBalance >= currentTableBet)
        {
            buttonsToShow.Add(CallButton);
        }
        else if (currentPlayerBet > currentTableBet)
        {
            buttonsToShow.Add(RaiseButton);
        }

        ShowButtons(buttonsToShow);        
    }

    /// <summary>
    /// Shows provided buttons and hides the others.
    /// </summary>
    void ShowButtons(List<Button> buttons)
    {
        var gm = Game.Services.GetService<GameManager>();
        if (gm.CheckForRunningAnimations<AnimatedCardGameComponent>())
            return;

        // show buttons
        foreach (var button in Buttons)
        {
            if (buttons.Contains(button))
                button.Show();
            else
                button.Hide();
        }

        // calculate left most button position
        int _leftMostButtonX = (Constants.GameWidth - buttons.Count * Constants.ButtonWidthWithPadding
            + Constants.ButtonPadding) / 2;

        // arrange buttons
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].ChangePosition(_leftMostButtonX + Constants.ButtonWidthWithPadding * i);
    }

    /// <summary>
    /// Hides all the buttons.
    /// </summary>
    public void HideButtons()
    {
        foreach (var button in Buttons)
            button.Hide();
    }

    void GameManager_OnStateChanged(object o, GameStateEventArgs e)
    {
        if (o is not GameManager gm) return;

        switch (e.NewState)
        {
            case GameState.Waiting:
                List<Button> buttons = new();

                // there are at least two players remaining that are not bankrupt
                int remainingPlayerCount = gm.PlayerCount - gm.BankruptPlayerCount;
                var humanPlayer = gm[0];
                if (remainingPlayerCount >= 2 && !humanPlayer.IsBankrupt)
                    buttons.Add(DealButton);
                else
                    buttons.Add(RestartButton);

                buttons.Add(ExitButton);
                ShowButtons(buttons);
                break;

            default:
                HideButtons();
                break;
        }
    }
}