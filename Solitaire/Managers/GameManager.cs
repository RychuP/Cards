using Framework.Assets;
using Framework.Engine;
using Framework.Misc;
using Solitaire.Gameplay.Piles;
using Solitaire.Gameplay.Rules;
using Solitaire.Misc;
using Solitaire.UI;
using Solitaire.UI.Screens;
using System.Collections.Generic;

namespace Solitaire.Managers;

internal class GameManager : CardGame
{
    /// <summary>
    /// Raised when the difficulty is changed in settings.
    /// </summary>
    public event EventHandler DifficultyChanged;

    /// <summary>
    /// Raised when the game ends and it's time to clean up.
    /// </summary>
    public event EventHandler GameEnd;

    /// <summary>
    /// Raised just before the game starts to set everything up ready to begin playing.
    /// </summary>
    public event EventHandler GameInit;

    /// <summary>
    /// Raised when the game starts.
    /// </summary>
    public event EventHandler GameStart;

    public ScreenManager ScreenManager { get; }
    public InputManager InputManager { get; }
    
    /// <summary>
    /// Game rule that checks for win conditions.
    /// </summary>
    public WinRule WinRule { get; }

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
        Difficulty = Difficulty.Easy;
        WinRule = new(this);
        InputManager = new(this);
        InputManager.EscapePressed += InputManager_OnEscapePressed;
        ScreenManager = new(this);

        // create stock pile
        Stock = new Stock(this);
        CardDeck.DealCardsToHand(Stock, CardDeck.Count);

        // create waste pile
        Waste = new Waste(this);

        // create foundation piles
        PilePlace place = PilePlace.Foundation0;
        for (int i = 0; i < 4; i++)
        {
            var foundation = new Foundation(this, place++);
            Foundations.Add(foundation);
        }

        // create tableau piles
        place = PilePlace.Tableau0;
        for (int i = 0; i < 7; i++)
        {
            var tableau = new Tableau(this, place++);
            Tableaus.Add(tableau);
        }

        // register event handlers
        var startScreen = ScreenManager.GetScreen<StartScreen>();
        var optionsScreen = ScreenManager.GetScreen<OptionsScreen>();
        var winScreen = ScreenManager.GetScreen<WinScreen>();
        var pauseScreen = ScreenManager.GetScreen<PauseScreen>();
        WinRule.RuleMatch += (o, e) => StopPlaying();
        winScreen.RestartButton.Click += (o, e) => StartPlaying(); // that is correct
        startScreen.StartButton.Click += (o, e) => StartPlaying();
        startScreen.ExitButton.Click += (o, e) => Game.Exit();
        optionsScreen.DifficultyButton.Click += OptionsScreen_DifficultyButton_OnClick;
        optionsScreen.ThemeButton.Click += OptionsScreen_ThemeButton_OnClick;
        pauseScreen.ExitButton.Click += (o, e) => StopPlaying();
    }

    public void Update(GameTime gameTime)
    {
        WinRule.Check();
    }

    /// <summary>
    /// Returns the pile whose bounds contain given position. Used to find card destinations for mouse drops.
    /// </summary>
    /// <remarks>Stock is not considered.</remarks>
    public Pile GetPileFromPosition(Point position)
    {
        if (Waste.Bounds.Contains(position))
        {
            return Waste;
        }
        foreach (var tableau in Tableaus)
        {
            if (tableau.Bounds.Contains(position))
                return tableau;
        }
        foreach (var foundation in Foundations)
        {
            if (foundation.Bounds.Contains(position))
                return foundation;
        }
        return null;
    }

    /// <summary>
    /// Checks if two cards are of consecutive value. The first card must be of lower value.
    /// </summary>
    public bool CheckConsecutiveValue(TraditionalCard c1, TraditionalCard c2)
    {
        int val1 = GetCardValue(c1);
        int val2 = GetCardValue(c2);
        if (val1 + 1 == val2)
            return true;
        return false;
    }

    public static bool CheckOppositeColor(TraditionalCard c1, TraditionalCard c2) =>
        (c1.IsRed() && c2.IsBlack()) || (c1.IsBlack() && c2.IsRed());

    public override void StartPlaying()
    {
        OnGameInit();
        OnGameStart();
    }

    void StopPlaying()
    {
        OnGameEnd();
    }

    // for some reason this is buggy...
    void RestartPlaying()
    {
        StopPlaying();
        StartPlaying();
    }

    /// <summary>
    /// Returns number of cards to be drawn from stock based on the difficulty.
    /// </summary>
    public int GetDrawCount()
    {
        return Difficulty switch
        {
            Difficulty.Medium => 2,
            Difficulty.Hard => 3,
            _ => 1
        };
    }

    #region Unused CardGame methods
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
    #endregion

    void OnDifficultyChanged()
    {
        DifficultyChanged?.Invoke(this, EventArgs.Empty);
    }

    void OnGameInit()
    {
        GameInit?.Invoke(this, EventArgs.Empty);
    }

    void OnGameStart()
    {
        GameStart?.Invoke(this, EventArgs.Empty);
    }

    void OnGameEnd()
    {
        GameEnd?.Invoke(this, EventArgs.Empty);
    }

    void OptionsScreen_DifficultyButton_OnClick(object o, EventArgs e)
    {
        Difficulty = Difficulty switch
        {
            Difficulty.Easy => Difficulty.Medium,
            Difficulty.Medium => Difficulty.Hard,
            _ => Difficulty.Easy,
        };
    }

    void OptionsScreen_ThemeButton_OnClick(object o, EventArgs e)
    {
        Theme = Theme == Strings.Red ? Strings.Blue : Strings.Red;
    }

    void InputManager_OnEscapePressed(object o, EventArgs e)
    {
        if (ScreenManager.Screen is PauseScreen)
            StopPlaying();
    }
}