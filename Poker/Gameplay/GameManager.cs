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
using Poker.Gameplay.Chips;
using System.Linq;
using Poker.Gameplay.Rules;

namespace Poker.Gameplay;

class GameManager : CardGame, IGlobalManager
{
    public event EventHandler<ThemeChangedEventArgs> ThemeChanged;
    public event EventHandler<GameStateEventArgs> GameStateChanged;
    public event EventHandler<PlayerChangedEventArgs> PlayerChanged;

    readonly AnimatedCardPile _animatedCardPile;
    readonly CommunityCards _communityCards;
    readonly BetComponent _betComponent;
    readonly ScreenManager _screenManager;
    readonly Dealer _dealer;

    /// <summary>
    /// Used to make sure that animations finish playing before the next player takes turn.
    /// </summary>
    bool _ignorePlayerAnimations;

    /// <summary>
    /// Used during showdown to iterate over the animated card components only once.
    /// </summary>
    bool _allCardsAreFaceUp;

    // used for pausing and resuming gameplay components
    readonly List<DrawableGameComponent> _pauseEnabledComponents = new();
    readonly List<DrawableGameComponent> _pauseVisibleComponents = new();

    // backing field
    GameState _state = GameState.None;
    /// <summary>
    /// Current state of the game.
    /// </summary>
    public GameState State
    {
        get => _state;
        set
        {
            if (value == _state) return;
            var prevGameState = _state;
            _state = value;
            OnGameStateChanged(prevGameState, value);
        }
    }

    // backing field
    PokerBettingPlayer _currentPlayer;
    /// <summary>
    /// Player currently taking their turn.
    /// </summary>
    public PokerBettingPlayer CurrentPlayer
    {
        get => _currentPlayer;
        set
        {
            if (value == _currentPlayer) return;
            var prevPlayer = _currentPlayer;
            _currentPlayer = value;
            OnPlayerChanged(prevPlayer, value);
        }
    }

    public int PlayerCount => Players.Count;

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

