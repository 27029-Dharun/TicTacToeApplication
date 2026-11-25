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
    private readonly BoardView _boardView;
    private readonly TicTacToeService _gameService;
    private readonly EasyStrategy _easyStrategy;
    private readonly MediumStrategy _mediumStrategy;
    private readonly HardStrategy _hardStrategy;
    private readonly ScoreService _scoreService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameController"/> class.
    /// </summary>
    /// <param name="boardView">Instance of the board view.</param>
    /// <param name="gameService">Instance of the game service.</param>
    /// <param name="scoreService">Instance of the score service.</param>
    public GameController(BoardView boardView, TicTacToeService gameService, ScoreService scoreService)
    {
        this._boardView = boardView;
        this._gameService = gameService;
        this._easyStrategy = new EasyStrategy();
        this._mediumStrategy = new MediumStrategy();
        this._hardStrategy = new HardStrategy();
        this._scoreService = scoreService;
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    /// <param name="mode">Game mode to start.</param>
    /// <param name="timer">Timer countdown for each move.</param>
    /// <param name="session">User session.</param>
    internal void StartGame(GameMode mode, TimerOption timer, UserSession session)
    {
        if (mode == GameMode.Manual)
        {
            this.StartManualGame(timer, session);
            return;
        }

        Symbol[] board = Enumerable.Repeat(Symbol.Empty, 9).ToArray();
        Symbol humanSymbol;
        Symbol computerSymbol;
        Symbol currentTurn = Symbol.X;

        if (session.IsStart)
        {
            humanSymbol = Symbol.X;
            computerSymbol = Symbol.O;
            session.IsStart = false;
        }
        else
        {
            humanSymbol = Symbol.O;
            computerSymbol = Symbol.X;
            session.IsStart = true;
        }

        bool quitGame = false;

        this._boardView.RunMatch(board, session.UserName, mode, timer, () =>
        {
            while (true)
            {
                if (currentTurn == humanSymbol)
                {
                    int move = this._boardView.GetPlayerMove(currentTurn);

                    if (move == BoardView.QuitMove)
                    {
                        quitGame = true;
                        break;
                    }

                    if (move == BoardView.TimeoutMove)
                    {
                        this._boardView.DisplayResult("[bold red]Time's up! Computer wins![/]");
                        break;
                    }

                    board[move] = humanSymbol;
                }
                else
                {
                    int computerMove = this.GetComputerMove(board, computerSymbol, mode);
                    Thread.Sleep(500);
                    board[computerMove] = computerSymbol;
                }

                this._boardView.UpdateBoard(currentTurn);

                Symbol winner = this._gameService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    if (winner == humanSymbol)
                    {
                        this._scoreService.RecordWin(session.UserName, mode, session);
                        this._boardView.DisplayResult($"[bold green]You({humanSymbol}) won the match![/]");
                    }
                    else
                    {
                        this._scoreService.RecordLoss(session.UserName, session);
                        this._boardView.DisplayResult($"[bold red]Computer({computerSymbol}) won the match![/]");
                    }

                    break;
                }

                if (this._gameService.IsDraw(board))
                {
                    this._boardView.DisplayResult("[bold yellow]It's a draw![/]");
                    this._scoreService.RecordDraw(session.UserName, session);
                    break;
                }

                currentTurn = this._gameService.ToggleSymbol(currentTurn);
            }
        });

        if (quitGame)
        {
            Ui.Info("Game discarded.");
        }
    }

    private void StartManualGame(TimerOption timer, UserSession session)
    {
        GameMode mode = GameMode.Manual;
        Symbol[] board = Enumerable.Repeat(Symbol.Empty, 9).ToArray();
        Symbol firstPlayerSymbol;
        Symbol secondPlayerSymbol;
        Symbol currentTurn = Symbol.X;

        string secondPlayerName = this._boardView.GetPlayerName();

        if (session.IsStart)
        {
            firstPlayerSymbol = Symbol.X;
            secondPlayerSymbol = Symbol.O;
            session.IsStart = false;
        }
        else
        {
            firstPlayerSymbol = Symbol.O;
            secondPlayerSymbol = Symbol.X;
            session.IsStart = true;
        }

        bool quitGame = false;

        this._boardView.RunMatch(board, session.UserName, GameMode.Manual, timer, () =>
        {
            while (true)
            {
                if (currentTurn == firstPlayerSymbol)
                {
                    int move = this._boardView.GetPlayerMove(currentTurn);

                    if (move == BoardView.QuitMove)
                    {
                        quitGame = true;
                        break;
                    }

                    if (move == BoardView.TimeoutMove)
                    {
                        this._boardView.DisplayResult($"[bold red]Time's up! {secondPlayerName} wins![/]");
                        break;
                    }

                    board[move] = firstPlayerSymbol;
                }
                else
                {
                    int move = this._boardView.GetPlayerMove(secondPlayerSymbol);

                    if (move == BoardView.QuitMove)
                    {
                        quitGame = true;
                        break;
                    }

                    if (move == BoardView.TimeoutMove)
                    {
                        this._boardView.DisplayResult($"[bold red]Time's up! {session.UserName} wins![/]");
                        break;
                    }

                    board[move] = secondPlayerSymbol;
                }

                this._boardView.UpdateBoard(currentTurn);

                Symbol winner = this._gameService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    if (winner == firstPlayerSymbol)
                    {
                        this._scoreService.RecordWin(session.UserName, mode, session);
                        this._boardView.DisplayResult($"[bold green]{session.UserName}({firstPlayerSymbol}) won the match![/]");
                    }
                    else
                    {
                        this._scoreService.RecordLoss(session.UserName, session);
                        this._boardView.DisplayResult($"[bold red]{secondPlayerName}({secondPlayerSymbol}) won the match![/]");
                    }

                    break;
                }

                if (this._gameService.IsDraw(board))
                {
                    this._boardView.DisplayResult("[bold yellow]It's a draw[/]");
                    break;
                }

                currentTurn = this._gameService.ToggleSymbol(currentTurn);
            }
        });

        if (quitGame)
        {
            Ui.Info("Game discarded.");
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
