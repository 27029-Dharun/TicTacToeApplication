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
        TicTacToeService gameService = new TicTacToeService(repositorySelector);

        TicTacToeView boardView = new TicTacToeView();
        TicTacToeController gameController = new TicTacToeController(boardView, gameService, scoreService, authController);

        SnakeWaterGunView snakeWaterGunView = new SnakeWaterGunView();
        SnakeWaterGunService snakeWaterGunService = new SnakeWaterGunService();
        SnakeWaterGunController snakeWaterGunController = new SnakeWaterGunController(snakeWaterGunView, snakeWaterGunService);

        AppView appView = new AppView();
        AppController appController = new AppController(authController, appView, gameController, scoreService, gameService, boardView, snakeWaterGunController);
        appController.Run();
    }
}