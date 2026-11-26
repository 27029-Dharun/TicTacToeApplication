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
        bool playAgain = true;
        while (playAgain)
        {
            GameRecord game = new GameRecord()
            {
                PlayerOneName = session.UserName,
                Mode = mode,
                PlayedAt = DateTime.Now,
                TimeLimitSeconds = timer,
            };

            if (session.IsStart)
            {
                game.PlayerOneSymbol = Symbol.X;
                game.PlayerTwoSymbol = Symbol.O;
                session.IsStart = false;
            }
            else
            {
                game.PlayerOneSymbol = Symbol.O;
                game.PlayerTwoSymbol = Symbol.X;
                session.IsStart = true;
            }

            if (mode == GameMode.Manual)
            {
                this.StartManualGame(game, session);
            }
            else
            {
                this.StartComputerGame(game, session);
            }

            playAgain = this._boardView.AskPlayAgain();
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

                Symbol winner = this._gameService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    if (winner == game.PlayerOneSymbol)
                    {
                        this._scoreService.RecordWin(session.UserName, game.Mode, session);
                        this._boardView.DisplayResult($"[bold green]You({game.PlayerOneSymbol}) won the match![/]");
                    }
                    else
                    {
                        this._scoreService.RecordLoss(session.UserName, session);
                        this._boardView.DisplayResult($"[bold red]Computer({game.PlayerTwoSymbol}) won the match![/]");
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

    private void StartManualGame(GameRecord game, UserSession session)
    {
        GameMode mode = GameMode.Manual;
        Symbol[] board = Enumerable.Repeat(Symbol.Empty, 9).ToArray();
        Symbol currentTurn = Symbol.X;

        game.PlayerTwoName = this._boardView.GetPlayerName();

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
                        break;
                    }

                    board[move] = game.PlayerOneSymbol;
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
                        break;
                    }

                    board[move] = game.PlayerTwoSymbol;
                }

                this._boardView.UpdateBoard(currentTurn);

                Symbol winner = this._gameService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    if (winner == game.PlayerOneSymbol)
                    {
                        this._scoreService.RecordWin(game.PlayerOneName, mode, session);
                        this._boardView.DisplayResult($"[bold green]{session.UserName}({game.PlayerOneSymbol}) won the match![/]");
                    }
                    else
                    {
                        this._scoreService.RecordLoss(session.UserName, session);
                        this._boardView.DisplayResult($"[bold red]{game.PlayerTwoName}({game.PlayerTwoSymbol}) won the match![/]");
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
