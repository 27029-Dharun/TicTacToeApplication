using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Services;
using TicTacToe.Views;

namespace TicTacToe.Controllers;

/// <summary>
/// Coordinates between the view and service.
/// </summary>
internal class AppController
{
    private readonly AuthController _authController;
    private readonly AppView _appView;
    private readonly TicTacToeController _ticTacToeController;
    private readonly SnakeWaterGunController _snakeWaterGunController;
    private readonly ScoreService _scoreService;
    private readonly TicTacToeService _ticTacToeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppController"/> class.
    /// </summary>
    /// <param name="authController">Instance of the authentication controller.</param>
    /// <param name="appView">Instance of the view.</param>
    /// <param name="gameController">Instance of the game controller.</param>
    /// <param name="scoreService">Instance of the score service.</param>
    /// <param name="ticTacToeService">Instance of the tic tac toe service.</param>
    /// <param name="snakeWaterGunController">The instance of the snake water gun controller.</param>
    public AppController(AuthController authController, AppView appView, TicTacToeController gameController, ScoreService scoreService, TicTacToeService ticTacToeService, SnakeWaterGunController snakeWaterGunController)
    {
        this._authController = authController;
        this._appView = appView;
        this._ticTacToeController = gameController;
        this._scoreService = scoreService;
        this._snakeWaterGunController = snakeWaterGunController;
        this._ticTacToeService = ticTacToeService;
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

            this.ChooseGame(session);
        }
    }

    /// <summary>
    /// Chooses the game to play based on user input and runs the corresponding game controller.
    /// </summary>
    /// <param name="session">The user session.</param>
    public void ChooseGame(UserSession session)
    {
        while (true)
        {
            try
            {
                GameOption option = this._appView.SelectGame();

                switch (option)
                {
                    case GameOption.TicTacToe:
                        this._ticTacToeController.RunMainMenu(session);
                        break;

                    case GameOption.SnakeWaterGun:
                        this._snakeWaterGunController.RunSnakeWaterGunGame(session);
                        break;

                    case GameOption.Profile:
                        this.HandleProfile(session);
                        break;

                    case GameOption.LogOut:
                        if (session.IsGuest && this._appView.ConfirmLogout())
                        {
                            this._scoreService.ClearGuestData(session);
                            this._ticTacToeService.ClearGuestData(session);
                            return;
                        }
                        else if (!session.IsGuest)
                        {
                            return;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                this._appView.DisplayError(ex.Message);
            }
        }
    }

    private void HandleProfile(UserSession session)
    {
        if (session.IsGuest)
        {
            this._appView.DisplayInfo("Login to view your profile.");
            return;
        }

        (string username, string fullName, string email, DateTime joinDate) = this._authController.GetProfile(session.UserName);

        this._appView.DisplayProfile(username, fullName, email, joinDate);

        EditOption option = this._appView.GetProfileEditOption();

        switch (option)
        {
            case EditOption.Name:
                this._authController.ChangeName(session);
                break;

            case EditOption.Password:
                this._authController.ChangePassword(session);
                break;

            case EditOption.Back:
                return;
        }

        this._appView.Pause();
    }
}
