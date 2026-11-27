using TicTacToe.Models.Enums;

namespace TicTacToe.Models;

/// <summary>
/// Game record played.
/// </summary>
internal class GameRecord
{
    /// <summary>
    /// Gets or sets the game type.
    /// </summary>
    /// <value>The value of the game type.</value>
    public GameType GameType { get; set; }

    /// <summary>
    /// Gets or sets the best of option.
    /// </summary>
    /// <value>The best of value.</value>
    public int BestOf { get; set; }

    /// <summary>
    /// Gets or sets the symbol used by Player One in the game.
    /// </summary>
    /// <value>The symbol used by Player One.</value>
    public int PlayerOneScore { get; set; }

    /// <summary>
    /// Gets or sets the symbol used by Player Two in the game.
    /// </summary>
    /// <value>The symbol used by Player Two.</value>
    public int PlayerTwoScore { get; set; }

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
    /// Gets or sets the symbol used by Player One in the game.
    /// </summary>
    /// <value>The symbol used by Player One.</value>
    public Symbol PlayerOneSymbol { get; set; }

    /// <summary>
    /// Gets or sets the symbol used by Player Two in the game.
    /// </summary>
    /// <value>The symbol used by Player Two.</value>
    public Symbol PlayerTwoSymbol { get; set; }

    /// <summary>
    /// Gets or sets the game mode for the game record.
    /// </summary>
    /// <value>The game mode for the game record.</value>
    public GameMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the game was played.
    /// </summary>
    /// <value>The date and time when the game was played.</value>
    public DateTime PlayedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the name of the winner of the game.
    /// </summary>
    /// <value>The name of the winner of the game.</value>
    public string WinnerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the time limit for each move in seconds.
    /// </summary>
    /// <value>The time limit for each move in seconds.</value>
    public TimerOption TimeLimitSeconds { get; set; }

    /// <summary>
    /// Gets or sets the reason for the game ending.
    /// </summary>
    /// <value>The reason for the game ending.</value>
    public EndReason EndReason { get; set; }

    /// <summary>
    /// Gets or sets the list of moves made during the game.
    /// </summary>
    /// <value>The list of moves made during the game.</value>
    public List<MoveRecord> Moves { get; set; } = new List<MoveRecord>();
}