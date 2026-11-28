using TicTacToe.Models.Enums;

namespace TicTacToe.Models;

/// <summary>
/// The stats of the tic tac toe game.
/// </summary>
internal class TicTacToeStats : GameStats
{
    /// <summary>
    /// Gets or sets the draws in the match.
    /// </summary>
    /// <value>The number of matches ended in draw.</value>
    public int Draws { get; set; }

    /// <summary>
    /// Gets or sets the points scores in each game mode.
    /// </summary>
    /// <value>The points scored in each game mode.</value>
    public Dictionary<GameMode, int> PointsByMode { get; set; } = new Dictionary<GameMode, int>()
    {
        { GameMode.Easy, 0 },
        { GameMode.Medium, 0 },
        { GameMode.Hard, 0 },
        { GameMode.Manual, 0 },
    };

    /// <summary>
    /// Gets the total game played.
    /// </summary>
    /// <value>The total games played.</value>
    public int GamesPlayed => this.Wins + this.Losses + this.Draws;
}
