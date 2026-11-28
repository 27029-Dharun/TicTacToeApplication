using TicTacToe.Models;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Repositories;

/// <summary>
/// Selects the repository according to the logged in user.
/// </summary>
internal class RepositorySelector
{
    private readonly IScoreRepository _jsonScoreRepository;
    private readonly InMemoryScoreRepository _inMemoryScoreRepository;

    private readonly IGameRepository<TicTacToeGameRecord> _jsonTicTacToeRepository;
    private readonly InMemoryGameRepository<TicTacToeGameRecord> _inMemoryTicTacToeRepository;

    private readonly IGameRepository<SnakeWaterGunGameRecord> _jsonSnakeWaterGunRepository;
    private readonly InMemoryGameRepository<SnakeWaterGunGameRecord> _inMemorySnakeWaterGunRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositorySelector"/> class.
    /// </summary>
    /// <param name="jsonScoreRepository">Instance of the json score repository.</param>
    /// <param name="inMemoryScoreRepository">Instance of the in-memory score repository.</param>
    /// <param name="jsonTicTacToeRepository">Instance of the json game repository.</param>
    /// <param name="inMemoryTicTacToeRepository">Instance of the in-memory game repository.</param>
    /// <param name="jsonSnakeWaterGunRepository">Instance of the json snake water gun repository.</param>
    /// <param name="inMemorySnakeWaterGunRepository">Instance of the in memory snake water gun repository</param>
    public RepositorySelector(
        JsonScoreRepository jsonScoreRepository,
        InMemoryScoreRepository inMemoryScoreRepository,
        IGameRepository<TicTacToeGameRecord> jsonTicTacToeRepository,
        InMemoryGameRepository<TicTacToeGameRecord> inMemoryTicTacToeRepository,
        IGameRepository<SnakeWaterGunGameRecord> jsonSnakeWaterGunRepository,
        InMemoryGameRepository<SnakeWaterGunGameRecord> inMemorySnakeWaterGunRepository)
    {
        this._jsonScoreRepository = jsonScoreRepository;
        this._inMemoryScoreRepository = inMemoryScoreRepository;
        this._jsonTicTacToeRepository = jsonTicTacToeRepository;
        this._inMemoryTicTacToeRepository = inMemoryTicTacToeRepository;
        this._inMemorySnakeWaterGunRepository = inMemorySnakeWaterGunRepository;
        this._jsonSnakeWaterGunRepository = jsonSnakeWaterGunRepository;
    }

    /// <summary>
    /// Get the score repository according to the user.
    /// </summary>
    /// <param name="session">The user session.</param>
    /// <returns>In-memory repository for the guest user; Json file for the logged in user.</returns>
    public IScoreRepository GetScoreRepository(UserSession session)
    {
        return session.IsGuest ? this._inMemoryScoreRepository : this._jsonScoreRepository;
    }

    /// <summary>
    /// Get the game repository according to the user.
    /// </summary>
    /// <param name="session">The user session.</param>
    /// <returns>In-memory repository for the guest user; Json file for the logged in user.</returns>
    public IGameRepository<TicTacToeGameRecord> GetTicTacToeRepository(UserSession session)
    {
        return session.IsGuest ? this._inMemoryTicTacToeRepository : this._jsonTicTacToeRepository;
    }

    /// <summary>
    /// Get the game repository according to the user.
    /// </summary>
    /// <param name="session">The user session.</param>
    /// <returns>In-memory repository for the guest user; Json file for the logged in user.</returns>
    public IGameRepository<SnakeWaterGunGameRecord> GetSnakeWaterGunRepository(UserSession session)
    {
        return session.IsGuest ? this._inMemorySnakeWaterGunRepository : this._jsonSnakeWaterGunRepository;
    }

    /// <summary>
    /// Clears the guest data.
    /// </summary>
    /// <param name="session">The session of the logged in user.</param>
    internal void ClearGuest(UserSession session)
    {
        if (!session.IsGuest)
        {
            return;
        }

        this._inMemoryScoreRepository.Remove();
        this._inMemoryTicTacToeRepository.Remove();
        this._inMemorySnakeWaterGunRepository.Remove();
    }
}
