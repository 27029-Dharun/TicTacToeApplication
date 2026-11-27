using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Services;
using TicTacToe.Services.Strategies;
using TicTacToe.Views;

namespace TicTacToe.Controllers;

/// <summary>
/// Controls the game flow,gets input from the view and stores the record.
/// </summary>
internal class TicTacToeController
{
    private readonly AuthController _authController;
    private readonly TicTacToeView _ticTacToeView;
    private readonly TicTacToeService _ticTacToeService;
    private readonly ScoreService _scoreService;
    private readonly EasyStrategy _easyStrategy;
    private readonly MediumStrategy _mediumStrategy;
    private readonly HardStrategy _hardStrategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="TicTacToeController"/> class.
    /// </summary>
    /// <param name="boardView">Instance of the board view.</param>
    /// <param name="ticTacToeService">Instance of the game service.</param>
    /// <param name="scoreService">Instance of the score service.</param>
    /// <param name="authController">Instance of the authentication controller.</param>
    public TicTacToeController(TicTacToeView boardView, TicTacToeService ticTacToeService, ScoreService scoreService, AuthController authController)
    {
        this._ticTacToeView = boardView;
        this._ticTacToeService = ticTacToeService;
        this._easyStrategy = new EasyStrategy();
        this._mediumStrategy = new MediumStrategy();
        this._hardStrategy = new HardStrategy();
        this._scoreService = scoreService;
        this._authController = authController;
    }

    /// <summary>
    /// Run the main menu for the Tic Tac Toe game.
    /// </summary>
    /// <param name="session">The user session.</param>
    public void RunMainMenu(UserSession session)
    {
        while (true)
        {
            Score score = this._scoreService.GetScoreByUserName(session);
            TicTacToeMenuOption option = this._ticTacToeView.GetMenuOption(session, score.TotalPoints, score.Wins);

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
                        this._ticTacToeView.DisplayInfo("Login as user to the see the leaderboard\n");
                        break;
                    }

                    this.DisplayLeaderBoard(session);
                    break;

                case TicTacToeMenuOption.Back:
                    return;
            }

