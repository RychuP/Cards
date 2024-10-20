using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.BaseScreens;
using System.Collections.Generic;

namespace Solitaire.UI.Window;

/// <summary>
/// Window with some information displayed in the dark area below menu screen titles.
/// </summary>
internal class Window : DrawableGameComponent
{
    /// <summary>
    /// Top and bottom margin for text lines.
    /// </summary>
    public static readonly int Margin = 110;

    /// <summary>
    /// Vertical space between text lines.
    /// </summary>
    public static readonly int LineSpacing = 10;

    /// <summary>
    /// Duration for the fade animation.
    /// </summary>
    static readonly TimeSpan FadeDuration = TimeSpan.FromSeconds(0.5d);
    protected double _percent;

    public MenuScreen MenuScreen { get; }
    public GameManager GameManager { get; }
    public Vector2 Position { get; }
    public Point Size { get; }
    public List<TextLine> TextLines { get; } = new();

    // backing field
    float _opacity;
    public float Opacity
    {
        get => _opacity;
        set
        {
            if (_opacity == value) return;
            _opacity = Math.Clamp(value, 0, 1);
            OnOpacityChanged();
        }
    }

    // backing field
    Visibility _visibility;
    public Visibility Visibility
    {
        get => _visibility;
        set
        {
            if (_visibility == value) return;
            _visibility = value;
            OnVisibilityChanged();
        }
    }

    public Window(Vector2 position, Rectangle bounds, MenuScreen menuScreen) : base(menuScreen.GameManager.Game)
    { 
        MenuScreen = menuScreen;
        GameManager = menuScreen.GameManager;
        Position = position;
        Size = bounds.Size;
        Visible = false;
        Enabled = false;

        menuScreen.Title.DestinationReached += MenuScreen_Title_OnDestinationReached;
        menuScreen.VisibleChanged += MenuScreen_OnVisibleChanged;
    }

    public void Show() => Visibility = Visibility.Visible;
    public void Hide() => Visibility = Visibility.Hidden;
    protected void AddText(string text) => TextLines.Add(new TextLine(text, this));

    public override void Update(GameTime gameTime)
    {
        if (Visibility == Visibility.Visible && Opacity != 1)
        {
            _percent += gameTime.ElapsedGameTime.TotalSeconds / FadeDuration.TotalSeconds;
            Opacity = (float)_percent;
            
        }
        else if (Visibility == Visibility.Hidden && Opacity != 0)
        {
            Opacity = 0;
            return;
            _percent += gameTime.ElapsedGameTime.TotalSeconds / FadeDuration.TotalSeconds;
            Opacity = (float)(1 - _percent);
        }
    }

    void OnVisibilityChanged()
    {
        if (Visibility == Visibility.Visible)
        {
            Visible = true;
        }
        Enabled = true;
    }

    void OnOpacityChanged()
    {
        if (Opacity == 1)
        {
            _percent = 0;
            Enabled = false;
        }
        if (Opacity == 0)
        {
            _percent = 0;
            Visible = false;
            Enabled = false;
        }
    }

    void MenuScreen_Title_OnDestinationReached(object o, DirectionEventArgs e)
    {
        if (e.Direction == Direction.Up)
            Show();
    }

    void MenuScreen_OnVisibleChanged(object o, EventArgs e)
    {
        if (MenuScreen.Visible == false)
            Hide();
        base.OnVisibleChanged(o, e);
    }
}