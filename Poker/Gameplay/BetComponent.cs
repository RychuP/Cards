using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Poker.Gameplay.Chips;
using Poker.Gameplay.Players;
using Poker.UI;
using Poker.UI.Screens;
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
    /// Last player to call or check.
    /// </summary>
    PokerBettingPlayer _savedPlayer;

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

    public BetComponent(GameManager gm) : base(gm.Game)
    {
        _gameManager = gm;
        Enabled = false;
        Visible = false;

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

    public override void Update(GameTime gameTime)
    {
        foreach (var chip in CommunityChips)
            chip.Update();

        base.Update(gameTime);
    }

    public override void Initialize()
    {
        base.Initialize();
        var gameplayScreen = Game.Components.Find<GameplayScreen>();
        gameplayScreen.CallButton.Click += CallButton_OnClick;
        gameplayScreen.AllInButton.Click += AllInButton_OnClick;
        gameplayScreen.ClearButton.Click += ClearButton_OnClick;
        gameplayScreen.RaiseButton.Click += RaiseButton_OnClick;
        gameplayScreen.CheckButton.Click += CheckButton_OnClick;
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

    public void StartNewRound()
    {

    }

    /// <summary>
    /// Puts the component in the default state.
    /// </summary>
    public void Reset()
    {
        Enabled = true;
        Visible = true;
        _savedPlayer = null;
        Stage = BetStage.Raise;

        var rand = Game.Services.GetService<Random>();

        // allocate a random player for the small blind chip
        int index = rand.Next(0, Constants.MaxPlayers);
        _smallBlindPlayer = _gameManager[index];

        // remove blind chips from players
        for (int i = 0; i < _gameManager.PlayerCount; i++)
            _gameManager[i].BlindChip = null;

        // put the community chips in the hidden position
        foreach (var chip in CommunityChips)
            chip.Position = Chip.HiddenPosition;

        // put the blind chips in the hidden position
        foreach (var chip in BlindChips)
            chip.Position = Chip.HiddenPosition;

        // mark human player's turn as not started
        if (_gameManager[0] is HumanPlayer humanPlayer)
            humanPlayer.StartedTurn = false;
    }
    
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
        // issue small blind chip
        _smallBlindPlayer.BlindChip = SmallBlindChip;

        // issue big blind chip
        var bigBlindPlayer = _gameManager.GetNextPlayer(_smallBlindPlayer);
        bigBlindPlayer.BlindChip = BigBlindChip;

        // save the last raise player
        _savedPlayer = bigBlindPlayer;
    }

    /// <summary>
    /// Takes care of all the betting logic.
    /// </summary>
    /// <param name="currentPlayer"><see cref="PokerBettingPlayer"/> currently taking their turn.</param>
    public void HandleBetting(PokerBettingPlayer currentPlayer)
    {
        // everybody called or checked -> finish the round
        if (currentPlayer == _savedPlayer)
        {
            if (Stage == BetStage.Raise)
            {
                // check if all players called the big blind
                if (_gameManager.State == GameState.Preflop
                    && currentPlayer.BetAmount == BigBlind && currentPlayer.BlindChip is BigBlindChip)
                {
                    // if so, give the big blind player a chance to raise (live blind rule)
                    _savedPlayer = _gameManager.GetNextPlayer(currentPlayer);
                    return;
                }
            }

            FinishBettingRound();
        }
        else 
        {
            // skip all-in players
            if (currentPlayer.State == PlayerState.AllIn)
            {
                _gameManager.ChangeCurrentPlayer();
            }
            // get ai players to take turn
            else if (currentPlayer is AIPlayer aiPlayer)
            {
                if (Stage == BetStage.Check)
                {
                    aiPlayer.TakeTurn();
                    
                    if (aiPlayer.State == PlayerState.Raised)
                    {
                        Stage = BetStage.Raise;
                        _savedPlayer = aiPlayer;
                    }
                    else if (aiPlayer.State == PlayerState.Checked)
                    {
                        _savedPlayer ??= aiPlayer;
                    }
                    else
                        throw new Exception("AI Player did not take appropriate action.");
                }
                else
                {
                    if (_savedPlayer is null)
                        throw new Exception("No saved player in the Raise stage.");

                    aiPlayer.TakeTurn(_savedPlayer.BetAmount);
                    if (aiPlayer.State == PlayerState.Raised)
                        _savedPlayer = aiPlayer;
                }
                _gameManager.ChangeCurrentPlayer();
            }
            // handle human player turn
            else if (currentPlayer is HumanPlayer humanPlayer)
            {
                // mark the player's turn as started
                if (!humanPlayer.StartedTurn)
                    humanPlayer.StartedTurn = true;

                // handle community chip clicks
                foreach (var chip in CommunityChips)
                {
                    if (chip.IsClick)
                    {
                        int value = chip.Value;
                        int diff = humanPlayer.Balance - value - humanPlayer.BetAmount;
                        if (diff >= 0)
                            humanPlayer.BetAmount += value;
                    }
                }

                // turn on buttons
                var gameplayScreen = Game.Components.Find<GameplayScreen>();
                int currentBet = _savedPlayer is not null ?
                    _savedPlayer.BetAmount : humanPlayer.PrevBetAmount;
                gameplayScreen.ShowButtons(currentBet, humanPlayer.BetAmount,
                    humanPlayer.PrevBetAmount, humanPlayer.Balance, Stage == BetStage.Check);
            }
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

    void FinishBettingRound()
    {
        var humanPlayer = _gameManager[0] as HumanPlayer;
        humanPlayer.StartedTurn = false;

        Stage = BetStage.Check;
        _savedPlayer = null;

        _gameManager.MoveToNextStage();
    }

    void FinishPlayerTurn(HumanPlayer humanPlayer)
    {
        humanPlayer.StartedTurn = false;

        // move to the next player
        _gameManager.ChangeCurrentPlayer();

        // hide buttons
        var gameplayScreen = Game.Components.Find<GameplayScreen>();
        gameplayScreen?.HideButtons();
    }

    void CallButton_OnClick(object o, EventArgs e)
    {
        var player = _gameManager.CurrentPlayer;
        if (player is not HumanPlayer humanPlayer)
            throw new Exception(Constants.ButtonClickExceptionText);

        // match the current bet amount and finish turn
        humanPlayer.BetAmount = _savedPlayer.BetAmount;
        humanPlayer.State = PlayerState.Called;
        FinishPlayerTurn(humanPlayer);
    }

    void AllInButton_OnClick(object o, EventArgs e)
    {
        var player = _gameManager.CurrentPlayer;
        if (player is not HumanPlayer humanPlayer)
            throw new Exception(Constants.ButtonClickExceptionText);

        humanPlayer.BetAmount = humanPlayer.Balance;
        humanPlayer.State = PlayerState.AllIn;
        Stage = BetStage.Raise;

        // check if this is the highest bet
        if (_savedPlayer.BetAmount < humanPlayer.BetAmount)
            _savedPlayer = humanPlayer;

        FinishPlayerTurn(humanPlayer);
    }

    void ClearButton_OnClick(object o, EventArgs e)
    {
        var player = _gameManager.CurrentPlayer;
        if (player is not HumanPlayer humanPlayer)
            throw new Exception(Constants.ButtonClickExceptionText);

        humanPlayer.BetAmount = humanPlayer.PrevBetAmount;
    }

    void RaiseButton_OnClick(object o, EventArgs e)
    {
        var player = _gameManager.CurrentPlayer;
        if (player is not HumanPlayer humanPlayer)
            throw new Exception(Constants.ButtonClickExceptionText);

        _savedPlayer = humanPlayer;
        Stage = BetStage.Raise;
        humanPlayer.State = PlayerState.Raised;
        FinishPlayerTurn(humanPlayer);
    }

    void CheckButton_OnClick(object o, EventArgs e)
    {
        var player = _gameManager.CurrentPlayer;
        if (player is not HumanPlayer humanPlayer)
            throw new Exception(Constants.ButtonClickExceptionText);

        if (_savedPlayer is null)
            _savedPlayer = humanPlayer;
        FinishPlayerTurn(humanPlayer);
    }
}