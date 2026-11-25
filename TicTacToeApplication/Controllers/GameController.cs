using TicTacToeApplication.Models;
using TicTacToeApplication.Models.Enums;
using TicTacToeApplication.Services;
using TicTacToeApplication.Services.Strategies;
using TicTacToeApplication.Views;

namespace TicTacToeApplication.Controllers;

internal class GameController
{
    private readonly BoardView _boardView;
    private readonly TicTacToeService _gameService;
    private readonly EasyStrategy _easyStrategy;
    private readonly MediumStrategy _mediumStrategy;
    private readonly HardStrategy _hardStrategy;
    private readonly ScoreService _scoreService;

    public GameController(BoardView boardView, TicTacToeService gameService, ScoreService scoreService)
    {
        _boardView = boardView;
        _gameService = gameService;
        _easyStrategy = new EasyStrategy();
        _mediumStrategy = new MediumStrategy();
        _hardStrategy = new HardStrategy();
        _scoreService = scoreService;
    }

    internal void StartGame(GameMode mode, TimerOption timer, UserSession session)
    {
        if (mode == GameMode.Manual)
        {
            StartManualGame(timer, session);
            return;
        }

        Symbol[] board = Enumerable.Repeat(Symbol.Empty, 9).ToArray();
        Symbol humanSymbol;
        Symbol computerSymbol;
        Symbol currentTurn = Symbol.X;

        if(session.IsStart)
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
                    int move = _boardView.GetPlayerMove(currentTurn);

                    if (move == BoardView.QuitMove)
                    {
                        quitGame = true;
                        break;
                    }

                    if (move == BoardView.TimeoutMove)
                    {
                        _boardView.DisplayResult("[bold red]Time's up! Computer wins![/]");
                        break;
                    }

                    board[move] = humanSymbol;
                }
                else
                {
                    int computerMove = GetComputerMove(board, computerSymbol, mode);
                    Thread.Sleep(500);
                    board[computerMove] = computerSymbol;
                }

                _boardView.UpdateBoard(currentTurn);

                Symbol winner = _gameService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    if (winner == humanSymbol)
                    {
                        _scoreService.RecordWin(session.UserName, mode, session);
                        _boardView.DisplayResult($"[bold green]You({humanSymbol}) won the match![/]");
                    }
                    else
                    {
                        _scoreService.RecordLoss(session.UserName, session);
                        _boardView.DisplayResult($"[bold red]Computer({computerSymbol}) won the match![/]");
                    }
                    break;
                }

                if (_gameService.IsDraw(board))
                {
                    _boardView.DisplayResult("[bold yellow]It's a draw![/]");
                    _scoreService.RecordDraw(session.UserName, session);
                    break;
                }

                currentTurn = _gameService.ToggleSymbol(currentTurn);
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

        _boardView.RunMatch(board, session.UserName, GameMode.Manual, timer, () =>
        {
            while (true)
            {
                if (currentTurn == firstPlayerSymbol)
                {
                    int move = _boardView.GetPlayerMove(currentTurn);

                    if (move == BoardView.QuitMove)
                    {
                        quitGame = true;
                        break;
                    }

                    if (move == BoardView.TimeoutMove)
                    {
                        _boardView.DisplayResult($"[bold red]Time's up! {secondPlayerName} wins![/]");
                        break;
                    }

                    board[move] = firstPlayerSymbol;
                }
                else
                {
                    int move = _boardView.GetPlayerMove(secondPlayerSymbol);

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

                _boardView.UpdateBoard(currentTurn);

                Symbol winner = _gameService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    if (winner == firstPlayerSymbol)
                    {
                        _scoreService.RecordWin(session.UserName, mode, session);
                        _boardView.DisplayResult($"[bold green]{session.UserName}({firstPlayerSymbol}) won the match![/]");
                    }
                    else
                    {
                        _scoreService.RecordLoss(session.UserName, session);
                        _boardView.DisplayResult($"[bold red]{secondPlayerName}({secondPlayerSymbol}) won the match![/]");
                    }
                    break;
                }

                if (_gameService.IsDraw(board))
                {
                    _boardView.DisplayResult("[bold yellow]It's a draw[/]");
                    break;
                }

                currentTurn = _gameService.ToggleSymbol(currentTurn);
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
            GameMode.Easy => _easyStrategy.GetNextMove(board, computerSymbol),
            GameMode.Medium => _mediumStrategy.GetNextMove(board, computerSymbol),
            GameMode.Hard => _hardStrategy.GetNextMove(board, computerSymbol),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Unsupported game mode.")
        };
    }
}
