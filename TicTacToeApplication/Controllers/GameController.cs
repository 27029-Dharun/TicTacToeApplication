using TicTacToe.Models.Enums;
using TicTacToe.Services;
using TicTacToe.Views;

namespace TicTacToe.Controllers;

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
        this._boardView = boardView;
        this._ticTacToeService = ticTacToeService;
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
            Symbol result = this._ticTacToeService.IsWinCondition(moves);

            if (result != Symbol.Empty)
            {
                this._boardView.DisplayResult(result);
                break;
            }

            if (this._ticTacToeService.IsDraw(moves))
            {
                this._boardView.DisplayResult(Symbol.Empty);
                break;
            }

            int index = this._boardView.GetPlayerMove(moves, currentSymbol);
            moves[index] = currentSymbol;

            currentSymbol = this._ticTacToeService.ToggleSymbol(currentSymbol);
        }
    }
}
