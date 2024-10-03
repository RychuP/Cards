namespace Framework.Engine;

/// <summary>
/// Represents a rule in card game.
/// </summary>
/// <remarks>
/// Inherit from this class and write your code
/// </remarks>
public abstract class GameRule
{
    /// <summary>
    /// An event which triggers when the rule conditions are matched.
    /// </summary>
    public event EventHandler RuleMatch;

    /// <summary>
    /// Checks whether the rule conditions are met. Should call 
    /// <see cref="OnRuleMatch"/>.
    /// </summary>
    public abstract void Check();

    /// <summary>
    /// Fires the rule's event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    protected void OnRuleMatch(EventArgs e)
    {
        RuleMatch?.Invoke(this, e);
    }
}