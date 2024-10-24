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
using Poker.Gameplay.Evaluation;

namespace Poker.Gameplay;

class GameManager : CardGame, IGlobalManager
{
    public event EventHandler<GameStateEventArgs> StateChanged;
    public event EventHandler<PlayerChangedEventArgs> PlayerChanged;

    readonly AnimatedCardPile _animatedCardPile;
    readonly BetComponent _betComponent;
    readonly ScreenManager _screenManager;
    
    /// <summary>
    /// Used to make sure that the label animations are ignored during betting rounds.
    /// </summary>
    bool _ignorePlayerAnimations;

    // used for pausing and resuming gameplay components
    readonly List<DrawableGameComponent> _pauseEnabledComponents = new();
    readonly List<DrawableGameComponent> _pauseVisibleComponents = new();

    public Dealer Dealer { get; }

    /// <summary>
    /// Represents the 5 community cards placed on the table.
    /// </summary>
    public CommunityCards CommunityCards { get; }

    /// <summary>
    /// Stores the winner for the purpose of distributing chips.
    /// </summary>
    public PokerBettingPlayer Winner { get; private set; }

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
    public int ActivePlayerCount
    {
        get
        {
            int count = 0;
            foreach (var player in Players.Cast<PokerBettingPlayer>())
                if (player.CanParticipateInBettingRound) 
                    count++;
            return count;
        }
    }

    /// <summary>
    /// Number of players waiting to take their turn.
    /// </summary>
    public int WaitingPlayerCount => CountPlayers(p => p.IsWaiting);

    /// <summary>
    /// Number of players that are bankrupt.
    /// </summary>
    public int BankruptPlayerCount => CountPlayers(p => p.IsBankrupt);

    /// <summary>
    /// Number of players that are bankrupt or folded.
    /// </summary>
    public int OutPlayerCount => CountPlayers(p => p.IsOut);

    /// <summary>
    /// Number of players in the game.
    /// </summary>
    public int PlayerCount => Players.Count;

