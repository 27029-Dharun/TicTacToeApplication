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
    private readonly TicTacToeService _gameService;
    private readonly TicTacToeView _boardView;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppController"/> class.
    /// </summary>
    /// <param name="authController">Instance of the authentication controller.</param>
    /// <param name="appView">Instance of the view.</param>
    /// <param name="gameController">Instance of the game controller.</param>
    /// <param name="scoreService">Instance of the score service.</param>
    /// <param name="gameService">Instance of game services.</param>
    /// <param name="boardView">Instance of the board view.</param>
    /// <param name="snakeWaterGunController">The instance of the snake water gun controller.</param>
    public AppController(AuthController authController, AppView appView, TicTacToeController gameController, ScoreService scoreService, TicTacToeService gameService, TicTacToeView boardView, SnakeWaterGunController snakeWaterGunController)
    {
        this._authController = authController;
        this._appView = appView;
        this._ticTacToeController = gameController;
        this._scoreService = scoreService;
        this._gameService = gameService;
        this._boardView = boardView;
        this._snakeWaterGunController = snakeWaterGunController;
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
            GameOption option = this._appView.SelectGame();

            switch(option)
            {
                case GameOption.TicTacToe:
                    this.RunMainMenu(session);
                    break;

                case GameOption.SnakeWaterGun:
                    this._snakeWaterGunController.RunSnakeWaterGunGame(session);
                    break;

                case GameOption.LogOut:
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
        }
    }

    private void RunMainMenu(UserSession session)
    {
        while (true)
        {
            Score score = this._scoreService.GetScoreByUserName(session.UserName, session);
            TicTacToeMenuOption option = this._appView.GetMenuOption(session, score.TotalPoints, score.Wins);

            switch (option)
            {
                case TicTacToeMenuOption.NewGame:
                    this.StartNewGame(session);
                    break;

                case TicTacToeMenuOption.History:
                    this.DisplayHistory(session);
                    break;

                case TicTacToeMenuOption.MatchReplay:
                    this.HandleReplay(session);
                    break;

                case TicTacToeMenuOption.Profile:
                    this.DisplayProfile(session);
                    break;

                case TicTacToeMenuOption.LeaderBoard:
                    if (session.IsGuest)
                    {
                        this._appView.DisplayInfo("Login as user to the see the leaderboard\n");
                        break;
                    }

                    this.DisplayLeaderBoard(session);
                    break;

                case TicTacToeMenuOption.Logout:
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

    private void HandleReplay(UserSession session)
    {
        List<GameRecord> gameRecords = this._gameService.GetGameReplay(session);

        if (gameRecords.Capacity == 0)
        {
            this._appView.DisplayInfo("No game history found");
            return;
        }

        this._appView.DisplayHistory(gameRecords, session);

        int gameNumber = this._appView.GetGameToReplay(gameRecords.Capacity);

        this._boardView.ReplayGame(gameRecords[gameNumber - 1]);
    }

    private void DisplayHistory(UserSession session)
    {
        List<GameRecord> gameRecords = this._gameService.GetGameHistory(session);
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

        this._ticTacToeController.StartGame(mode, timerOption, session);
    }
}
