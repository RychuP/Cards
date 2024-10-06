using Microsoft.Xna.Framework.Input;
using Framework.Assets;
using Framework.Engine;
using Framework.Misc;
using Framework.UI;
using Poker.Gameplay.Players;
using Poker.UI;
using Poker.UI.Screens;
using System;
using System.Collections.Generic;
using Poker.UI.BaseScreens;
using Poker.UI.AnimatedGameComponents;

namespace Poker.Gameplay;

class GameManager : CardGame, IGlobalManager
{
    readonly AnimatedCardPile _animatedCardPile;
    readonly CommunityCards _communityCards;
    readonly BetComponent _betComponent;
    readonly ScreenManager _screenManager;
    readonly Dealer _dealer;

    // used for pausing and resuming gameplay components
    readonly List<DrawableGameComponent> _pauseEnabledComponents = new();
    readonly List<DrawableGameComponent> _pauseVisibleComponents = new();

    GameState _gameState = GameState.None;

    public GameManager(Game game, ScreenManager screenManager) 
        : base(1, 0, CardSuits.AllSuits, CardValues.NonJokers, Fonts.Moire.Regular,
        Constants.MinPlayers, Constants.MaxPlayers, new PokerTable(game), Constants.DefaultTheme, game)
    {
        _screenManager = screenManager;

        // create animated card pile
        _animatedCardPile = new AnimatedCardPile(this);
        game.Components.Add(_animatedCardPile);

        // create dealer hand
        _dealer = new(Dealer);

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
        HandleInput();

        if (_screenManager.ActiveScreen is GameplayScreen)
            HandleGameplay();
    }

    void HandleGameplay()
    {
        switch (State)
        {
            case GameState.Shuffling:
                if (!CheckForRunningAnimations<AnimatedCardPile>())
                {
                    State = GameState.Dealing;
                    Deal();
                }
                break;

            case GameState.Dealing:
                if (!CheckForRunningAnimations<AnimatedCardGameComponent>())
                {
                    State = GameState.FirstBet;
                    _animatedCardPile.SlideUp();
                }
                break;

            case GameState.FirstBet:
                if (!CheckForRunningAnimations<AnimatedCardPile>())
                {

                }
                break;
        }
    }

    void HandleInput()
    {
        if (InputManager.IsNewKeyPress(Keys.Escape))
        {
            if (_screenManager.ActiveScreen is GameplayScreen)
                PauseGame();
            else if (_screenManager.ActiveScreen is PauseScreen)
                StopPlaying();
            else if (_screenManager.ActiveScreen is StartScreen)
                Game.Exit();
        }
    }

    /// <summary>
    /// Called when pressed escape or clicked on exit button in the <see cref="PauseScreen"/>.
    /// </summary>
    public void StopPlaying()
    {
        _screenManager.ShowScreen<StartScreen>();
        _betComponent.Visible = false;
    }

    /// <summary>
    /// Called when pressed escape in the <see cref="GameplayScreen"/>.
    /// </summary>
    void PauseGame()
    {
        PauseGameplayComponents();
        _screenManager.ShowScreen<PauseScreen>();
    }

    /// <summary>
    /// Called when clicked continue button in the <see cref="PauseScreen"/>.
    /// </summary>
    public void ResumeGame()
    {
        ResumePausedGameplayComponents();
        _screenManager.ShowScreen<GameplayScreen>();
    }

    /// <summary>
    /// Pause the game.
    /// </summary>
    void PauseGameplayComponents()
    {
        _pauseEnabledComponents.Clear();
        _pauseVisibleComponents.Clear();

        // Hide and disable all components which are related to the gameplay screen
        foreach (IGameComponent component in Game.Components)
        {
            if (component is BetComponent ||
                component is AnimatedGameComponent ||
                component is GameTable)
            {
                DrawableGameComponent pauseComponent = (DrawableGameComponent)component;
                if (pauseComponent.Enabled)
                {
                    _pauseEnabledComponents.Add(pauseComponent);
                    pauseComponent.Enabled = false;
                }
                if (pauseComponent.Visible)
                {
                    _pauseVisibleComponents.Add(pauseComponent);
                    pauseComponent.Visible = false;
                }
            }
        }
    }

    /// <summary>
    /// Returns from pause.
    /// </summary>
    void ResumePausedGameplayComponents()
    {
        // Reveal and enable all previously hidden components
        foreach (DrawableGameComponent component in _pauseEnabledComponents)
            component.Enabled = true;

        foreach (DrawableGameComponent component in _pauseVisibleComponents)
            component.Visible = true;
    }

    public override void StartPlaying()
    {
        Reset();

        State = GameState.Shuffling;
        _animatedCardPile.ShowAndShuffle();
    }

    void Reset()
    {
        // remove all gameplay
        //foreach (IGameComponent component in Game.Components)
        //{
        //    if (component is GameScreen || component is GameTable ||
        //        component is Button || component is BetComponent)
        //        continue;
        //    else
        //        Game.Components.Remove(component);
        //}

        // return all cards to dealer and show relevant components
        foreach (var player in Players)
            ((PokerBettingPlayer)player).Reset(_dealer);
        _communityCards.Reset(_dealer);

        // reset components
        _betComponent.Reset();
        _animatedCardPile.Reset();

        // shuffle cards
        _dealer.Shuffle();
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
                TraditionalCard card = _dealer.DealCardToHand(player.Hand);
                bool flip = player is HumanPlayer;

                // calculate start time and add deal animation
                player.AddDealAnimation(card, flip, startTime);
                startTime += Constants.DealAnimationDuration;
            }
        }

        // deal community cards
        for (int i = 0; i < 3; i++)
        {
            TraditionalCard card = _dealer.DealCardToHand(_communityCards.Hand);
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