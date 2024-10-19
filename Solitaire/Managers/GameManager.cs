using Framework.Assets;
using Framework.Engine;
using Framework.Misc;
using Solitaire.Gameplay.Piles;
using Solitaire.Gameplay.Rules;
using Solitaire.Misc;
using Solitaire.UI;
using Solitaire.UI.AnimatedGameComponents;
using Solitaire.UI.Screens;
using System.Collections.Generic;

namespace Solitaire.Managers;

internal class GameManager : CardGame
{
    public event EventHandler DifficultyChanged;

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
        winScreen.RestartButton.Click += (o, e) => StartPlaying();
        startScreen.StartButton.Click += (o, e) => StartPlaying();
        startScreen.ExitButton.Click += (o, e) => Game.Exit();
        optionsScreen.DifficultyButton.Click += OptionsScreen_DifficultyButton_OnClick;
    }

    public void Update(GameTime gameTime)
    {
        WinRule.Check();
    }

    /// <summary>
    /// Returns the pile whose bounds contain given position. Used to find card destinations for mouse drops.
    /// </summary>
    /// <remarks>Only tableaus and foundations are searched.</remarks>
    public Pile GetPileFromPosition(Point position)
    {
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
        Stock.Shuffle();
        Stock.DealTablueaCards();
        foreach (var tablea in Tableaus)
            (tablea.AnimatedPile as AnimatedTableau).SetUpCards();
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


    void OptionsScreen_DifficultyButton_OnClick(object o, EventArgs e) =>
        Difficulty = Difficulty == Difficulty.Easy ? Difficulty.Hard : Difficulty.Easy;
}