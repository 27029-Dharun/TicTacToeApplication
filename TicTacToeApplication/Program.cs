using TicTacToe.Controllers;
using TicTacToe.Repositories;
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

        BoardView boardView = new BoardView();
        TicTacToeService ticTacToeService = new TicTacToeService();
        GameController gameController = new GameController(boardView, ticTacToeService);

        AppView appView = new AppView();
        AppController appController = new AppController(authController, appView, gameController);
        appController.Run();
    }
}