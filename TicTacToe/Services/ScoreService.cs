using TicTacToe.Constants;
using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Repositories;

namespace TicTacToe.Services;

/// <summary>
/// Contains the business logic for updating scores.
/// </summary>
internal class ScoreService
{
    private readonly RepositorySelector _scoreRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScoreService"/> class.
    /// </summary>
    /// <param name="repository">Instance of the repository.</param>
    public ScoreService(RepositorySelector repository)
    {
        this._scoreRepository = repository;
    }

    /// <summary>
    /// Gets the scores of top 20 players.
    /// </summary>
    /// <param name="session">User session.</param>
    /// <returns>The list of tuple with username, total points, wins.</returns>
    public List<(string Username, int TotalPoints, int Wins)> GetTicTacToeLeaderBoard(UserSession session)
    {
        return this._scoreRepository.GetScoreRepository(session).GetAll()
            .Where(s => s.TicTacToeStats.Points > 0)
            .OrderByDescending(s => s.TicTacToeStats.Points)
            .ThenByDescending(s => s.TicTacToeStats.Wins)
            .Take(20)
            .Select(s => (s.Username, s.TicTacToeStats.Points, s.TicTacToeStats.Wins))
            .ToList();
    }

    /// <summary>
    /// Gets the scores of all the repository.
    /// </summary>
    /// <param name="session">User session.</param>
    /// <returns>The list of tuple with username, total points, wins.</returns>
    public List<(string Username, int TotalPoints)> GetSnakeWaterGunLeaderBoard(UserSession session)
    {
        return this._scoreRepository.GetScoreRepository(session).GetAll()
            .Where(s => s.SnakeWaterGunStats.Wins > 0)
            .OrderByDescending(s => s.SnakeWaterGunStats.Points)
            .ThenByDescending(s => s.SnakeWaterGunStats.GamePlayed)
            .Take(20)
            .Select(s => (s.Username, s.SnakeWaterGunStats.Points))
            .ToList();
    }

    /// <summary>
    /// Gets the score of the user.
    /// </summary>
    /// <param name="session">The user session.</param>
    /// <param name="type">The game type.</param>
    /// <returns>The score of the user.</returns>
    public GameStats GetScoreByUserName(UserSession session, GameType type)
    {
        Score score = this._scoreRepository.GetScoreRepository(session).GetScore(session.UserName);

        if (type == GameType.TicTacToe)
        {
            return score.TicTacToeStats;
        }
        else
        {
            return score.SnakeWaterGunStats;
        }
    }

    /// <summary>
    /// Records win record.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <param name="mode">Game mode played.</param>
    /// <param name="session">The user session.</param>
    public void RecordTicTacToeWin(string userName, GameMode mode, UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(userName);
        int pointsEarned = Configurable.GetPointsForMode(mode);

        playerScore.TicTacToeStats.Wins++;
        playerScore.TicTacToeStats.Points += pointsEarned;
        playerScore.TicTacToeStats.PointsByMode[mode] = playerScore.TicTacToeStats.PointsByMode.GetValueOrDefault(mode) + pointsEarned;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    /// <summary>
    /// Record the loss.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <param name="session">The user session.</param>
    public void RecordTicTacToeLoss(string userName, UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(userName);
        playerScore.SnakeWaterGunStats.Losses++;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    /// <summary>
    /// Records the draw condition.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <param name="mode">Game mode of the user.</param>
    /// <param name="session">The user session.</param>
    public void RecordTicTacToeDraw(string userName, GameMode mode, UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(userName);
        playerScore.TicTacToeStats.Draws++;
        playerScore.TicTacToeStats.Points += Configurable.PointForTicTacToeDraw;
        playerScore.TicTacToeStats.PointsByMode[mode] = playerScore.TicTacToeStats.PointsByMode.GetValueOrDefault(mode) + 1;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    /// <summary>
    /// Records the snake water gun win.
    /// </summary>
    /// <param name="session">The session of the user.</param>
    internal void RecordSnakeWaterGunWin(UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(session.UserName);

        playerScore.SnakeWaterGunStats.Wins++;
        playerScore.TicTacToeStats.Points += Configurable.PointForSnakeWaterGunWin;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    /// <summary>
    /// Records the snake water gun loss.
    /// </summary>
    /// <param name="session">The session of the user.</param>
    internal void RecordSnakeWaterGunLoss(UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(session.UserName);

        playerScore.SnakeWaterGunStats.Losses++;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    /// <summary>
    /// Clears guest data when logout.
    /// </summary>
    /// <param name="session">The user session when created.</param>
    internal void ClearGuestData(UserSession session)
    {
        this._scoreRepository.ClearGuest(session);
    }
}
