using Microsoft.Xna.Framework;

namespace Framework.UI;

/// <summary>
/// Animation that can alter an animated component.
/// </summary>
public class AnimatedGameComponentAnimation
{
    protected TimeSpan Elapsed { get; set; }
    public AnimatedGameComponent Component { get; internal set; }

    /// <summary>
    /// An action to perform before the animation begins.
    /// </summary>
    public Action<object> PerformBeforeStart;
    public object PerformBeforeStartArgs { get; set; }

    /// <summary>
    /// An action to perform once the animation is complete.
    /// </summary>
    public Action<object> PerformWhenDone;
    public object PerformWhenDoneArgs { get; set; }

    uint _animationCycles = 1;

    /// <summary>
    /// Sets the amount of cycles to perform for the animation.
    /// </summary>
    public uint AnimationCycles
    {
        get => _animationCycles;
        set
        {
            if (value > 0)
                _animationCycles = value;
        }
    }
    public DateTime StartTime { get; set; }
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Returns the time at which the animation is estimated to end.
    /// </summary>
    public TimeSpan EstimatedTimeForAnimationCompletion =>
        _isStarted ? Duration - Elapsed : StartTime - DateTime.Now + Duration;

    public bool IsLooped { get; set; }

    private bool _isDone = false;

    private bool _isStarted = false;

    /// <summary>
    /// Initializes a new instance of the class. Be default, an animation starts
    /// immediately and has a duration of 150 milliseconds.
    /// </summary>
    public AnimatedGameComponentAnimation()
    {
        StartTime = DateTime.Now;
        Duration = TimeSpan.FromMilliseconds(150);
    }

    /// <summary>
    /// Check whether or not the animation is done playing. Looped animations
    /// never finish playing.
    /// </summary>
    /// <returns>Whether or not the animation is done playing</returns>
    public bool IsDone()
    {
        if (!_isDone)
        {
            _isDone = !IsLooped && Elapsed >= Duration;
            if (_isDone && PerformWhenDone != null)
            {
                PerformWhenDone(PerformWhenDoneArgs);
                PerformWhenDone = null;
            }
        }
        return _isDone;
    }

    /// <summary>
    /// Returns whether or not the animation is started. As a side-effect, starts
    /// the animation if it is not started and it is time for it to start.
    /// </summary>
    /// <returns>Whether or not the animation is started</returns>
    public bool IsStarted()
    {
        if (!_isStarted && StartTime <= DateTime.Now)
        {
            if (PerformBeforeStart != null)
            {
                PerformBeforeStart(PerformBeforeStartArgs);
                PerformBeforeStart = null;
            }
            StartTime = DateTime.Now;
            _isStarted = true;
        }
        return _isStarted;
    }

    /// <summary>
    /// Increases the amount of elapsed time as seen by the animation, but only
    /// if the animation is started.
    /// </summary>
    /// <param name="elapsedTime">The timespan by which to incerase the animation's
    /// elapsed time.</param>
    internal void AccumulateElapsedTime(TimeSpan elapsedTime)
    {
        if (_isStarted)
            Elapsed += elapsedTime;
    }

    /// <summary>
    /// Runs the animation.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public virtual void Run(GameTime gameTime)
    {
        bool isStarted = IsStarted();
    }
}