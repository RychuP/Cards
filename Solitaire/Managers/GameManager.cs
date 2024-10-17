using Framework.Assets;
using Framework.Engine;
using Framework.Misc;
using Microsoft.Xna.Framework.Input;
using Solitaire.Gameplay.Piles;
using Solitaire.Misc;
using Solitaire.UI;
using Solitaire.UI.Screens;
using System.Collections.Generic;

namespace Solitaire.Managers;

internal class GameManager : CardGame
{
    public event EventHandler EscapePressed;

    public ScreenManager ScreenManager { get; }
    public InputManager InputManager { get; }

    // piles
    public Stock Stock { get; }
    List<Foundation> _foundations = new();
    List<Tableau> _tableaus = new();

    public GameManager(Game game) : base(1, 0, CardSuits.AllSuits, CardValues.NonJokers, Fonts.Moire.Regular,
        13, 13, new SolitaireTable(game), "Red", game)
    {
        ScreenManager = new(this);
        InputManager = new(game);

        // create stock pile
        Stock = new Stock(this);

        // create foundation piles
        Place place = Place.Foundation0;
        for (int i = 0; i < 4; i++)
        {
            var foundation = new Foundation(this, place++);
            _foundations.Add(foundation);
        }

        // create tableau piles
        place = Place.Tableau0;
        for (int i = 0; i < 7; i++)
        {
            var tableau = new Tableau(this, place++);
            _tableaus.Add(tableau);
        }

        var startScreen = ScreenManager.GetScreen<StartScreen>();
        startScreen.GetButton(Strings.Start).Click += StartScreen_StartButton_OnClick;
        startScreen.GetButton(Strings.Exit).Click += StartScreen_ExitButton_OnClick;
    }

    /// <summary>
    /// Since the game uses mostly mouse for the player input, keyboard is checked just for the escape button. 
    /// </summary>
    void HandleKeyboardEscapeButton()
    {
        if (InputManager.IsNewKeyPress(Keys.Escape))
            OnEscapeButtonPressed();
    }

    public void Update(GameTime gameTime)
    {
        HandleKeyboardEscapeButton();
    }

    void Reset()
    {

    }

    public override void StartPlaying()
    {
        Reset();
    }

    public override void AddPlayer(Player player)
    {
        throw new NotImplementedException();
    }

    public override Player GetCurrentPlayer()
    {
        throw new NotImplementedException();
    }

    public override void DealCardsToPlayers()
    {
        throw new NotImplementedException();
    }

    void OnEscapeButtonPressed()
    {
        if (ScreenManager.Screen is StartScreen)
            Game.Exit();
        EscapePressed?.Invoke(this, EventArgs.Empty);
    }

    void StartScreen_StartButton_OnClick(object o, EventArgs e) =>
        StartPlaying();

    void StartScreen_ExitButton_OnClick(object o, EventArgs e) =>
        Game.Exit();
}