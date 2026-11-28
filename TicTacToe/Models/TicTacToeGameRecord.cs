using TicTacToe.Models.Enums;

namespace TicTacToe.Models;

/// <summary>
/// Represents a record of a Tic Tac Toe game.
/// </summary>
internal class TicTacToeGameRecord : GameRecord
{
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
    /// Gets or sets the time limit for each move in seconds.
    /// </summary>
    /// <value>The time limit for each move in seconds.</value>
    public TimerOption TimeLimitSeconds { get; set; }

    /// <summary>
    /// Gets or sets the list of moves made during the game.
    /// </summary>
    /// <value>The list of moves made during the game.</value>
    public List<MoveRecord> Moves { get; set; } = new List<MoveRecord>();
}
