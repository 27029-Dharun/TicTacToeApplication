using TicTacToeApplication.Models;

namespace TicTacToeApplication.Repositories;

internal interface IScoreRepository
{
    void SaveScore(Score score);

    Score GetScore(string username);

    List<Score> GetAll();
}
