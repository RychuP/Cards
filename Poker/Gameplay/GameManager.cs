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
using Poker.UI.AnimatedGameComponents;
using Poker.Gameplay.Chips;
using System.Linq;

namespace Poker.Gameplay;

class GameManager : CardGame, IGlobalManager
{
    public event EventHandler<ThemeChangedEventArgs> ThemeChanged;
    public event EventHandler<GameStateEventArgs> GameStateChanged;
    public event EventHandler<PlayerChangedEventArgs> PlayerChanged;

    readonly AnimatedCardPile _animatedCardPile;
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

    /// <summary>
    /// Represents the 5 community cards placed on the table.
    /// </summary>
    public CommunityCards CommunityCards { get; }

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
            OnStateChanged(prevGameState, value);
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
            OnCurrentPlayerChanged(prevPlayer, value);
        }
    }

    /// <summary>
    /// Number of players that can take action in the current betting round.
    /// </summary>
    public int ActivePlayersCount
    {
        get
        {
            int count = 0;
            foreach (var player in Players.Cast<PokerBettingPlayer>())
                if (player.IsActive) count++;
            return count;
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
        CommunityCards = new CommunityCards(this);

        // create bet component
        _betComponent = new BetComponent(this);
        game.Components.Add(_betComponent);
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
                HandleCardDealingState(3);
                break;

            case GameState.FlopBet:
                HandleBettingState();
                break;

            case GameState.Turn:
                HandleCardDealingState(1);
                break;

            case GameState.TurnBet:
                HandleBettingState();
                break;

            case GameState.River:
                HandleCardDealingState(1);
                break;

            case GameState.RiverBet:
                HandleBettingState();
                break;

            case GameState.Showdown:
                HandleShowdown();
                break;
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

            _betComponent.HandleBetting(CurrentPlayer);
        }
    }

    void HandleCardDealingState(int cardCount)
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
                State++;
            }
        }
    }

    void HandleBettingState()
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

            _betComponent.HandleBetting(CurrentPlayer);
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
            else if (_screenManager.ActiveScreen is ThemeScreen)
                _screenManager.ShowScreen<StartScreen>();
            else if (_screenManager.ActiveScreen is TestScreen)
                StopPlaying();
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

    /// <summary>
    /// Changes the current player to the next one to take turn.
    /// </summary>
    public void ChangeCurrentPlayer()
    {
        CurrentPlayer = GetNextPlayer(CurrentPlayer);
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

    /// <summary>
    /// Resets everything to a default state.
    /// </summary>
    public void Reset()
    {
        // reset players
        foreach (var player in Players)
            ((PokerBettingPlayer)player).Reset();

        CommunityCards.Reset();
        _dealer.Reset();
        _betComponent.Reset();
        _animatedCardPile.Reset();
        CurrentPlayer = null;
        _allCardsAreFaceUp = false;
    }

    /// <summary>
    /// This is the action performed when the player clicks the start button.
    /// </summary>
    /// <remarks>It assumes the game is in the default state.
    /// There are only two screen that make the components of this game change their state:
    /// <br></br><br></br><b>gameplay screen</b> and the <b>test screen</b>.<br></br><br></br>
    /// Both need to make sure to call the game manager's Reset() when they exit to the start screen.</remarks>
    public override void StartPlaying()
    {
        foreach (PokerBettingPlayer player in Players.Cast<PokerBettingPlayer>())
            player.StartPlaying();

        _dealer.StartPlaying();
        _betComponent.StartPlaying();
        _animatedCardPile.StartPlaying();
        State = GameState.Shuffling;
    }

    /// <summary>
    /// Called when pressed escape or clicked on exit button in the <see cref="PauseScreen"/>.
    /// </summary>
    public void StopPlaying()
    {
        _screenManager.ShowScreen<StartScreen>();
        Reset();
    }

    /// <summary>
    /// Called before starting a new poker game. Players carry their balances on.
    /// </summary>
    void StartNewGame()
    {
        // prepare players
        foreach (var player in Players)
            ((PokerBettingPlayer)player).StartNewGame();

        // return community cards
        CommunityCards.ReturnCardsToDealer();

        CurrentPlayer = null;
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
            TraditionalCard card = _dealer.DealCardToHand(CommunityCards.Hand);
            CommunityCards.AddDealAnimation(card, true, startTime);
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

    /// <summary>
    /// Determines whether the current state allows placing bets.<br></br>
    /// Preflop is not counted here as this state should be handled by the StartPlaying()
    /// method of all the components.
    /// </summary>
    /// <param name="state"><see cref="GameState"/> to check.</param>
    static bool IsBettingRound(GameState state)
    {
        return state switch
        {
            GameState.FlopBet or GameState.TurnBet or GameState.RiverBet => true,
            _ => false
        };
    }

    void OnThemeChanged(string theme)
    {
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme));
    }

    void OnStateChanged(GameState prevGameState, GameState newGameState)
    {
        // betting rounds not including preflop
        if (IsBettingRound(newGameState))
        {
            _betComponent.StartNewBettingRound();

            for (int i = 0; i < PlayerCount; i++)
            {
                var player = this[i];
                player.StartNewBettingRound();

                // set the current player as the one with the small blind chip
                // if they have folded or are all in, the bet component will move on to the next one itself
                if (player.BlindChip is SmallBlindChip)
                    CurrentPlayer = player;
            }
        }

        GameStateChanged?.Invoke(this, new GameStateEventArgs(prevGameState, newGameState));
    }

    void AllFoldedButOne_OnRuleMatch(object o, GameEndEventArgs e)
    {
        _betComponent.TransferPotToWinner(e.Winner);
    }

    void OnCurrentPlayerChanged(PokerBettingPlayer prevPlayer, PokerBettingPlayer newPlayer)
    {
        _ignorePlayerAnimations = false;

        var args = new PlayerChangedEventArgs(prevPlayer, newPlayer);
        PlayerChanged?.Invoke(this, args);
    }
}