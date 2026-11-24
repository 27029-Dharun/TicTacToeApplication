using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Views;

namespace TicTacToe.Controllers;

/// <summary>
/// Coordinates between the view and service.
/// </summary>
internal class AppController
{
    private readonly AuthController _authController;
    private readonly AppView _appView;
    private readonly GameController _gameController;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppController"/> class.
    /// </summary>
    /// <param name="authController">Instance of the authentication controller.</param>
    /// <param name="appView">Instance of the view.</param>
    /// <param name="gameController">Instance of the game controller.</param>
    public AppController(AuthController authController, AppView appView, GameController gameController)
    {
        this._authController = authController;
        this._appView = appView;
        this._gameController = gameController;
    }

    /// <summary>
    /// Entry point of the application.
    /// </summary>
    public void Run()
    {
        while (true)
        {
            UserSession? session = this._authController.HandleAuth();
            if (session == null)
            {
                break;
            }

            this.RunMainMenu(session);
        }
    }

    private void RunMainMenu(UserSession session)
    {
        while (true)
        {
            MainMenuOption option = this._appView.GetMenuOption(session);

            switch (option)
            {
                case MainMenuOption.NewGame:
                    this.StartNewGame();
                    break;

                case MainMenuOption.History:
                    break;

                case MainMenuOption.Profile:
                    break;

                case MainMenuOption.LeaderBoard:
                    break;

                case MainMenuOption.Logout:
                    return;
            }
        }
    }

    private void StartNewGame()
    {
        GameMode mode = this._appView.GetGameMode();
        TimerOption timerOption = this._appView.GetTimerOption();

        this._gameController.StartGame(mode, timerOption);

        Ui.Pause();
    }
}
