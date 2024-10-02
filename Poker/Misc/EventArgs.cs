using Poker.UI;
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