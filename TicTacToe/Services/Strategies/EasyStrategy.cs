using TicTacToe.Models.Enums;

namespace TicTacToe.Services.Strategies;

/// <summary>
/// Selects a random available cell for the computer.
/// </summary>
internal class EasyStrategy : IMoveStrategy
{
    /// <summary>
    /// Selects a random empty cell from the board.
    /// </summary>
    /// <param name="board">Current board state.</param>
    /// <param name="computerSymbol">Computer's symbol.</param>
    /// <returns>Index of the selected cell.</returns>
    public int GetNextMove(Symbol[] board, Symbol computerSymbol)
    {
        ArgumentNullException.ThrowIfNull(board);

        List<int> availableCells = Enumerable
            .Range(0, board.Length)
            .Where(index => board[index] == Symbol.Empty)
            .ToList();

        if (availableCells.Count == 0)
        {
            return -1;
        }

        int randomIndex = Random.Shared.Next(availableCells.Count);

        return availableCells[randomIndex];
    }
}