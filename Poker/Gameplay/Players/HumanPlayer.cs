using Framework.Engine;
using Poker.UI.Screens;
using System;

namespace Poker.Gameplay.Players;

class HumanPlayer : PokerBettingPlayer
{
    readonly GameplayScreen _gameplayScreen;

    /// <summary>
    /// Bet amount that the player started the turn with.
    /// </summary>
    int _initialBetAmount;

    /// <summary>
    /// Saved current bet amount passed by the the bet component in TakeTurn method.
    /// </summary>
    int _tableBetAmount;

    // backing field
    bool _startedTurn;
    /// <summary>
    /// Turn marker that shows whether the human player is currently taking their turn.
    /// </summary>
    public bool StartedTurn
    {
        get => _startedTurn;
        set
        {
            if (_startedTurn == value) return;
            bool prevValue = _startedTurn;
            _startedTurn = value;
            OnStartedTurnChanged(prevValue, value);
        }
    }

    public HumanPlayer(Gender gender, GameManager gm) : base("You", gender, 0, gm)
    {
        _gameplayScreen = Game.Components.Find<GameplayScreen>();
    }

    public void AssignClickHandlers()
    {
        _gameplayScreen.CallButton.Click += CallButton_OnClick;
        _gameplayScreen.AllInButton.Click += AllInButton_OnClick;
        _gameplayScreen.ClearButton.Click += ClearButton_OnClick;
        _gameplayScreen.RaiseButton.Click += RaiseButton_OnClick;
        _gameplayScreen.CheckButton.Click += CheckButton_OnClick;
    }

    /// <inheritdoc/>
    public override void Reset()
    {
        StartedTurn = false;
        base.Reset();
    }

    /// <inheritdoc/>
    public override void StartNewGame()
    {
        StartedTurn = false;
        base.StartNewGame();
    }

    /// <inheritdoc/>
    public override void StartNewBettingRound()
    {
        StartedTurn = false;
        base.StartNewBettingRound();
    }

    public override void TakeTurn(int currentBetAmount, Hand communityCards, bool checkPossible)
    {
        base.TakeTurn(currentBetAmount, communityCards, checkPossible);

        if (!StartedTurn)
            StartedTurn = true;
        
        _tableBetAmount = currentBetAmount;

        // show buttons
        _gameplayScreen.ShowButtons(currentBetAmount, BetAmount, _initialBetAmount, 
            Balance, checkPossible);
    }

    void HideButtons()
    {
        _gameplayScreen.HideButtons();
    }

    void OnStartedTurnChanged(bool prevValue, bool newValue)
    {
        if (newValue == true)
            _initialBetAmount = BetAmount;
        else
            _initialBetAmount = -1;
    }

    void CallButton_OnClick(object o, EventArgs e)
    {
        Call(_tableBetAmount);
        HideButtons();
    }

    void AllInButton_OnClick(object o, EventArgs e)
    {
        AllIn();
        HideButtons();
    }

    void ClearButton_OnClick(object o, EventArgs e)
    {
        BetAmount = _initialBetAmount;
    }

    void RaiseButton_OnClick(object o, EventArgs e)
    {
        Raise(BetAmount);
        HideButtons();
    }

    void CheckButton_OnClick(object o, EventArgs e)
    {
        Check();
        HideButtons();
    }
}