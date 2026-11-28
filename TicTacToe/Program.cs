using TicTacToe.Controllers;
using TicTacToe.Models;
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

        JsonGameRepository<TicTacToeGameRecord> jsonTicTacToeRepository = new JsonGameRepository<TicTacToeGameRecord>(jsonFileManager, "TicTacToe.json");
        JsonGameRepository<SnakeWaterGunGameRecord> jsonSnakeWaterGunRepository = new JsonGameRepository<SnakeWaterGunGameRecord>(jsonFileManager, "SnakeWaterGun.json");
        InMemoryGameRepository<TicTacToeGameRecord> inMemoryTicTacToeRepository = new InMemoryGameRepository<TicTacToeGameRecord>();
        InMemoryGameRepository<SnakeWaterGunGameRecord> inMemorySnakeWaterGunRepository = new InMemoryGameRepository<SnakeWaterGunGameRecord>();

        RepositorySelector repositorySelector = new RepositorySelector(jsonScoreRepository, inMemoryScoreRepository, jsonTicTacToeRepository, inMemoryTicTacToeRepository, jsonSnakeWaterGunRepository, inMemorySnakeWaterGunRepository);

        ScoreService scoreService = new ScoreService(repositorySelector);
        TicTacToeService gameService = new TicTacToeService(repositorySelector);

        TicTacToeView boardView = new TicTacToeView();
        TicTacToeController gameController = new TicTacToeController(boardView, gameService, scoreService, authController);

        SnakeWaterGunView snakeWaterGunView = new SnakeWaterGunView();
        SnakeWaterGunService snakeWaterGunService = new SnakeWaterGunService(repositorySelector);
        SnakeWaterGunController snakeWaterGunController = new SnakeWaterGunController(snakeWaterGunView, snakeWaterGunService, scoreService);

        AppView appView = new AppView();
        AppController appController = new AppController(authController, appView, gameController, scoreService, gameService, snakeWaterGunController);
        appController.Run();
    }
}