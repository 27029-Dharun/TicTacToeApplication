using TicTacToe.Models.Enums;

namespace TicTacToe.Models;

/// <summary>
/// Represents a single move made during a game.
/// </summary>
internal class MoveRecord
{
    /// <summary>
    /// Gets or sets the selected cell (0 to 8).
    /// </summary>
    /// <value>The index of the selected cell.</value>
    public int Cell { get; set; }

    /// <summary>
    /// Gets or sets the symbol placed in the cell.
    /// </summary>
    /// <value>The symbol placed in the cell.</value>
    public Symbol Symbol { get; set; }

    /// <summary>
    /// Gets or sets the time at which the move was made.
    /// </summary>
    /// <value>The timestamp of the move.</value>
    public DateTime Timestamp { get; set; }
}