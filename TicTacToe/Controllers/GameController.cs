using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Services;
using TicTacToe.Services.Strategies;
using TicTacToe.Views;

namespace TicTacToe.Controllers;

/// <summary>
/// Controls the game flow,gets input from the view and stores the record.
/// </summary>
internal class GameController
{
    private readonly AuthController _authController;
    private readonly BoardView _boardView;
    private readonly GameService _ticTacToeService;
    private readonly EasyStrategy _easyStrategy;
    private readonly MediumStrategy _mediumStrategy;
    private readonly HardStrategy _hardStrategy;
    private readonly ScoreService _scoreService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameController"/> class.
    /// </summary>
    /// <param name="boardView">Instance of the board view.</param>
    /// <param name="ticTacToeService">Instance of the game service.</param>
    /// <param name="scoreService">Instance of the score service.</param>
    /// <param name="authController">Instance of the authentication controller.</param>
    public GameController(BoardView boardView, GameService ticTacToeService, ScoreService scoreService, AuthController authController)
    {
        this._boardView = boardView;
        this._ticTacToeService = ticTacToeService;
        this._easyStrategy = new EasyStrategy();
        this._mediumStrategy = new MediumStrategy();
        this._hardStrategy = new HardStrategy();
        this._scoreService = scoreService;
        this._authController = authController;
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    /// <param name="mode">Game mode to start.</param>
    /// <param name="timer">Timer countdown for each move.</param>
    /// <param name="session">User session.</param>
    internal void StartGame(GameMode mode, TimerOption timer, UserSession session)
    {
        GameRecord game = this._ticTacToeService.CreateGame(mode, timer, session);

        if (mode == GameMode.Manual)
        {
            UserSession? playerTwoSession = this._authController.GetPlayer2();

            if (playerTwoSession is null)
            {
                this._boardView.DisplayError("Player 2 authentication failed, game cancelled.");
                return;
            }

            game.PlayerTwoName = playerTwoSession.UserName;
            this.StartManualGame(game, session, playerTwoSession);
        }
        else
        {
            this.StartComputerGame(game, session);
        }
    }

    private void StartComputerGame(GameRecord game, UserSession session)
    {
        Symbol[] board = Enumerable.Repeat(Symbol.Empty, 9).ToArray();

        game.PlayerTwoName = "Computer";
        Symbol currentTurn = Symbol.X;

        bool quitGame = false;

        this._boardView.RunMatch(board, game, () =>
        {
            while (true)
            {
                if (currentTurn == game.PlayerOneSymbol)
                {
                    int move = this._boardView.GetPlayerMove(currentTurn);

                    if (move == BoardView.QuitMove)
                    {
                        quitGame = true;
                        break;
                    }

                    if (move == BoardView.TimeoutMove)
                    {
                        this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Timeout, null, session);
                        this._scoreService.RecordLoss(session.UserName, session);
                        this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Timeout, game.PlayerTwoName, session);
                        this._boardView.DisplayResult("[bold red]Time's up! Computer wins![/]");
                        break;
                    }

                    board[move] = game.PlayerOneSymbol;
                }
                else
                {
                    int computerMove = this.GetComputerMove(board, game.PlayerTwoSymbol, game.Mode);
                    Thread.Sleep(500);
                    board[computerMove] = game.PlayerTwoSymbol;
                }

                this._boardView.UpdateBoard(currentTurn);

                Symbol winner = this._ticTacToeService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    if (winner == game.PlayerOneSymbol)
                    {
                        this._scoreService.RecordWin(session.UserName, game.Mode, session);
                        this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, game.PlayerOneName, session);
                        this._boardView.DisplayResult($"[bold green]You({game.PlayerOneSymbol}) won the match![/]");
                    }
                    else
                    {
                        this._scoreService.RecordLoss(session.UserName, session);
                        this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, game.PlayerTwoName, session);
                        this._boardView.DisplayResult($"[bold red]Computer({game.PlayerTwoSymbol}) won the match![/]");
                    }

                    break;
                }

                if (this._ticTacToeService.IsDraw(board))
                {
                    this._boardView.DisplayResult("[bold yellow]It's a draw![/]");
                    this._scoreService.RecordDraw(session.UserName, game.Mode, session);
                    this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, game.PlayerOneName, session);
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
        GameMode mode = GameMode.Manual;
        Symbol[] board = Enumerable.Repeat(Symbol.Empty, 9).ToArray();
        Symbol currentTurn = Symbol.X;

        bool quitGame = false;

        this._boardView.RunMatch(board, game, () =>
        {
            while (true)
            {
                if (currentTurn == game.PlayerOneSymbol)
                {
                    int move = this._boardView.GetPlayerMove(game.PlayerOneSymbol);

                    if (move == BoardView.QuitMove)
                    {
                        quitGame = true;
                        break;
                    }

                    if (move == BoardView.TimeoutMove)
                    {
                        this._boardView.DisplayResult($"[bold red]Time's up! {game.PlayerTwoName} wins![/]");
                        this._scoreService.RecordLoss(playerOneSession.UserName, playerOneSession);
                        this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Timeout, game.PlayerTwoName, playerOneSession, playerTwoSession);
                        break;
                    }

                    board[move] = game.PlayerOneSymbol;
                    this._ticTacToeService.AddMove(game, game.PlayerOneSymbol, move);
                }
                else
                {
                    int move = this._boardView.GetPlayerMove(game.PlayerTwoSymbol);

                    if (move == BoardView.QuitMove)
                    {
                        quitGame = true;
                        break;
                    }

                    if (move == BoardView.TimeoutMove)
                    {
                        this._boardView.DisplayResult($"[bold red]Time's up! {game.PlayerOneName} wins![/]");
                        this._scoreService.RecordLoss(playerTwoSession.UserName, playerTwoSession);
                        this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Timeout, game.PlayerOneName, playerOneSession, playerTwoSession);
                        break;
                    }

                    board[move] = game.PlayerTwoSymbol;
                    this._ticTacToeService.AddMove(game, game.PlayerTwoSymbol, move);
                }

                this._boardView.UpdateBoard(currentTurn);

                Symbol winner = this._ticTacToeService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    if (winner == game.PlayerOneSymbol)
                    {
                        this._scoreService.RecordWin(game.PlayerOneName, mode, playerOneSession);
                        this._scoreService.RecordLoss(game.PlayerTwoName, playerTwoSession);
                        this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, game.PlayerOneName, playerOneSession, playerTwoSession);
                        this._boardView.DisplayResult($"[bold green]{playerOneSession.UserName}({game.PlayerOneSymbol}) won the match![/]");
                    }
                    else
                    {
                        this._scoreService.RecordLoss(playerOneSession.UserName, playerOneSession);
                        this._scoreService.RecordWin(game.PlayerTwoName, mode, playerTwoSession);
                        this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, game.PlayerTwoName, playerOneSession, playerTwoSession);
                        this._boardView.DisplayResult($"[bold red]{game.PlayerTwoName}({game.PlayerTwoSymbol}) won the match![/]");
                    }

                    break;
                }

                if (this._ticTacToeService.IsDraw(board))
                {
                    this._scoreService.RecordDraw(game.PlayerOneName, game.Mode, playerOneSession);
                    this._scoreService.RecordDraw(game.PlayerTwoName, game.Mode, playerTwoSession);
                    this._ticTacToeService.CompleteAndSaveGame(game, EndReason.Normal, null, playerOneSession, playerTwoSession);
                    this._boardView.DisplayResult("[bold yellow]It's a draw[/]");
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
