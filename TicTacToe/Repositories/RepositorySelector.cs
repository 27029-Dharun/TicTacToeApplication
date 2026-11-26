using TicTacToe.Models;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Repositories;

/// <summary>
/// Selects the repository according to the logged in user.
/// </summary>
internal class RepositorySelector
{
    private readonly JsonScoreRepository _jsonScoreRepository;
    private readonly InMemoryScoreRepository _inMemoryScoreRepository;
    private readonly JsonGameRepository _jsonGameRepository;
    private readonly InMemoryGameRepository _inMemoryGameRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositorySelector"/> class.
    /// </summary>
    /// <param name="jsonScoreRepository">Instance of the json score repository.</param>
    /// <param name="inMemoryScoreRepository">Instance of the in-memory score repository.</param>
    /// <param name="jsonGameRepository">Instance of the json game repository.</param>
    /// <param name="inMemoryGameRepository">Instance of the in-memory game repository.</param>
    public RepositorySelector(JsonScoreRepository jsonScoreRepository, InMemoryScoreRepository inMemoryScoreRepository, JsonGameRepository jsonGameRepository, InMemoryGameRepository inMemoryGameRepository)
    {
        this._jsonScoreRepository = jsonScoreRepository;
        this._inMemoryScoreRepository = inMemoryScoreRepository;
        this._jsonGameRepository = jsonGameRepository;
        this._inMemoryGameRepository = inMemoryGameRepository;
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
    public IGameRepository GetGameRepository(UserSession session)
    {
        return session.IsGuest ? this._inMemoryGameRepository : this._jsonGameRepository;
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
        this._inMemoryGameRepository.Remove();
    }
}
