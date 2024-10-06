using Poker.UI.BaseScreens;
using System;

namespace Poker.Misc;

class ThemeChangedEventArgs : EventArgs
{
    public string Theme { get; }
    public ThemeChangedEventArgs(string theme) =>
        Theme = theme;
}

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