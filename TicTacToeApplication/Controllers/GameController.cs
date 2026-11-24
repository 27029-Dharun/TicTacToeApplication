using TicTacToeApplication.Models.Enums;
using TicTacToeApplication.Services;
using TicTacToeApplication.Views;

namespace TicTacToeApplication.Controllers;

/// <summary>
/// Controls the game flow.
/// </summary>
internal class GameController
{
    private readonly BoardView _boardView;
    private readonly TicTacToeService _ticTacToeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameController"/> class.
    /// </summary>
    /// <param name="boardView">Instance of the view layer.</param>
    /// <param name="ticTacToeService">Instance of the tic-tac-toe game service.</param>
    public GameController(BoardView boardView, TicTacToeService ticTacToeService)
    {
        _boardView = boardView;
        _ticTacToeService = ticTacToeService;
    }

    /// <summary>
    /// Start the game by game mode.
    /// </summary>
    /// <param name="mode">The mode of the game.</param>
    /// <param name="timer">The timer option for the game.</param>
    internal void StartGame(GameMode mode, TimerOption timer)
    {
        Symbol[] moves = new Symbol[9];
        Symbol startSymbol = Symbol.X;
        Symbol currentSymbol = startSymbol;

        while (true)
        {
            Symbol result = _ticTacToeService.IsWinCondition(moves);

            if (result != Symbol.Empty)
            {
                _boardView.DisplayResult(result);
                break;
            }

            if (_ticTacToeService.IsDraw(moves))
            {
                _boardView.DisplayResult(Symbol.Empty);
                break;
            }

            int index = _boardView.GetPlayerMove(moves, currentSymbol);
            moves[index] = currentSymbol;

            currentSymbol = _ticTacToeService.ToggleSymbol(currentSymbol);
        }
    }
}
