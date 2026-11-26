using TicTacToe.Controllers;
using TicTacToe.Repositories;
using TicTacToe.Repositories.Interfaces;
using TicTacToe.Services;
using TicTacToe.Views;

namespace TicTacToe;

/// <summary>
/// Application entry point wires up the dependency once.
/// </summary>
internal class Program
{
    private static void Main()
    {
        JsonFileManager jsonFileManager = new JsonFileManager();

        IUserRepository userRepository = new JsonUserRepository(jsonFileManager, "User.json");
        AuthService authService = new AuthService(userRepository);
        AuthView authView = new AuthView();
        AuthController authController = new AuthController(authService, authView);

        JsonScoreRepository jsonScoreRepository = new JsonScoreRepository(jsonFileManager, "Score.json");
        InMemoryScoreRepository inMemoryScoreRepository = new InMemoryScoreRepository();

        JsonGameRepository jsonGameRepository = new JsonGameRepository(jsonFileManager, "Game.json");
        InMemoryGameRepository inMemoryGameRepository = new InMemoryGameRepository();

        RepositorySelector repositorySelector = new RepositorySelector(jsonScoreRepository, inMemoryScoreRepository, jsonGameRepository, inMemoryGameRepository);

        ScoreService scoreService = new ScoreService(repositorySelector);
        GameService gameService = new GameService(repositorySelector);

        BoardView boardView = new BoardView();
        GameController gameController = new GameController(boardView, gameService, scoreService, authController);

        AppView appView = new AppView();
        AppController appController = new AppController(authController, appView, gameController, scoreService, gameService);
        appController.Run();
    }
}