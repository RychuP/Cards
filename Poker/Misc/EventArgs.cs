using Poker.Gameplay.Players;
using Poker.UI.BaseScreens;
using System;

namespace Poker.Misc;

class ScreenChangedEventArgs : EventArgs
{
    public GameScreen PrevScreen { get; }
    public GameScreen NewScreen { get; }
    public ScreenChangedEventArgs(GameScreen prevScreen, GameScreen newScreen) =>
        (PrevScreen, NewScreen) = (prevScreen, newScreen);
}

class PositionChangedEventArgs : EventArgs
{
    public Vector2 PrevPosition { get; }
    public Vector2 NewPosition { get; }
    public PositionChangedEventArgs(Vector2 prevPosition, Vector2 newPosition) =>
        (PrevPosition, NewPosition) = (prevPosition, newPosition);
}

class GameStateEventArgs : EventArgs
{
    public GameState PrevState { get; }
    public GameState NewState { get; }
    public GameStateEventArgs(GameState prevState, GameState newState) =>
        (PrevState, NewState) = (prevState, newState);
}

class GameEndEventArgs : EventArgs
{
    public PokerBettingPlayer Winner { get; }
    public GameEndEventArgs(PokerBettingPlayer winner) =>
        Winner = winner;
}

class PlayerChangedEventArgs : EventArgs
{
    public PokerBettingPlayer PrevPlayer { get; }
    public PokerBettingPlayer NewPlayer { get; }
    public PlayerChangedEventArgs(PokerBettingPlayer prevPlayer, PokerBettingPlayer newPlayer) =>
        (PrevPlayer, NewPlayer) = (prevPlayer, newPlayer);
}

class PlayerStateChangedEventArgs : EventArgs
{
    public PlayerState PrevState { get; }
    public PlayerState NewState { get; }
    public PlayerStateChangedEventArgs(PlayerState prevState, PlayerState newState) =>
        (PrevState, NewState) = (prevState, newState);
}