            Ui.Pause();
        }
    }

    private void HandleReplay(UserSession session)
    {
        List<GameRecord> gameRecords = this._ticTacToeService.GetGameReplay(session);

        if (gameRecords.Count == 0)
        {
            this._ticTacToeView.DisplayInfo("No game history found");
            return;
        }

        this._ticTacToeView.DisplayHistory(gameRecords, session);

        int gameNumber = this._ticTacToeView.GetGameToReplay(gameRecords.Capacity);

        this._ticTacToeView.ReplayGame(gameRecords[gameNumber - 1]);
    }

    private void DisplayHistory(UserSession session)
    {
        List<GameRecord> gameRecords = this._ticTacToeService.GetGameHistory(session);
        this._ticTacToeView.DisplayHistory(gameRecords, session);
    }

    private void DisplayProfile(UserSession session)
    {
        Score score = this._scoreService.GetScoreByUserName(session);
        this._ticTacToeView.DisplayProfile(score);
    }

    private void DisplayLeaderBoard(UserSession session)
    {
        List<(string Username, int TotalPoints, int Wins)> leaderboard = this._scoreService.GetTicTacToeLeaderBoard(session);
        this._ticTacToeView.DisplayLeaderBoard(session.UserName, leaderboard);
    }

    private void StartNewGame(UserSession session)
    {
        GameMode mode = this._ticTacToeView.GetGameMode();
        TimerOption timerOption = this._ticTacToeView.GetTimerOption();

        this.StartGame(mode, timerOption, session);
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    /// <param name="mode">Game mode to start.</param>
    /// <param name="timer">Timer countdown for each move.</param>
    /// <param name="session">User session.</param>
    private void StartGame(GameMode mode, TimerOption timer, UserSession session)
    {
        GameRecord game = this._ticTacToeService.CreateGame(mode, timer, session);

        if (mode == GameMode.Manual)
        {
            UserSession? playerTwoSession = this._authController.GetPlayer2();

            if (playerTwoSession is null)
            {
                this._ticTacToeView.DisplayError("Player 2 authentication failed, game cancelled.");
                return;
            }

            game.PlayerTwoName = playerTwoSession.UserName;
            game.Mode = GameMode.Manual;
            this.StartManualGame(game, session, playerTwoSession);
        }
        else
        {
            game.PlayerTwoName = "Computer";
            this.StartComputerGame(game, session);
        }
    }

    private void StartComputerGame(GameRecord game, UserSession session)
    {
        Symbol[] board = this.CreateBoard();
        Symbol currentTurn = Symbol.X;

        bool quitGame = false;

        this._ticTacToeView.RunMatch(board, game, () =>
        {
            while (true)
            {
                if (currentTurn == game.PlayerOneSymbol)
                {
                    int move = this._ticTacToeView.GetPlayerMove(currentTurn);

                    if (move == TicTacToeView.QuitMove)
                    {
                        quitGame = true;
                        break;
                    }

                    if (move == TicTacToeView.TimeoutMove)
                    {
                        this._scoreService.RecordLoss(session.UserName, session);
                        this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Timeout, game.PlayerTwoName, session);
                        this._ticTacToeView.DisplayResult("[bold red]Time's up! Computer wins![/]");
                        break;
                    }

                    board[move] = game.PlayerOneSymbol;
                    this._ticTacToeService.AddMove(game, game.PlayerOneSymbol, move);
                }
                else
                {
                    int computerMove = this.GetComputerMove(board, game.PlayerTwoSymbol, game.Mode);
                    Thread.Sleep(500);

                    board[computerMove] = game.PlayerTwoSymbol;
                    this._ticTacToeService.AddMove(game, game.PlayerTwoSymbol, computerMove);
                }

                this._ticTacToeView.UpdateBoard(currentTurn);

                Symbol winner = this._ticTacToeService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    this.HandleComputerWin(game, session, winner);
                    break;
                }

                if (this._ticTacToeService.IsDraw(board))
                {
                    this._scoreService.RecordDraw(session.UserName, game.Mode, session);
                    this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, null, session);
                    this._ticTacToeView.DisplayResult("[bold yellow]It's a draw![/]");
                    break;
                }

                currentTurn = this._ticTacToeService.ToggleSymbol(currentTurn);
            }
        });

        if (quitGame)
        {
            Ui.Info("Game discarded.");
            this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Quit, null, session);
        }
    }

    private void StartManualGame(GameRecord game, UserSession playerOneSession, UserSession playerTwoSession)
    {
        Symbol[] board = this.CreateBoard();
        Symbol currentTurn = Symbol.X;
        GameMode mode = GameMode.Manual;

        bool quitGame = false;

        this._ticTacToeView.RunMatch(board, game, () =>
        {
            while (true)
            {
                int move = this._ticTacToeView.GetPlayerMove(currentTurn);

                if (move == TicTacToeView.QuitMove)
                {
                    quitGame = true;
                    break;
                }

                if (move == TicTacToeView.TimeoutMove)
                {
                    this.HandleManualTimeOut(game, playerOneSession, playerTwoSession, mode);
                    break;
                }

                board[move] = currentTurn;
                this._ticTacToeService.AddMove(game, currentTurn, move);

                this._ticTacToeView.UpdateBoard(currentTurn);

                Symbol winner = this._ticTacToeService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    this.HandleManualWinner(game, playerOneSession, playerTwoSession, mode, winner);
                    break;
                }

                if (this._ticTacToeService.IsDraw(board))
                {
                    this._scoreService.RecordDraw(game.PlayerOneName, game.Mode, playerOneSession);
                    this._scoreService.RecordDraw(game.PlayerTwoName, game.Mode, playerTwoSession);
                    this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, null, playerOneSession, playerTwoSession);
                    this._ticTacToeView.DisplayResult("[bold yellow]It's a draw[/]");
                    break;
                }

                currentTurn = this._ticTacToeService.ToggleSymbol(currentTurn);
            }
        });

        if (quitGame)
        {
            Ui.Info("Game discarded.");
            this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Quit, null, playerOneSession, playerTwoSession);
        }
    }

    private Symbol[] CreateBoard()
    {
        return Enumerable.Repeat(Symbol.Empty, 9).ToArray();
    }

    private void HandleManualWinner(GameRecord game, UserSession playerOneSession, UserSession playerTwoSession, GameMode mode, Symbol winner)
    {
        if (winner == game.PlayerOneSymbol)
        {
            this._scoreService.RecordWin(game.PlayerOneName, mode, playerOneSession);
            this._scoreService.RecordLoss(game.PlayerTwoName, playerTwoSession);
            this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, game.PlayerOneName, playerOneSession, playerTwoSession);
            this._ticTacToeView.DisplayResult($"[bold green]{playerOneSession.UserName}({game.PlayerOneSymbol}) won the match![/]");
        }
        else
        {
            this._scoreService.RecordLoss(playerOneSession.UserName, playerOneSession);
            this._scoreService.RecordWin(game.PlayerTwoName, mode, playerTwoSession);
            this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, game.PlayerTwoName, playerOneSession, playerTwoSession);
            this._ticTacToeView.DisplayResult($"[bold red]{game.PlayerTwoName}({game.PlayerTwoSymbol}) won the match![/]");
        }
    }

    private void HandleComputerWin(GameRecord game, UserSession session, Symbol winner)
    {
        if (winner == game.PlayerOneSymbol)
        {
            this._scoreService.RecordWin(session.UserName, game.Mode, session);
            this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, game.PlayerOneName, session);
            this._ticTacToeView.DisplayResult($"[bold green]You({game.PlayerOneSymbol}) won the match![/]");
        }
        else
        {
            this._scoreService.RecordLoss(session.UserName, session);
            this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, game.PlayerTwoName, session);
            this._ticTacToeView.DisplayResult($"[bold red]Computer({game.PlayerTwoSymbol}) won the match![/]");
        }
    }

    private void HandleManualTimeOut(GameRecord game, UserSession playerOneSession, UserSession playerTwoSession, GameMode mode)
    {
        this._scoreService.RecordLoss(playerOneSession.UserName, playerOneSession);
        this._scoreService.RecordWin(playerTwoSession.UserName, mode, playerTwoSession);
        this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Timeout, game.PlayerTwoName, playerOneSession, playerTwoSession);
        this._ticTacToeView.DisplayResult($"[bold red]Time's up! {game.PlayerTwoName} wins![/]");
    }

    private int GetComputerMove(Symbol[] board, Symbol computerSymbol, GameMode mode)
    {
        return mode switch
        {
            GameMode.Easy => this._easyStrategy.GetNextMove(board, computerSymbol),
            GameMode.Medium => this._mediumStrategy.GetNextMove(board, computerSymbol),
            GameMode.Hard => this._hardStrategy.GetNextMove(board, computerSymbol),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Unsupported game mode.")
        };
    }
}