        // create rules
        var gameEndRule = new GameEndRule(this);
        Rules.Add(gameEndRule);
    }

    public Dealer GetPokerDealer() => _dealer;

    public override Player GetCurrentPlayer() => CurrentPlayer;

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
                _dealer.Shuffle();
                // wait for the shuffling animations to finish
                if (!CheckForRunningAnimations<AnimatedCardPile>())
                {
                    // deal cards and change state
                    DealCardsToPlayers();
                    State = GameState.Dealing;
                }
                break;

            case GameState.Dealing:
                // wait for the player cards animations to finish
                if (!CheckForRunningAnimations<AnimatedCardGameComponent>())
                {
                    // hide the card pile and change state
                    _animatedCardPile.Hide();
                    State = GameState.Preflop;

                    // deal blind tokens
                    _betComponent.IssueBlindChips();

                    _ignorePlayerAnimations = false;
                }
                break;

            case GameState.Preflop:
                HandlePreflop();
                break;

            case GameState.Flop:
                HandleCardDealingStage(3);
                break;

            case GameState.FlopBet:
                HandleBettingStage();
                break;

            case GameState.Turn:
                HandleCardDealingStage(1);
                break;

            case GameState.TurnBet:
                HandleBettingStage();
                break;

            case GameState.River:
                HandleCardDealingStage(1);
                break;

            case GameState.RiverBet:
                HandleBettingStage();
                break;

            case GameState.Showdown:
                HandleShowdown();
                break;
        }
    }

    void HandleShowdown()
    {
        if (!CheckForRunningAnimations<AnimatedGameComponent>())
        {
            if (_allCardsAreFaceUp)
            {
                
                
            }
            // make sure all the cards are flipped face up
            else
            {
                foreach (var component in Game.Components)
                {
                    if (component is AnimatedCardGameComponent cardComponent
                        && cardComponent.IsFaceDown)
                    {
                        cardComponent.AddAnimation(new FlipGameComponentAnimation());
                    }
                }
                _allCardsAreFaceUp = true;
            }
            

            // cards
            foreach (var player in Players)
            {
                if (player is AIPlayer aiPlayer
                    && aiPlayer.State != PlayerState.Folded
                    && aiPlayer.State != PlayerState.Bankrupt)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        
                    }
                }
            }
        }
    }

    void HandlePreflop()
    {
        // wait for the player chips animations to finish
        if (!CheckForRunningAnimations<AnimatedChipComponent>() ||
            _ignorePlayerAnimations)
        {
            _ignorePlayerAnimations = true;

            // establish who the current player is if not done already
            if (CurrentPlayer is null)
            {
                for (int i = 0; i < PlayerCount; i++)
                {
                    if (this[i].BlindChip is BigBlindChip)
                        CurrentPlayer = GetNextPlayer(this[i]);
                }
            }

            BetOrSkip();
        }
    }

    void HandleCardDealingStage(int cardCount)
    {
        if (!CheckForRunningAnimations<AnimatedChipComponent>())
        {
            // show card pile
            if (_animatedCardPile.Position != Constants.CardPileVisiblePosition &&
                !_animatedCardPile.IsAnimating)
                _animatedCardPile.SlideDown();

            // wait for the card pile animation to finish
            if (!CheckForRunningAnimations<AnimatedCardPile>())
            {
                DealCommunityCards(cardCount);
                MoveToNextStage();
            }
        }
    }

    void HandleBettingStage()
    {
        // wait for the community cards animations to finish
        if (!CheckForRunningAnimations<AnimatedGameComponent>()
            || _ignorePlayerAnimations)
        {
            // hide card pile
            if (_animatedCardPile.Position != Constants.CardPileHiddenPosition &&
                !_animatedCardPile.IsAnimating)
                _animatedCardPile.SlideUp();

            _ignorePlayerAnimations = true;

            BetOrSkip();
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

    void BetOrSkip()
    {
        if (CurrentPlayer.State == PlayerState.Folded || CurrentPlayer.State == PlayerState.Bankrupt)
            ChangeCurrentPlayer();
        else
            _betComponent.HandleBetting(CurrentPlayer);

        CheckRules();
    }

    public void MoveToNextStage()
    {
        State++;

        // betting round starts with the small blind player
        for (int i = 0; i < PlayerCount; i++)
        {
            var player = this[i];
            player.State = PlayerState.Waiting;
            if (player.BlindChip is SmallBlindChip)
                CurrentPlayer = player;
        }
    }

    /// <summary>
    /// Changes the current player to the next one to take turn.
    /// </summary>
    public void ChangeCurrentPlayer()
    {
        CurrentPlayer = GetNextPlayer(CurrentPlayer);
        _ignorePlayerAnimations = false;
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
        _betComponent.ShowCommunityChips();
    }

    void StartNewRound()
    {
        // prepare players
        foreach (var player in Players)
            ((PokerBettingPlayer)player).StartNewRound();

        // return community cards
        _communityCards.ReturnCardsToDealer();
    }

    void Reset()
    {
        // reset players
        foreach (var player in Players)
            ((PokerBettingPlayer)player).Reset();

        // return community cards
        _communityCards.ReturnCardsToDealer();

        // reset components
        _betComponent.Reset();
        _animatedCardPile.Reset();

        // reset fields
        CurrentPlayer = null;

        _allCardsAreFaceUp = false;
    }

    public override void DealCardsToPlayers()
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
    }

    public void DealCommunityCards(int amount)
    {
        DateTime startTime = DateTime.Now;

        // deal community cards
        for (int i = 0; i < amount; i++)
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

    /// <summary>
    /// Gets the <see cref="PokerBettingPlayer"/> that follows given player in the parameter.
    /// </summary>
    /// <param name="player"><see cref="PokerBettingPlayer"/> to find the next player from.</param>
    /// <returns><see cref="PokerBettingPlayer"/> in the next place after the given player.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public PokerBettingPlayer GetNextPlayer(PokerBettingPlayer player)
    {
        if (!Players.Contains(player))
            throw new ArgumentException("No such player in the current game.", nameof(player));

        int nextPlace = player.Place == PlayerCount - 1 ? 0 : player.Place + 1;
        return this[nextPlace];
    }

    void OnThemeChanged(string theme)
    {
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme));
    }

    void OnGameStateChanged(GameState prevGameState, GameState newGameState)
    {
        GameStateChanged?.Invoke(this, new GameStateEventArgs(prevGameState, newGameState));
    }

    void GameEndRule_OnRuleMatch(object o, GameEndEventArgs e)
    {
        _betComponent.TransferPotToWinner(e.Winner);
    }

    void OnPlayerChanged(PokerBettingPlayer prevPlayer, PokerBettingPlayer newPlayer)
    {
        _ignorePlayerAnimations = false;

        var args = new PlayerChangedEventArgs(prevPlayer, newPlayer);
        PlayerChanged?.Invoke(this, args);
    }
}