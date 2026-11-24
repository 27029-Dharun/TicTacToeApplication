using Spectre.Console;
using Spectre.Console.Rendering;
using TicTacToe.Models.Enums;

namespace TicTacToe.Views;

/// <summary>
/// Handles drawing the Tic-Tac-Toe board, reading player moves.
/// </summary>
internal class BoardView
{
    /// <summary>
    /// Lets the current player move around the board using arrow keys and select an empty cell.
    /// </summary>
    /// <param name="board">The current state of the 9 board cells.</param>
    /// <param name="currentPlayerSymbol">The symbol (<see cref="Symbol.X"/> or <see cref="Symbol.O"/>) of the active player.</param>
    /// <returns>The chosen cell index (0 to 8), or -1 if no empty cells remain.</returns>
    internal int GetPlayerMove(Symbol[] board, Symbol currentPlayerSymbol)
    {
        int selectedIndex = Array.FindIndex(board, c => c == Symbol.Empty);
        if (selectedIndex == -1)
        {
            return -1;
        }

        int chosenIndex = -1;

        AnsiConsole.Live(this.RenderBoard(board, selectedIndex))
            .Start(ctx =>
            {
                while (chosenIndex == -1)
                {
                    ctx.UpdateTarget(this.RenderBoard(board, selectedIndex));

                    ConsoleKey key = Console.ReadKey(intercept: true).Key;

                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex = (selectedIndex - 3 + 9) % 9;
                            break;

                        case ConsoleKey.DownArrow:
                            selectedIndex = (selectedIndex + 3) % 9;
                            break;

                        case ConsoleKey.LeftArrow:
                            selectedIndex = (selectedIndex - 1 + 9) % 9;
                            break;

                        case ConsoleKey.RightArrow:
                            selectedIndex = ++selectedIndex % 9;
                            break;

                        case ConsoleKey.Enter:
                        case ConsoleKey.Spacebar:
                            if (board[selectedIndex] == Symbol.Empty)
                            {
                                chosenIndex = selectedIndex;
                            }

                            break;
                    }
                }

                // Render final move without cursor highlight
                board[chosenIndex] = currentPlayerSymbol;
                ctx.UpdateTarget(this.RenderBoard(board, -1));
            });

        return chosenIndex;
    }

    /// <summary>
    /// Displays the final outcome of the match as either a win or a draw.
    /// </summary>
    /// <param name="result">The winning <see cref="Symbol"/>, or <see cref="Symbol.Empty"/> if the match is a draw.</param>
    internal void DisplayResult(Symbol result)
    {
        if (result == Symbol.X || result == Symbol.O)
        {
            Ui.Success($"Player with symbol {result} won the match");
        }
        else
        {
            Ui.Success($"Match Draw");
        }
    }

    private Renderable RenderBoard(Symbol[] symbols, int selectedIndex)
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(Align.Center(new Markup("[bold aqua] TIC TAC TOE \n[/]")));

        Table ticTacToeBoard = new Table()
            .Border(TableBorder.Heavy)
            .BorderColor(Color.Aqua)
            .HideHeaders()
            .ShowRowSeparators();

        for (int i = 0; i < 3; i++)
        {
            ticTacToeBoard.AddColumn(new TableColumn(" ").Centered().Padding(1, 0));
        }

        for (int i = 0; i < symbols.Length; i += 3)
        {
            ticTacToeBoard.AddRow(
                this.FormatCell(symbols[i], i == selectedIndex),
                this.FormatCell(symbols[i + 1], i + 1 == selectedIndex),
                this.FormatCell(symbols[i + 2], i + 2 == selectedIndex));
        }

        return Align.Center(ticTacToeBoard);
    }

    private string FormatCell(Symbol symbol, bool isSelected)
    {
        string display = symbol == Symbol.Empty ? " " : symbol.ToString();
        string padded = $"  {display}  ";

        if (isSelected)
        {
            return $"[black on grey]{padded}[/]";
        }

        return symbol switch
        {
            Symbol.X => $"[bold aqua]{padded}[/]",
            Symbol.O => $"[bold yellow]{padded}[/]",
            _ => padded
        };
    }
}
