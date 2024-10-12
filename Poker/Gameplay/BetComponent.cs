using Framework.Engine;
using Microsoft.Xna.Framework.Graphics;
using Poker.Gameplay.Chips;
using Poker.Gameplay.Players;
using System;
using System.Collections.Generic;

namespace Poker.Gameplay;

// Notes:
// There is no dealer chip in this implementation.
// Each betting round starts with whoever holds the small blind chip and continues clockwise.

/// <summary>
/// Class responsible for handling betting rounds.
/// </summary>
class BetComponent : DrawableGameComponent
{
    public static readonly int SmallBlind = 10;
    public static readonly int BigBlind = 25;

    readonly GameManager _gameManager;

    /// <summary>
    /// Player who receives the small blind chip this round.
    /// </summary>
    PokerBettingPlayer _smallBlindPlayer;

    /// <summary>
    /// Current stage of the betting round.
    /// </summary>
    BetStage Stage;

    /// <summary>
    /// Community chips that are placed below community cards.
    /// </summary>
    readonly List<CommunityChip> CommunityChips = new(ValueChip.Count);

    /// <summary>
    /// Blind chips that are distrubued among the players.
    /// </summary>
    readonly List<BlindChip> BlindChips = new(BlindChip.Count);

    SmallBlindChip SmallBlindChip => 
        BlindChips.Find(c => c is SmallBlindChip) as SmallBlindChip;

    BigBlindChip BigBlindChip =>
        BlindChips.Find(c => c is BigBlindChip) as BigBlindChip;

    // backing field
    int _currentBet;
    int CurrentBet
    {
        get => _currentBet;
        set
        {
            if (_currentBet == value)
            {
                if (_currentBet > 0 && !LiveBlindActive)
                {
                    throw new ArgumentException("The same amount as already saved have been passed." +
                        "This should not happen unless live blind rule is active.");
                }
                return;
            }

            int prevValue = _currentBet;
            _currentBet = value;
            OnCurrentBetChanged(prevValue, value);
        }
    }

    public BetComponent(GameManager gm) : base(gm.Game)
    {
        _gameManager = gm;
        Hide();

        // create community chips
        for (int i = 0; i < ValueChip.Count; i++)
        {
            var chip = new CommunityChip(Game, ValueChip.Values[i]);
            CommunityChips.Add(chip);
        }

        // create blind chips
        BlindChips.Add(new SmallBlindChip(Game));
        BlindChips.Add(new BigBlindChip(Game));
    }

    public override void Initialize()
    {
        for (int i = 0; i < _gameManager.PlayerCount; i++)
            _gameManager[i].StateChanged += Player_OnStateChanged;

        base.Initialize();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var chip in CommunityChips)
            chip.Update();

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        // draw balances and bet amounts
        for (int i = 0; i < Constants.MaxPlayers; i++)
        {
            var player = _gameManager[i];
            string balanceText = $"${player.Balance}";
            var balancePos = player.GetCenteredTextPosition(balanceText, 1);
            sb.DrawString(_gameManager.Font, balanceText, balancePos, Color.CornflowerBlue);

            string betText = $"${player.BetAmount}";
            var betPos = player.GetCenteredTextPosition(betText, 2);
            sb.DrawString(_gameManager.Font, betText, betPos, Color.OrangeRed);
        }

        // draw outlines for community chips in hover mode
        foreach (var chip in CommunityChips)
        {
            if (chip.IsHover)
                sb.Draw(Art.ChipOutline, chip.OutlineDestination, Color.White);
        }
        
