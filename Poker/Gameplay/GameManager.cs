using Framework.Assets;
using Framework.Engine;
using Framework.Misc;
using Framework.UI;
using Poker.Gameplay.Players;
using Poker.UI;
using Poker.UI.Screens;
using System;

namespace Poker.Gameplay;

class GameManager : CardGame
{
    readonly CardPile _cardPile;
    readonly CommunityCards _communityCards;
    readonly BetComponent _betComponent;
    GameState _gameState = GameState.None;

    public GameManager(Game game) : base(1, 0, CardSuits.AllSuits, CardValues.NonJokers, Fonts.Moire.Regular,
        Constants.MinPlayers, Constants.MaxPlayers, new PokerTable(game), Constants.DefaultTheme, game)
    {
        // create card pile
        _cardPile = new CardPile(this);
        game.Components.Add(_cardPile);

        // create players
        AddPlayer(new HumanPlayer(Gender.Male, this));
        for (int place = 1; place < MaximumPlayers; place++)
        {
            (string name, Gender gender) = GetRandomPerson();
            AddPlayer(new AIPlayer(name, gender, place, this));
        }

        // create community cards cards holder
        _communityCards = new CommunityCards(this);

        // create bet component
        _betComponent = new BetComponent(this);
        game.Components.Add(_betComponent);
    }
    
    public GameState State
    {
        get => _gameState;
        set
        {
            if (value == _gameState) return;
            var prevGameState = _gameState;
            _gameState = value;
            OnGameStateChanged(prevGameState, value);
        }
    }

    public void Update()
    {
        if (Game.Services.GetService<ScreenManager>().ActiveScreen is not GameplayScreen) return;

        switch (State)
        {
            case GameState.Shuffling:
                if (!CheckForRunningAnimations<CardPile>())
                {
                    State = GameState.Dealing;
                    Deal();
                }
                break;

            case GameState.Dealing:
                if (!CheckForRunningAnimations<AnimatedCardGameComponent>())
                {
                    State = GameState.FirstBet;
                    _cardPile.SlideUp();
                }
                break;

            case GameState.FirstBet:
                if (!CheckForRunningAnimations<CardPile>())
                {
                    
                }
                break;
        }
    }

    public override void StartPlaying()
    {
        State = GameState.Shuffling;

        Reset();
        Dealer.Shuffle();
        _cardPile.ShowAndShuffle();
    }

    void Reset()
    {
        foreach (var player in Players)
            ((PokerBettingPlayer)player).Reset();
    }

    public override void Deal()
    {
        DateTime startTime = DateTime.Now;

        // deal player cards
        for (int dealIndex = 0; dealIndex < 2; dealIndex++)
        {
            for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
            {
                var player = this[playerIndex];
                TraditionalCard card = Dealer.DealCardToHand(player.Hand);
                bool flip = player is HumanPlayer;

                // calculate start time and add deal animation
                player.AddDealAnimation(card, flip, startTime);
                startTime += Constants.DealAnimationDuration;
            }
        }

        // deal community cards
        for (int i = 0; i < 3; i++)
        {
            TraditionalCard card = Dealer.DealCardToHand(_communityCards.Hand);
            _communityCards.AddDealAnimation(card, true, startTime);
            startTime += Constants.DealAnimationDuration;
        }
    }

    /// <summary>
    /// Checks for running animations.
    /// </summary>
    /// <typeparam name="T">The type of animation to look for.</typeparam>
    /// <returns>True if a running animation of the desired type is found and
    /// false otherwise.</returns>
    internal bool CheckForRunningAnimations<T>() where T : AnimatedGameComponent
    {
        for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)
            if (Game.Components[componentIndex] is T animComponent && animComponent.IsAnimating)
                return true;
        return false;
    }

    /// <summary>
    /// Indexer that returns <see cref="PokerBettingPlayer"/>.
    /// </summary>
    /// <returns><see cref="PokerBettingPlayer"/> with the given index.</returns>
    public PokerBettingPlayer this[int index] =>
        Players[index] as PokerBettingPlayer;

    /// <summary>
    /// Changes card theme for the game.
    /// </summary>
    /// <param name="theme">New card theme.</param>
    public void SetTheme(string theme)
    {
        if (Theme == theme) return;
        else if (theme != Constants.RedThemeText && theme != Constants.BlueThemeText) return;
        else
        {
            Theme = theme;
            OnThemeChanged(theme);
        }
    }

    /// <summary>
    /// Selects a random name from a predefined list and an associated gender.
    /// </summary>
    /// <returns></returns>
    (string, Gender) GetRandomPerson()
    {
        string name = string.Empty;
        Gender gender = (Gender)Game.Services.GetService<Random>().Next(2);

        // get the count of 50% of the names (first half is male, second female)
        int nameCount = Constants.Names.Length / 2;
        do
        {
            // get random index taking into consideration the appropriate half of collection
            int offset = gender == Gender.Male ? 0 : nameCount;
            int index = Game.Services.GetService<Random>().Next(0, nameCount);

            // retrieve the name and cap the index (just in case)
            name = Constants.Names[Math.Min(index + offset, Constants.Names.Length - 1)];

            // repeat until a unique name is selected
        } while (Players.Find(p => p.Name == name) is PokerCardsHolder);
        
        return (name, gender);
    }

    /// <summary>
    /// Adds a player to the game.
    /// </summary>
    /// <param name="player">The player to add.</param>
    public override void AddPlayer(Player player)
    {
        if (player is PokerCardsHolder && Players.Count < MaximumPlayers)
            Players.Add(player);
    }

    public override void CheckRules()
    {
        base.CheckRules();
    }

    public override Player GetCurrentPlayer()
    {
        throw new NotImplementedException();
    }

    void OnThemeChanged(string theme)
    {
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme));
    }

    void OnGameStateChanged(GameState prevGameState, GameState newGameState)
    {
        GameStateChanged?.Invoke(this, new GameStateEventArgs(prevGameState, newGameState));
    }

    public event EventHandler<ThemeChangedEventArgs> ThemeChanged;
    public event EventHandler<GameStateEventArgs> GameStateChanged;
}