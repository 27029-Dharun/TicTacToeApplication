using TicTacToeApplication.Models;
using TicTacToeApplication.Repositories.Interfaces;

namespace TicTacToeApplication.Repositories
{
    internal class RepositorySelector
    {
        private readonly JsonScoreRepository _jsonScoreRepository;
        private readonly InMemoryScoreRepository _inMemoryScoreRepository;

        public RepositorySelector(JsonScoreRepository jsonScoreRepository, InMemoryScoreRepository inMemoryScoreRepository)
        {
            _jsonScoreRepository = jsonScoreRepository;
            _inMemoryScoreRepository = inMemoryScoreRepository;
        }

        public IScoreRepository GetScoreRepository(UserSession session)
        {
            return session.IsGuest ? _inMemoryScoreRepository : _jsonScoreRepository;
        }

        internal void ClearGuest(UserSession session)
        {
            if(!session.IsGuest)
            {
                return;
            }

            _inMemoryScoreRepository.Remove();
        }
    }
}
