using TicTacToe.Models.Enums;

namespace TicTacToe.Services.Strategies;

/// <summary>
/// Hard strategy for the computer player in Tic Tac Toe.
/// </summary>
internal class HardStrategy : IMoveStrategy
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

        if (board.All(c => c == Symbol.Empty))
        {
            return Corners[Random.Shared.Next(Corners.Length)];
        }

        if (board[4] == computerSymbol && HasOppositeCorners(board, opponentSymbol))
        {
            int[] openEdges = Edges.Where(i => board[i] == Symbol.Empty).ToArray();
            if (openEdges.Length > 0)
            {
                return openEdges[Random.Shared.Next(openEdges.Length)];
            }
        }

        if (board[4] == Symbol.Empty)
        {
            return 4;
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

    private static bool HasOppositeCorners(Symbol[] board, Symbol symbol)
    {
        return (board[0] == symbol && board[8] == symbol) ||
               (board[2] == symbol && board[6] == symbol);
    }

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
