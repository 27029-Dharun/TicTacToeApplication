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

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositorySelector"/> class.
    /// </summary>
    /// <param name="jsonScoreRepository">Instance of the json repository.</param>
    /// <param name="inMemoryScoreRepository">Instance of the in-memory repository.</param>
    public RepositorySelector(JsonScoreRepository jsonScoreRepository, InMemoryScoreRepository inMemoryScoreRepository)
    {
        this._jsonScoreRepository = jsonScoreRepository;
        this._inMemoryScoreRepository = inMemoryScoreRepository;
    }

    /// <summary>
    /// Get the repository according to the user..
    /// </summary>
    /// <param name="session">The user session.</param>
    /// <returns>In-memory repository for the guest user; Json file for the logged in user.</returns>
    public IScoreRepository GetScoreRepository(UserSession session)
    {
        return session.IsGuest ? this._inMemoryScoreRepository : this._jsonScoreRepository;
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
    }
}
