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

class PointEventArgs : EventArgs
{
    public Point Position { get; }
    public PointEventArgs(Point position) =>
        Position = position;
}

class MouseDragEventArgs : EventArgs
{
    /// <summary>
    /// Current mouse position.
    /// </summary>
    public Point Position { get; }

    /// <summary>
    /// Mouse initial position when started dragging.
    /// </summary>
    public Point InitialPosition { get; }
    public MouseDragEventArgs(Point position, Point initialPosition)
    {
        Position = position;
        InitialPosition = initialPosition;
    }
}

class DirectionEventArgs : EventArgs
{
    public Direction Direction { get; }
    public DirectionEventArgs(Direction direction) =>
        Direction = direction;
}