        sb.End();
    }

    public void Show()
    {
        Enabled = true;
        Visible = true;
    }

    public void Hide()
    {
        Enabled = false;
        Visible = false;
    }

    /// <summary>
    /// Puts everything in the default state.
    /// </summary>
    public void Reset()
    {
        Hide();
        CurrentBet = 0;

        // the preflop has to start with the raise stage (small and big blind)
        // checks are only available during the second betting round
        Stage = BetStage.Raise;

        // put the community chips in the hidden position
        foreach (var chip in CommunityChips)
            chip.Position = Chip.HiddenPosition;

        // put the blind chips in the hidden position
        foreach (var chip in BlindChips)
            chip.Position = Chip.HiddenPosition;
    }

    /// <summary>
    /// Called when the game moves from start screen to gameplay screen.<br></br>
    /// Sets everything up for the preflop stage.
    /// </summary>
    public void StartPlaying()
    {
        Show();

        // allocate a random player for the small blind chip
        var rand = Game.Services.GetService<Random>();
        int index = rand.Next(0, Constants.MaxPlayers);

        // DEBUGGING -> CHANGE
        //_smallBlindPlayer = _gameManager[index];
        _smallBlindPlayer = _gameManager[3];  

        // bring the community chips to the table
        ShowCommunityChips();
    }

    /// <summary>
    /// Called at the start of secord, third and forth rounds of betting.
    /// </summary>
    public void StartNewBettingRound()
    {
        Stage = BetStage.Check;
    }

    /// <summary>
    /// Called before the start of a new poker game. Players carry their balances on.
    /// </summary>
    public void StartNewGame()
    {
        CurrentBet = 0;

        // find the next player to receive the small blind chip
        var player = _smallBlindPlayer;
        for (int i = 0; i < _gameManager.PlayerCount; i++)
        {
            player = _gameManager.GetNextPlayer(player);
            if (player.State != PlayerState.Bankrupt)
                _smallBlindPlayer = player;
        }
    }

    /// <summary>
    /// A rule that allows the big blind player to make a raise when all players called the big blind.
    /// </summary>
    public bool LiveBlindActive =>
        _gameManager.State == GameState.Preflop &&
        _gameManager.CurrentPlayer.BlindChip is BigBlindChip &&
        _gameManager.CurrentPlayer.BetAmount == BigBlind &&
        _gameManager.CurrentPlayer.State == PlayerState.Raised &&       
        CurrentBet == BigBlind;

    /// <summary>
    /// Puts community chips and blind chips on the table.
    /// </summary>
    public void ShowCommunityChips()
    {
        foreach (var chip in CommunityChips)
            chip.Position = chip.GetTablePosition();

        foreach (var chip in BlindChips)
            chip.Position = chip.GetTablePosition();
    }

    public void IssueBlindChips()
    {
        AllocateBlindChip(SmallBlindChip, _smallBlindPlayer);

        // find the player to issue the big blind chip for
        var player = _smallBlindPlayer;
        PokerBettingPlayer bigBlindPlayer = null;
        for (int i = 0; i < _gameManager.PlayerCount; i++)
        {
            player = _gameManager.GetNextPlayer(player);
            if (player.State != PlayerState.Bankrupt)
            {
                bigBlindPlayer = player;
                break;
            }
        }

        if (bigBlindPlayer == null)
            throw new Exception("Variable is still null. This shouldn't happen.");
        if (bigBlindPlayer == _smallBlindPlayer)
            throw new Exception("No appropriate player found for the big blind chip.");
        else
            AllocateBlindChip(BigBlindChip, bigBlindPlayer);
    }

    void AllocateBlindChip(BlindChip blindChip, PokerBettingPlayer player)
    {
        player.BlindChip = blindChip;
    }

    /// <summary>
    /// Takes care of all the betting logic.
    /// </summary>
    /// <param name="currentPlayer"><see cref="PokerBettingPlayer"/> currently taking their turn.</param>
    public void HandleBetting(PokerBettingPlayer currentPlayer)
    {
        int waitingPlayerCount = 0;
        for (int i = 0; i < _gameManager.PlayerCount; ++i)
        {
            var player = _gameManager[i];
            if (player.State == PlayerState.Waiting)
                waitingPlayerCount++;
        }

        if (waitingPlayerCount == 0)
        {
            // check if the live blind rule is active
            if (LiveBlindActive)
            {
                // give the big blind player a chance to raise
                currentPlayer.ResetState();
                return;
            }

            // no player can take any further action in this betting round ->
            // move to the next stage
            _gameManager.State++;
        }

        // players get the chance to check, raise or fold
        else
        {
            MakePlayerTakeTurn(currentPlayer);
        }
    }

    /// <summary>
    /// Makes the player to take their turn.
    /// </summary>
    /// <param name="player">Player to take turn.</param>
    void MakePlayerTakeTurn(PokerBettingPlayer player)
    {
        if (CurrentBet == 0)
            throw new Exception("Betting stage should not have a current bet equal to zero.");
        else if (player is not HumanPlayer && CurrentBet < player.BetAmount)
            throw new Exception("Curent bet should not be lower than " +
                "this player's bet amount.");

        bool checkPossible = Stage == BetStage.Check;
        if (player is AIPlayer aiplayer)
        {
            aiplayer.TakeTurn(CurrentBet, _gameManager.CommunityCards.Hand,
                checkPossible);
        }
        else if (player is HumanPlayer humanPlayer)
        {
            // player has not finished their turn
            if (humanPlayer.State == PlayerState.Waiting)
            {
                humanPlayer.TakeTurn(CurrentBet, _gameManager.CommunityCards.Hand,
                    checkPossible);

                // handle community chip clicks
                foreach (var chip in CommunityChips)
                {
                    if (chip.IsClick)
                    {
                        bool playerHasSufficientBalance =
                            humanPlayer.Balance - chip.Value - humanPlayer.BetAmount >= 0;
                        if (playerHasSufficientBalance)
                            humanPlayer.BetAmount += chip.Value;
                    }
                }
            }

        }
    }

    /// <summary>
    /// Determines whether to count the allin as a raise or a call.
    /// </summary>
    /// <param name="allInBetAmount"></param>
    void HandleAllIn(int allInBetAmount)
    {
        // allin raised the current bet
        if (allInBetAmount > CurrentBet)
        {
            CurrentBet = allInBetAmount;
        }
        else
        {
            // TODO:
            // handle the side pots here
        }
    }

    public void TransferPotToWinner(PokerBettingPlayer winner)
    {
        for (int i = 0; i < _gameManager.PlayerCount; i++)
        {
            var player = _gameManager[i];
            if (player != winner && player.State != PlayerState.Bankrupt)
            {
                winner.Balance += player.BetAmount;
                player.BetAmount = 0;
            }
            winner.BetAmount = 0;
        }
    }

    void OnCurrentBetChanged(int prevValue, int newValue)
    {
        if (newValue != 0)
        {
            Stage = BetStage.Raise;

            // set active players to Waiting state to give them a chance to respond to the raise
            for (int i = 0; i < _gameManager.PlayerCount; ++i)
            {
                var player = _gameManager[i];
                if (player != _gameManager.CurrentPlayer && player.IsActive)
                    player.ResetState();
            }
        }
    }

    void Player_OnStateChanged(object o, PlayerStateChangedEventArgs e)
    {
        if (o is not PokerBettingPlayer player)
            throw new Exception("Event object should be a poker betting player.");

        switch (e.NewState)
        {
            case PlayerState.Checked:
                if (Stage != BetStage.Check)
                    throw new Exception("Player should not be able to check at this betting stage.");
                break;

            case PlayerState.Called:
                if (Stage != BetStage.Raise)
                    throw new Exception("Player should not be able to call at this betting stage.");
                break;

            case PlayerState.Folded:
                
                break;

            case PlayerState.Raised:
                CurrentBet = player.BetAmount;
                break;

            case PlayerState.AllIn:
                HandleAllIn(player.BetAmount);
                break;

            // waiting and bankrupt
            default:
                return;
        }

        if (_gameManager.CurrentPlayer == player)
            _gameManager.ChangeCurrentPlayer();
    }
}