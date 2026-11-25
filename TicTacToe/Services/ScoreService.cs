using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Repositories;

namespace TicTacToe.Services;

internal class ScoreService
{
    private readonly RepositorySelector _scoreRepository;

    public ScoreService(RepositorySelector repository)
    {
        this._scoreRepository = repository;
    }

    public List<(string Username, int TotalPoints, int Wins)> GetScores(UserSession session)
    {
        return this._scoreRepository.GetScoreRepository(session).GetAll()
            .OrderByDescending(s => s.TotalPoints)
            .ThenByDescending(s => s.Wins)
            .Take(20)
            .Select(s => (s.Username, s.TotalPoints, s.Wins))
            .ToList();
    }

    public Score GetScoreByUserName(string userName, UserSession session)
    {
        return this._scoreRepository.GetScoreRepository(session).GetScore(userName);
    }

    public void RecordWin(string userName, GameMode mode, UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(userName);
        int pointsEarned = this.GetPointsForMode(mode);

        playerScore.Wins++;
        playerScore.TotalPoints += pointsEarned;
        playerScore.PointsByMode[mode] = playerScore.PointsByMode.GetValueOrDefault(mode) + pointsEarned;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    public void RecordLoss(string userName, UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(userName);
        playerScore.Losses++;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
    }

    public void RecordDraw(string userName, UserSession session)
    {
        Score playerScore = this._scoreRepository.GetScoreRepository(session).GetScore(userName);
        playerScore.Draws++;

        this._scoreRepository.GetScoreRepository(session).SaveScore(playerScore);
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

    internal void ClearGuestData(UserSession session)
    {
        this._scoreRepository.ClearGuest(session);
    }
}
