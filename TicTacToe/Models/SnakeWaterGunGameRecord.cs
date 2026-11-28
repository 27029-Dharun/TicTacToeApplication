namespace TicTacToe.Models;

/// <summary>
/// Represents a snake water gun game record.
/// </summary>
internal class SnakeWaterGunGameRecord : GameRecord
{
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
}
