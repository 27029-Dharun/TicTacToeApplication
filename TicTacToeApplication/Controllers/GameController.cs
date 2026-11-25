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
    private readonly ScoreService _scoreService;

    public GameController(BoardView boardView, TicTacToeService gameService, ScoreService scoreService)
    {
        _boardView = boardView;
        _gameService = gameService;
        _easyStrategy = new EasyStrategy();
        _scoreService = scoreService;
    }

    internal void StartGame(GameMode mode, TimerOption timer, string userName)
    {
        if (mode == GameMode.Manual)
        {
            StartManualGame(timer, userName);
            return;
        }

        Symbol[] board = Enumerable.Repeat(Symbol.Empty, 9).ToArray();
        Symbol humanSymbol = Symbol.X;
        Symbol computerSymbol = Symbol.O;
        Symbol currentTurn = humanSymbol;
        bool quitGame = false;

        _boardView.RunMatch(board, userName, mode, timer, () =>
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
                        _boardView.DisplayResult(computerSymbol, "[bold red]Time's up! Computer wins![/]");
                        break;
                    }

                    board[move] = humanSymbol;
                }
                else
                {
                    int computerMove = GetComputerMove(board, computerSymbol, mode);
                    board[computerMove] = computerSymbol;
                }

                _boardView.UpdateBoard(currentTurn);

                Symbol winner = _gameService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    _boardView.DisplayResult(winner);

                    if (winner == humanSymbol)
                    {
                        _scoreService.RecordWin(userName, mode);
                    }
                    else
                    {
                        _scoreService.RecordLoss(userName);
                    }
                        break;
                }

                if (_gameService.IsDraw(board))
                {
                    _boardView.DisplayResult(Symbol.Empty);
                    _scoreService.RecordDraw(userName);
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

    private void StartManualGame(TimerOption timer, String userName)
    {

        Symbol[] board = Enumerable.Repeat(Symbol.Empty, 9).ToArray();
        Symbol humanSymbol = Symbol.X;
        Symbol computerSymbol = Symbol.O;
        Symbol currentTurn = humanSymbol;
        bool quitGame = false;

        _boardView.RunMatch(board, userName, GameMode.Manual, timer, () =>
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
                        _boardView.DisplayResult(computerSymbol, "[bold red]Time's up! Computer wins![/]");
                        break;
                    }

                    board[move] = humanSymbol;
                }
                else
                {
                    int move = _boardView.GetPlayerMove(computerSymbol);

                    if (move == BoardView.QuitMove)
                    {
                        quitGame = true;
                        break;
                    }

                    if (move == BoardView.TimeoutMove)
                    {
                        _boardView.DisplayResult(computerSymbol, "[bold red]Time's up! Computer wins![/]");
                        break;
                    }

                    board[move] = computerSymbol;
                }

                _boardView.UpdateBoard(currentTurn);

                Symbol winner = _gameService.IsWinCondition(board);
                if (winner != Symbol.Empty)
                {
                    _boardView.DisplayResult(winner);
                    break;
                }

                if (_gameService.IsDraw(board))
                {
                    _boardView.DisplayResult(Symbol.Empty);
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
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Unsupported game mode.")
        };
    }
}
