namespace TicTacToe.Models.Enums;

/// <summary>
/// Represents how a game ended.
/// </summary>
internal enum EndReason
{
    /// <summary>
    /// Represent an option to end the game in a normal mode.
    /// </summary>
    Normal,

    /// <summary>
    /// Represents an option to end the game due to timeout.
    /// </summary>
    Timeout,

    /// <summary>
    /// Represents an option to end the game due to quit.
    /// </summary>
    Quit,
}