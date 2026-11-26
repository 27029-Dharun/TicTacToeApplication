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
    private readonly GameController _gameController;
    private readonly ScoreService _scoreService;
    private readonly GameService _gameService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppController"/> class.
    /// </summary>
    /// <param name="authController">Instance of the authentication controller.</param>
    /// <param name="appView">Instance of the view.</param>
    /// <param name="gameController">Instance of the game controller.</param>
    /// <param name="scoreService">Instance of the score service.</param>
    /// <param name="gameService">Instance of game services.</param>
    public AppController(AuthController authController, AppView appView, GameController gameController, ScoreService scoreService, GameService gameService)
    {
        this._authController = authController;
        this._appView = appView;
        this._gameController = gameController;
        this._scoreService = scoreService;
        this._gameService = gameService;
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
            Score score = this._scoreService.GetScoreByUserName(session.UserName, session);
            MainMenuOption option = this._appView.GetMenuOption(session, score.TotalPoints, score.Wins);

            switch (option)
            {
                case MainMenuOption.NewGame:
                    this.StartNewGame(session);
                    break;

                case MainMenuOption.History:
                    this.DisplayHistory(session);
                    break;

                case MainMenuOption.Profile:
                    this.DisplayProfile(session);
                    break;

                case MainMenuOption.LeaderBoard:
                    if (session.IsGuest)
                    {
                        this._appView.DisplayInfo("Login as user to the see the leaderboard\n");
                        break;
                    }

                    this.DisplayLeaderBoard(session);
                    break;

                case MainMenuOption.Logout:
                    if (session.IsGuest && this._appView.ConfirmLogout())
                    {
                        this._scoreService.ClearGuestData(session);
                        return;
                    }
                    else if (!session.IsGuest)
                    {
                        return;
                    }

                    break;
            }

            Ui.Pause();
        }
    }

    private void DisplayHistory(UserSession session)
    {
        List<GameRecord> gameRecords = this._gameService.GetGameRecords(session);
        this._appView.DisplayHistory(gameRecords, session);
    }

    private void DisplayProfile(UserSession session)
    {
        Score score = this._scoreService.GetScoreByUserName(session.UserName, session);
        this._appView.DisplayProfile(score);
    }

    private void DisplayLeaderBoard(UserSession session)
    {
        List<(string Username, int TotalPoints, int Wins)> leaderboard = this._scoreService.GetScores(session);
        this._appView.DisplayLeaderBoard(session.UserName, leaderboard);
    }

    private void StartNewGame(UserSession session)
    {
        GameMode mode = this._appView.GetGameMode();
        TimerOption timerOption = this._appView.GetTimerOption();

        this._gameController.StartGame(mode, timerOption, session);
    }
}