    public GameManager(Game game, ScreenManager screenManager) 
        : base(1, 0, CardSuits.AllSuits, CardValues.NonJokers, Fonts.Moire.Regular,
        Constants.MinPlayers, Constants.MaxPlayers, new PokerTable(game), Strings.Red, game)
    {
        _screenManager = screenManager;

        // create animated card pile
        _animatedCardPile = new AnimatedCardPile(this);
        game.Components.Add(_animatedCardPile);

        // create dealer hand
        Dealer = new(CardDeck);

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

    /// <summary>
    /// Indexer that returns <see cref="PokerBettingPlayer"/>.
    /// </summary>
    /// <returns><see cref="PokerBettingPlayer"/> with the given index.</returns>
    public PokerBettingPlayer this[int index] =>
        Players[index] as PokerBettingPlayer;

    public Dealer GetPokerDealer() => Dealer;

    public override Player GetCurrentPlayer() => CurrentPlayer;

    int CountPlayers(Predicate<PokerBettingPlayer> predicate)
    {
        int count = 0;
        foreach (var player in Players.Cast<PokerBettingPlayer>())
        {
            if (predicate(player))
                count++;
        }
        return count;
    }

    /// <summary>
    /// Finds the player who meets given criteria in the predicate.
    /// </summary>
    /// <param name="predicate">Predicate to find the player by.</param>
    /// <returns>Player if found or null.</returns>
    PokerBettingPlayer FindPlayer(Predicate<PokerBettingPlayer> predicate)
    {
        foreach (var player in Players.Cast<PokerBettingPlayer>())
        {
            if (predicate(player))
                return player;
        }
        return null;
    }

    public void Update()
    {
        HandleKeyboardEscapeButton();

        if (_screenManager.ActiveScreen is GameplayScreen)
            HandleGameplay();
    }

    void HandleGameplay()
    {
        switch (State)
        {
            case GameState.Shuffling:
                Dealer.Shuffle();
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

            case GameState.Evaluation:
                HandleEvaluation(); 
                break;

            case GameState.RoundEnd:
                HandleRoundEnd();
                break;

            case GameState.GameOver:
                HandleGameOver();
                break;
        }
    }

    void HandlePreflop()
    {
        // wait for the player chips animations to finish
        if (!CheckForRunningAnimations<AnimatedChipComponent>() || _ignorePlayerAnimations)
        {
            if (!_ignorePlayerAnimations)
            {
                // establish who the current player is if not done already
                if (CurrentPlayer is null)
                {
                    var bigBlindPlayer = FindPlayer(p => p.BlindChip is BigBlindChip) ??
                        throw new Exception("Big blind player cannot be found.");
                    CurrentPlayer = GetNextPlayer(bigBlindPlayer);

                    //for (int i = 0; i < PlayerCount; i++)
                    //{
                    //    if (this[i].BlindChip is BigBlindChip)
                    //        CurrentPlayer = GetNextPlayer(this[i]);
                    //}
                }

                // check if everyone folded 
                else if (OutPlayerCount == PlayerCount - 1)
                {
                    var player = FindPlayer(p => !p.IsOut) ??
                        throw new Exception("Cannot find any players who are not out.");

                    Winner = player;
                    Winner.SetWinnerState();
                    State = GameState.RoundEnd;
                    return;
                }

                _ignorePlayerAnimations = true;
            }

            _betComponent.HandleBetting(CurrentPlayer);
        }
    }

    /// <summary>
    /// Handles flop, turn and river stages.
    /// </summary>
    /// <param name="cardCount">Number of cards to deal (Flop 3, Turn 1, River 1).</param>
    void HandleCardDealingState(int cardCount)
    {
        if (!CheckForRunningAnimations<AnimatedGameComponent>())
        {
            // show card pile
            if (_animatedCardPile.Position != Constants.CardPileVisiblePosition &&
                !_animatedCardPile.IsAnimating)
                _animatedCardPile.SlideDown();

            // wait for the card pile animation to finish
            if (!CheckForRunningAnimations<AnimatedCardPile>())
            {
                DealCommunityCards(cardCount);

                // check if there are at least two players able to take action
                if (ActivePlayerCount > 1)
                    State++;

                // looks like everyone (or everyone but one) is all in
                else
                    State += 2;
            }
        }
    }

    /// <summary>
    /// Handles three stages of betting: post flop, post turn and post river.
    /// </summary>
    void HandleBettingState()
    {
        // wait for the community cards animations to finish
        if (!CheckForRunningAnimations<AnimatedGameComponent>() || _ignorePlayerAnimations)
        {
            if (!_ignorePlayerAnimations)
            {
                // hide card pile
                if (_animatedCardPile.Position != Constants.CardPileHiddenPosition &&
                    !_animatedCardPile.IsAnimating)
                    _animatedCardPile.SlideUp();

                // check if everyone folded 
                if (OutPlayerCount == PlayerCount - 1)
                {
                    var player = FindPlayer(p => !p.IsOut) ?? 
                        throw new Exception("Cannot find any players who are not out.");

                    Winner = player;
                    Winner.SetWinnerState();
                    State = GameState.RoundEnd;
                    return;
                }

                _ignorePlayerAnimations = true;
            }

            _betComponent.HandleBetting(CurrentPlayer);
        }
    }

    /// <summary>
    /// Card reveal after the last betting round.
    /// </summary>
    void HandleShowdown()
    {
        if (!CheckForRunningAnimations<AnimatedGameComponent>())
        {
            var startTime = DateTime.Now;
            foreach (var player in Players.Cast<PokerBettingPlayer>())
            {
                if (!player.IsOut)
                {
                    // flip the cards if they are face down
                    if (player.AnimatedHand.IsFaceDown)
                    {
                        player.AnimatedPlayerHand.Flip(startTime);
                        startTime += Constants.CardFlipAnimationDuration; // * player.Hand.Count;
                    }

                    // reset state and hide the label
                    player.ResetState();
                }
            }

            // change state
            State++;
        }
    }

    /// <summary>
    /// Poker hand evaluation and finding the winner stage.
    /// </summary>
    void HandleEvaluation()
    {
        if (!CheckForRunningAnimations<AnimatedGameComponent>())
        {
            // evaluate all players
            foreach (var player in Players.Cast<PokerBettingPlayer>())
            {
                if (player.IsOut)
                    continue;

                var handType = Evaluator.CheckHand(player, CommunityCards,
                    out TraditionalCard[] cards);
                player.Result = new Result(handType, cards, player);
            }

            // evaluate results and find the winner (winners)
            List<PokerBettingPlayer> winners = new();
            foreach (var player in Players.Cast<PokerBettingPlayer>())
            {
                if (player.IsOut)
                    continue;

                if (Winner is null)
                {
                    Winner = player;
                }
                else if (Winner.Result < player.Result)
                {
                    Winner = player;
                    winners.Clear();
                }
                else if (Winner.Result == player.Result)
                {
                    winners.Add(player);
                }
            }

            if (Winner is null)
                throw new Exception("There has to be at least one winner.");

            // raise the winning cards
            DateTime startTime = DateTime.Now;

            foreach (var card in Winner.Result.Cards)
            {
                if (CommunityCards.HasCard(card))
                {
                    CommunityCards.RaiseCard(card, startTime);
                    startTime += TimeSpan.FromMilliseconds(200);
                }
                else if (Winner.HasCard(card))
                {
                    Winner.RaiseCard(card, startTime);
                    startTime += TimeSpan.FromMilliseconds(200);
                }
            }

            // change player label
            Winner.SetWinnerState();

            // change state
            State++;
        }
    }

    /// <summary>
    /// Chips distribution between winning players.
    /// </summary>
    void HandleRoundEnd()
    {
        if (!CheckForRunningAnimations<AnimatedGameComponent>())
        {
            foreach (var component in Game.Components)
            {
                if (component is Label label && 
                    label.Player == Winner)
                {
                    var test = label;
                    var count = label.IsAnimating;
                }
            }

            _betComponent.HandleWinning(Winner);
            State++;
        }
    }

    void HandleGameOver()
    {
        if (!CheckForRunningAnimations<AnimatedChipComponent>())
        {
            foreach (var player in Players.Cast<PokerBettingPlayer>())
            {
                if (!player.IsOut)
                    player.CheckBankrupt();
            }
            State++;
        }
    }

    /// <summary>
    /// Since the game uses mostly mouse for the player input, keyboard is checked just for the escape button. 
    /// </summary>
    void HandleKeyboardEscapeButton()
    {
        if (InputManager.IsNewKeyPress(Keys.Escape))
        {
            if (_screenManager.ActiveScreen is GameplayScreen)
            {
                if (State == GameState.Waiting)
                    StopPlaying();
                else
                    PauseGame();
            }
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

    /// <summary>
    /// Changes the current player to the next one to take turn.
    /// </summary>
    public void ChangeCurrentPlayer()
    {
        if (CurrentPlayer is HumanPlayer humanPlayer)
            humanPlayer.StartedTurn = false;
        CurrentPlayer = GetNextPlayer(CurrentPlayer);
        _ignorePlayerAnimations = false;
        
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
        Dealer.Reset();
        _betComponent.Reset();
        _animatedCardPile.Reset();
        CurrentPlayer = null;
        Winner = null;
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

        Dealer.StartPlaying();
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

        CommunityCards.ReturnCardsToDealer();
        _betComponent.StartNewGame();
        _animatedCardPile.StartNewGame();

        CurrentPlayer = null;
        Winner = null;
        State = GameState.Shuffling;
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
                if (player.IsBankrupt) continue;

                TraditionalCard card = Dealer.DealCardToHand(player.Hand);
                bool flip = false;

                if (player is HumanPlayer)
                {
                    flip = true;
                    player.AnimatedHand.IsFaceDown = false;
                }

                // add deal animation
                player.AddDealAnimation(card, flip, startTime);

                // advance start time
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
            TraditionalCard card = Dealer.DealCardToHand(CommunityCards.Hand);
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
    public bool CheckForRunningAnimations<T>() where T : AnimatedGameComponent
    {
        for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)
            if (Game.Components[componentIndex] is T animComponent && animComponent.IsAnimating)
                return true;
        return false;
    }

    /// <summary>
    /// Changes card theme for the game.
    /// </summary>
    /// <param name="theme">New card theme.</param>
    public void SetTheme(string theme)
    {
        if (theme != Strings.Red && theme != Strings.Blue) return;
        Theme = theme;
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
        int nameCount = Strings.Names.Length / 2;
        do
        {
            // get random index taking into consideration the appropriate half of collection
            int offset = gender == Gender.Male ? 0 : nameCount;
            int index = Game.Services.GetService<Random>().Next(0, nameCount);

            // retrieve the name and cap the index (just in case)
            name = Strings.Names[Math.Min(index + offset, Strings.Names.Length - 1)];

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

    public void AssignEventHandlers()
    {
        var gameplayScreen = Game.Components.Find<GameplayScreen>();
        gameplayScreen.RestartButton.Click += GameplayScreen_OnRestartClick;
        gameplayScreen.DealButton.Click += GameplayScreen_OnDealClick;
        gameplayScreen.ExitButton.Click += GameplayScreen_OnExitClick;
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

    void OnStateChanged(GameState prevGameState, GameState newGameState)
    {
        // betting rounds not including preflop
        if (IsBettingRound(newGameState))
        {
            _betComponent.StartNewBettingRound();

            foreach (var player in Players.Cast<PokerBettingPlayer>())
            {
                player.StartNewBettingRound();

                // set the current player as the one with the small blind chip
                // if they have folded or are all in, the bet component will move on to the next one itself
                if (player.BlindChip is SmallBlindChip)
                    CurrentPlayer = player;
            }
        }

        StateChanged?.Invoke(this, new GameStateEventArgs(prevGameState, newGameState));
    }

    void OnCurrentPlayerChanged(PokerBettingPlayer prevPlayer, PokerBettingPlayer newPlayer)
    {
        _ignorePlayerAnimations = false;

        var args = new PlayerChangedEventArgs(prevPlayer, newPlayer);
        PlayerChanged?.Invoke(this, args);
    }

    void GameplayScreen_OnRestartClick(object o, EventArgs e)
    {
        Reset();
        StartPlaying();
    }

    void GameplayScreen_OnExitClick(object o, EventArgs e) =>
        StopPlaying();

    void GameplayScreen_OnDealClick(object o, EventArgs e) =>
        StartNewGame();

    //void ThemeScreen_OnRedButtonClick(object o, EventArgs e) =>
    //    SetTheme(Strings.Red);

    //void ThemeScreen_OnBlueButtonClick(object o, EventArgs e) =>
    //    SetTheme(Strings.Blue);
}