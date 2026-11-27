using TicTacToe.Models.Enums;

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
    /// <param name="wins">Number of matches ended in win.</param>
    /// <param name="losses">Number of matches ended in loss.</param>
    /// <param name="draws">Number of matches ended draw.</param>
    /// <param name="totalPoints">Total points awarded.</param>
    public Score(string username, int wins, int losses, int draws, int totalPoints)
    {
        this.Username = username;
        this.Wins = wins;
        this.Losses = losses;
        this.Draws = draws;
        this.TotalPoints = totalPoints;
    }

    /// <summary>
    /// Gets or sets the username of the score.
    /// </summary>
    /// <value>The username of the score.</value>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the dictionary containing the score by modes.
    /// </summary>
    /// <value>The points by modes.</value>
    public Dictionary<GameMode, int> PointsByMode { get; set; } = new Dictionary<GameMode, int>()
    {
        { GameMode.Easy, 0 },
        { GameMode.Medium, 0 },
        { GameMode.Hard, 0 },
        { GameMode.Manual, 0 },
    };

    /// <summary>
    /// Gets or sets the number of wins.
    /// </summary>
    /// <value>Number of matches ended in win.</value>
    public int Wins { get; set; }

    /// <summary>
    /// Gets or sets the number of losses.
    /// </summary>
    /// <value>Number of matches ended in loss.</value>
    public int Losses { get; set; }

    /// <summary>
    /// Gets or sets the number of draw.
    /// </summary>
    /// <value>Number of matches ended in draw.</value>
    public int Draws { get; set; }

    /// <summary>
    /// Gets or sets the total score.
    /// </summary>
    /// <value>The total score of the user.</value>
    public int TotalPoints { get; set; }

    /// <summary>
    /// Gets or sets the total wins.
    /// </summary>
    /// <value>No of wins.</value>
    public int SnakeWaterGunWins { get; set; }

    /// <summary>
    /// Gets or sets the total loss.
    /// </summary>
    /// <value>No of loss.</value>
    public int SnakeWaterGunLosses { get; set; }

    /// <summary>
    /// Gets or sets the total points.
    /// </summary>
    /// <value>Total point in Snake water gun.</value>
    public int SnakeWaterGunPoints { get; set; }
}