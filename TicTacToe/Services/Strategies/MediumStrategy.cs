using TicTacToe.Models.Enums;

namespace TicTacToe.Services.Strategies;

/// <summary>
/// Medium strategy for the computer player in Tic Tac Toe. It uses a combination of random moves and basic rules to choose the next move.
/// </summary>
internal class MediumStrategy : IMoveStrategy
{
    private static readonly int[][] WinLines = new int[][]
    {
            new int[] { 0, 1, 2 },
            new int[] { 3, 4, 5 },
            new int[] { 6, 7, 8 },
            new int[] { 0, 3, 6 },
            new int[] { 1, 4, 7 },
            new int[] { 2, 5, 8 },
            new int[] { 0, 4, 8 },
            new int[] { 2, 4, 6 },
    };

    private static readonly int[] Corners = new int[] { 0, 2, 6, 8 };
    private static readonly int[] Edges = new int[] { 1, 3, 5, 7 };

    /// <summary>
    /// Chooses the next move index using rules.
    /// </summary>
    /// <param name="board">The current state of the 9 board cells.</param>
    /// <param name="computerSymbol">The symbol used by the computer.</param>
    /// <returns>The chosen cell index (0 to 8), or -1 if no empty cells exist.</returns>
    public int GetNextMove(Symbol[] board, Symbol computerSymbol)
    {
        List<int> availableCells = Enumerable
            .Range(0, board.Length)
            .Where(index => board[index] == Symbol.Empty)
            .ToList();

        if (Random.Shared.Next(100) < 35)
        {
            return availableCells[Random.Shared.Next(availableCells.Capacity - 1)];
        }

        Symbol opponentSymbol = computerSymbol == Symbol.X ? Symbol.O : Symbol.X;

        int winningMove = FindWinningMove(board, computerSymbol);
        if (winningMove != -1)
        {
            return winningMove;
        }

        int blockingMove = FindWinningMove(board, opponentSymbol);
        if (blockingMove != -1)
        {
            return blockingMove;
        }

        int[] availableCorners = Corners.Where(i => board[i] == Symbol.Empty).ToArray();
        if (availableCorners.Length > 0)
        {
            return availableCorners[Random.Shared.Next(availableCorners.Length)];
        }

        int[] availableEdges = Edges.Where(i => board[i] == Symbol.Empty).ToArray();
        if (availableEdges.Length > 0)
        {
            return availableEdges[Random.Shared.Next(availableEdges.Length)];
        }

        return -1;
    }

    /// <summary>
    /// Finds an empty cell that wins for the given symbol.
    /// </summary>
    private static int FindWinningMove(Symbol[] board, Symbol targetSymbol)
    {
        foreach (int[] line in WinLines)
        {
            int targetCount = 0;
            int emptyIndex = -1;

            foreach (int index in line)
            {
                if (board[index] == targetSymbol)
                {
                    targetCount++;
                }
                else if (board[index] == Symbol.Empty)
                {
                    emptyIndex = index;
                }
            }

            if (targetCount == 2 && emptyIndex != -1)
            {
                return emptyIndex;
            }
        }

        return -1;
    }
}