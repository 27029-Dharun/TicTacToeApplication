using TicTacToe.Models.Enums;

namespace TicTacToe.Models;

/// <summary>
/// Game record played.
/// </summary>
internal class GameRecord
{
    /// <summary>
    /// Gets or sets the player names for the game record.
    /// </summary>
    /// <value>The name of the player.</value>
    public string PlayerOneName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets opponent player name for the game record.
    /// </summary>
    /// <value>The name of the opponent player.</value>
    public string PlayerTwoName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the game was played.
    /// </summary>
    /// <value>The date and time when the game was played.</value>
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the name of the winner of the game.
    /// </summary>
    /// <value>The name of the winner of the game.</value>
    public string WinnerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the reason for the game ending.
    /// </summary>
    /// <value>The reason for the game ending.</value>
    public EndReason EndReason { get; set; }
}