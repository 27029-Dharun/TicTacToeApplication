namespace TicTacToe.Models;

/// <summary>
/// Represents the score and statistics of a registered player.
/// </summary>
internal class Score
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Score"/> class.
    /// </summary>
    /// <param name="username">Username of the player.</param>
    public Score(string username)
    {
        this.Username = username;
    }

    /// <summary>
    /// Gets or sets the username of the score.
    /// </summary>
    /// <value>The username of the score.</value>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the tic tac toe game stats.
    /// </summary>
    /// <value>The tic tac toe game stats.</value>
    public TicTacToeStats TicTacToeStats { get; set; } = new TicTacToeStats();

    /// <summary>
    /// Gets or sets the snake water gun game stats.
    /// </summary>
    /// <value>The snake water gun game stats.</value>
    public GameStats SnakeWaterGunStats { get; set; } = new GameStats();
}