using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Repositories;

namespace TicTacToe.Services;

/// <summary>
/// Contains the business logic for updating scores.
/// </summary>
internal class ScoreService
{
    private const int PointForTicTacToeDraw = 1;
    private const int PointForSnakeWaterGunWin = 1;
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
            .Where(s => s.TotalPoints > 0)
            .OrderByDescending(s => s.TotalPoints)
            .ThenByDescending(s => s.Wins)
            .Take(20)
            .Select(s => (s.Username, s.TotalPoints, s.Wins))
            .ToList();
    }

    /// <summary>
    /// Gets the scores of all the repository.
    /// </summary>
    /// <param name="session">User session.</param>
    /// <returns>The list of tuple with username, total points, wins.</returns>
    public List<(string Username, int TotalPoints, int Wins)> GetSnakeWaterGunLeaderBoard(UserSession session)
    {
        return this._scoreRepository.GetScoreRepository(session).GetAll()
            .Where(s => s.SnakeWaterGunWins > 0)
            .OrderByDescending(s => s.SnakeWaterGunPoints)
            .ThenByDescending(s => s.SnakeWaterGunWins)
            .Take(20)
            .Select(s => (s.Username, s.SnakeWaterGunPoints, s.SnakeWaterGunWins))
            .ToList();
    }

    /// <summary>
    /// Gets the score of the user.
    /// </summary>
    /// <param name="session">The user session.</param>
    /// <returns>The score of the user.</returns>
    public Score GetScoreByUserName(UserSession session)
    {
        return this._scoreRepository.GetScoreRepository(session).GetScore(session.UserName);
    }

    /// <summary>
    /// Records win record.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <param name="mode">Game mode played.</param>
    /// <param name="session">The user session.</param>
    public void RecordWin(string userName, GameMode mode, UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(userName);
        int pointsEarned = this.GetPointsForMode(mode);

        playerScore.Wins++;
        playerScore.TotalPoints += pointsEarned;
        playerScore.PointsByMode[mode] = playerScore.PointsByMode.GetValueOrDefault(mode) + pointsEarned;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    /// <summary>
    /// Record the loss.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <param name="session">The user session.</param>
    public void RecordLoss(string userName, UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(userName);
        playerScore.Losses++;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    /// <summary>
    /// Records the draw condition.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <param name="mode">Game mode of the user.</param>
    /// <param name="session">The user session.</param>
    public void RecordDraw(string userName, GameMode mode, UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(userName);
        playerScore.Draws++;
        playerScore.TotalPoints += PointForTicTacToeDraw;
        playerScore.PointsByMode[mode] = playerScore.PointsByMode.GetValueOrDefault(mode) + 1;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    /// <summary>
    /// Records the snake water gun win.
    /// </summary>
    /// <param name="session">The session of the user.</param>
    internal void RecordSnakeWaterGunWin(UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(session.UserName);

        playerScore.SnakeWaterGunWins++;
        playerScore.SnakeWaterGunPoints += PointForSnakeWaterGunWin;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    /// <summary>
    /// Records the snake water gun loss.
    /// </summary>
    /// <param name="session">The session of the user.</param>
    internal void RecordSnakeWaterGunLoss(UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(session.UserName);

        playerScore.SnakeWaterGunLosses++;

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

    private int GetPointsForMode(GameMode mode)
    {
        return mode switch
        {
            GameMode.Easy => 1,
            GameMode.Medium => 2,
            GameMode.Hard => 3,
            GameMode.Manual => 2,
            _ => 0
        };
    }
}
