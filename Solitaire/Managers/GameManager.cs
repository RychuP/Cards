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
    public event EventHandler DifficultyChanged;

    public ScreenManager ScreenManager { get; }
    public InputManager InputManager { get; }

    /// <summary>
    /// Stock pile.
    /// </summary>
    public Stock Stock { get; }

    /// <summary>
    /// Waste pile.
    /// </summary>
    public Waste Waste { get; }

    /// <summary>
    /// Foundation piles.
    /// </summary>
    public List<Foundation> Foundations { get; } = new();

    /// <summary>
    /// Tableau piles.
    /// </summary>
    public List<Tableau> Tableaus { get; } = new();

    //backing field
    Difficulty _difficulty;
    public Difficulty Difficulty
    {
        get => _difficulty;
        set
        {
            if (_difficulty == value) return;
            _difficulty = value;
            OnDifficultyChanged();
        }
    }

    public GameManager(Game game) : base(1, 0, CardSuits.AllSuits, CardValues.NonJokers, Fonts.Moire.Regular,
        13, 13, new SolitaireTable(game), Strings.Red, game)
    {
        ScreenManager = new(this);
        InputManager = new(game);
        Difficulty = Difficulty.Easy;

        // create stock pile
        Stock = new Stock(this);
        CardDeck.DealCardsToHand(Stock, CardDeck.Count);

        // create waste pile
        Waste = new Waste(this);

        // create foundation piles
        Place place = Place.Foundation0;
        for (int i = 0; i < 4; i++)
        {
            var foundation = new Foundation(this, place++);
            Foundations.Add(foundation);
        }

        // create tableau piles
        place = Place.Tableau0;
        for (int i = 0; i < 7; i++)
        {
            var tableau = new Tableau(this, place++);
            Tableaus.Add(tableau);
        }

        // register event handlers
        var startScreen = ScreenManager.GetScreen<StartScreen>();
        startScreen.GetButton(Strings.Start).Click += StartScreen_StartButton_OnClick;
        startScreen.GetButton(Strings.Exit).Click += StartScreen_ExitButton_OnClick;

        var optionsScreen = ScreenManager.GetScreen<OptionsScreen>();
        optionsScreen.GetButton(Difficulty.ToString()).Click += OptionsScreen_DifficultyButton_OnClick;
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

    public override void StartPlaying()
    {
        
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

    void OnDifficultyChanged()
    {
        DifficultyChanged?.Invoke(this, EventArgs.Empty);
    }

    void StartScreen_StartButton_OnClick(object o, EventArgs e) =>
        StartPlaying();

    void StartScreen_ExitButton_OnClick(object o, EventArgs e) =>
        Game.Exit();

    void OptionsScreen_DifficultyButton_OnClick(object o, EventArgs e) =>
        Difficulty = Difficulty == Difficulty.Easy ? Difficulty.Hard : Difficulty.Easy;
}