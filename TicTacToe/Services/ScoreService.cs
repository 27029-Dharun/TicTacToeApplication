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
    /// Gets the scores of all the repository.
    /// </summary>
    /// <param name="session">User session.</param>
    /// <returns>The list of tuple with username, total points, wins.</returns>
    public List<(string Username, int TotalPoints, int Wins)> GetScores(UserSession session)
    {
        return this._scoreRepository.GetScoreRepository(session).GetAll()
            .OrderByDescending(s => s.TotalPoints)
            .ThenByDescending(s => s.Wins)
            .Take(20)
            .Select(s => (s.Username, s.TotalPoints, s.Wins))
            .ToList();
    }

    /// <summary>
    /// Gets the score of the user.
    /// </summary>
    /// <param name="userName">The user name of the account.</param>
    /// <param name="session">The user session.</param>
    /// <returns>The score of the user.</returns>
    public Score GetScoreByUserName(string userName, UserSession session)
    {
        return this._scoreRepository.GetScoreRepository(session).GetScore(userName);
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
    /// <param name="session">The user session.</param>
    public void RecordDraw(string userName, UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(userName);
        playerScore.Draws++;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    /// <summary>
    /// Clears guest data when logout.
    /// </summary>
    /// <param name="session">The user session when screated.</param>
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
