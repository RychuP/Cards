using Solitaire.UI.BaseScreens;

namespace Solitaire.Misc;

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