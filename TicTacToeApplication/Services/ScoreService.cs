using TicTacToeApplication.Models;
using TicTacToeApplication.Models.Enums;
using TicTacToeApplication.Repositories;

namespace TicTacToeApplication.Services;

internal class ScoreService
{
    private readonly IScoreRepository _scoreRepository;

    public ScoreService(IScoreRepository scoreRepository)
    {
        _scoreRepository = scoreRepository;
    }

    public List<(string Username, int TotalPoints, int Wins)> GetScores()
    {
        return _scoreRepository.GetAll()
            .OrderByDescending(s => s.TotalPoints)
            .ThenByDescending(s => s.Wins)
            .Take(20)
            .Select(s => (s.Username, s.TotalPoints, s.Wins))
            .ToList();
    }

    public Score GetScoreByUserName(string userName)
    {
        return _scoreRepository.GetScore(userName);
    }

    public void RecordWin(string userName, GameMode mode)
    {
        Score playerScore = _scoreRepository.GetScore(userName);
        int pointsEarned = GetPointsForMode(mode);

        playerScore.Wins++;
        playerScore.TotalPoints += pointsEarned;
        playerScore.PointsByMode[mode] = playerScore.PointsByMode.GetValueOrDefault(mode) + pointsEarned;

        _scoreRepository.SaveScore(playerScore);
    }

    public void RecordLoss(string userName)
    {
        Score playerScore = _scoreRepository.GetScore(userName);
        playerScore.Losses++;

        _scoreRepository.SaveScore(playerScore);
    }

    public void RecordDraw(string userName)
    {
        Score playerScore = _scoreRepository.GetScore(userName);
        playerScore.Draws++;

        _scoreRepository.SaveScore(playerScore);
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
