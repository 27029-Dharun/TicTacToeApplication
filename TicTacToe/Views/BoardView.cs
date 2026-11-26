using System.Diagnostics;
using Spectre.Console;
using Spectre.Console.Rendering;
using TicTacToe.Models;
using TicTacToe.Models.Enums;

namespace TicTacToe.Views;

/// <summary>
/// Display the board and handles user input for moves in the Tic Tac Toe game.
/// </summary>
internal class BoardView
{
    /// <summary>
    /// Constant representing a move where the player chooses to quit the game.
    /// </summary>
    public const int QuitMove = -1;

    /// <summary>
    /// Constant representing a move where the player runs out of time to make a move.
    /// </summary>
    public const int TimeoutMove = -2;

    private Symbol[] _board = Array.Empty<Symbol>();
    private string _firstPlayerName = string.Empty;
    private string _secondPlayerName = string.Empty;
    private Symbol _firstPlayerSymbol;
    private Symbol _secondPlayerSymbol;
    private GameMode _mode;
    private TimerOption _timer;
    private LiveDisplayContext? _liveContext;

    /// <summary>
    /// Starts a live display session for the match.
    /// </summary>
    /// <param name="board">The board state.</param>
    /// <param name="game">Game record name.</param>
    /// <param name="gameLoop">Game loop logic.</param>
    internal void RunMatch(Symbol[] board, GameRecord game, Action gameLoop)
    {
        this._board = board;
        this._firstPlayerName = game.PlayerOneName;
        this._secondPlayerName = game.PlayerTwoName;
        this._firstPlayerSymbol = game.PlayerOneSymbol;
        this._secondPlayerSymbol = game.PlayerTwoSymbol;
        this._mode = game.Mode;
        this._timer = game.TimeLimitSeconds;

        AnsiConsole.Clear();

        AnsiConsole.Live(this.BuildScreen(-1, Symbol.X, (int)game.TimeLimitSeconds))
            .Start(ctx =>
            {
                this._liveContext = ctx;
                gameLoop();
                this._liveContext = null;
            });
    }

    /// <summary>
    /// Reads arrow-key navigation and handles the move timer for the human player.
    /// </summary>
    /// <param name="currentSymbol">The current symbol to get.</param>
    /// <returns>The player move entered by the user.</returns>
    internal int GetPlayerMove(Symbol currentSymbol)
    {
        int selectedIndex = Array.FindIndex(this._board, cell => cell == Symbol.Empty);
        if (selectedIndex == -1)
        {
            return QuitMove;
        }

        int timeLimit = (int)this._timer;
        Stopwatch stopwatch = Stopwatch.StartNew();

        while (true)
        {
            int remainingSeconds;
            if (timeLimit == 0)
            {
                remainingSeconds = 0;
            }
            else
            {
                remainingSeconds = Math.Max(0, (int)Math.Ceiling(timeLimit - stopwatch.Elapsed.TotalSeconds));
            }

            this._liveContext?.UpdateTarget(this.BuildScreen(selectedIndex, currentSymbol, remainingSeconds));

            if (timeLimit > 0 && stopwatch.Elapsed.TotalSeconds >= timeLimit)
            {
                return TimeoutMove;
            }

            if (!Console.KeyAvailable)
            {
                Thread.Sleep(50);
                continue;
            }

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
                    selectedIndex = (selectedIndex + 1) % 9;
                    break;

                case ConsoleKey.Escape:
                case ConsoleKey.Q:
                    return QuitMove;

                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    if (this._board[selectedIndex] == Symbol.Empty)
                    {
                        return selectedIndex;
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Replays the game with the records.
    /// </summary>
    /// <param name="gameRecord">The game records to be played.</param>
    internal void ReplayGame(GameRecord gameRecord)
    {
        if (gameRecord.Moves == null || gameRecord.Moves.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No moves recorded for this game.[/]");
            return;
        }

        Symbol[] board = Enumerable.Repeat(Symbol.Empty, 9).ToArray();

        this.RunMatch(board, gameRecord, () =>
        {
            int moveIndex = 0;

            while (moveIndex < gameRecord.Moves.Count)
            {
                var currentMove = gameRecord.Moves[moveIndex];
                string playerName = currentMove.Symbol == gameRecord.PlayerOneSymbol
                    ? gameRecord.PlayerOneName
                    : gameRecord.PlayerTwoName;

                AnsiConsole.MarkupLine($"[grey]Move {moveIndex + 1}/{gameRecord.Moves.Count} | {playerName} - {currentMove.Symbol} {currentMove.Timestamp}[/]");

                ConsoleKey key = Console.ReadKey().Key;

                if (key == ConsoleKey.Escape || key == ConsoleKey.Q)
                {
                    return;
                }

                if (key == ConsoleKey.RightArrow)
                {
                    board[currentMove.Cell] = currentMove.Symbol;

                    Symbol nextTurn = (moveIndex + 1 < gameRecord.Moves.Count)
                        ? gameRecord.Moves[moveIndex + 1].Symbol
                        : currentMove.Symbol;

                    this.UpdateBoard(nextTurn);
                    moveIndex++;
                }
                else if (key == ConsoleKey.LeftArrow && moveIndex > 0)
                {
                    moveIndex--;
                    var previousMove = gameRecord.Moves[moveIndex];
                    board[previousMove.Cell] = Symbol.Empty;

                    this.UpdateBoard(previousMove.Symbol);
                }
            }

            string outcomeText = string.IsNullOrWhiteSpace(gameRecord.WinnerName)
                ? "[bold yellow]Match ended in a Draw![/]"
                : $"[bold green]{Markup.Escape(gameRecord.WinnerName)} won the match![/]";

            string reasonText = gameRecord.EndReason != default
                ? $" [grey](Reason: {gameRecord.EndReason})[/]"
                : string.Empty;

            this.DisplayResult($"{outcomeText}{reasonText}");
        });
    }

    /// <summary>
    /// Refreshes the board on screen after a move is made.
    /// </summary>
    /// <param name="currentSymbol">Current symbol</param>
    internal void UpdateBoard(Symbol currentSymbol)
    {
        this._liveContext?.UpdateTarget(this.BuildScreen(-1, currentSymbol, (int)this._timer));
    }

    /// <summary>
    /// Displays the final board state along with the winner or draw banner.
    /// </summary>
    /// <param name="customMessage">The custom message to print.</param>
    internal void DisplayResult(string customMessage)
    {
        this._liveContext?.UpdateTarget(this.BuildScreen(-1, Symbol.Empty, 0, customMessage));
    }

    /// <summary>
    /// Asks the user if they want to play again after a game ends.
    /// </summary>
    /// <returns>True if the user want to play again;otherwise, false.</returns>
    internal bool AskPlayAgain()
    {
        AnsiConsole.WriteLine();

        string choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What do you want to do?")
                .AddChoices("Play Again", "Exit"));

        if (choice == "Play Again")
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Displays the error message.
    /// </summary>
    /// <param name="message">The error message to be displayed.</param>
    internal void DisplayError(string message)
    {
        Ui.Error(message);
    }

    private IRenderable BuildScreen(int selectedIndex, Symbol currentTurn, int remainingSeconds, string? result = null)
    {
        string firstColor = this._firstPlayerSymbol == Symbol.X ? "bold aqua" : "bold yellow";
        string secondColor = this._secondPlayerSymbol == Symbol.X ? "bold aqua" : "bold yellow";

        var header = new Markup(
            $"[bold aqua]TIC TAC TOE[/]\n\n" +
            $"[{firstColor}]{Markup.Escape(this._firstPlayerName)}({this._firstPlayerSymbol})[/] Vs " +
            $"[{secondColor}]{Markup.Escape(this._secondPlayerName)}({this._secondPlayerSymbol})[/]\n" +
            $"[grey]{this._mode}[/]");

        string statusLine = result ?? (
            this._timer == TimerOption.None
                ? $"Turn: [bold]{currentTurn}[/]\n\n[grey]Q to quit[/]"
                : $"Turn: [bold]{currentTurn}[/]  [grey] | [/]  Time Left: [bold yellow]{remainingSeconds}s[/]\n\n[grey]Q to quit[/]");

        var status = new Markup(statusLine);

        Table table = new Table()
            .Border(TableBorder.Heavy)
            .BorderColor(Color.Aqua)
            .HideHeaders()
            .ShowRowSeparators();

        for (int i = 0; i < 3; i++)
        {
            table.AddColumn(new TableColumn(" ").Centered().Padding(1, 0));
        }

        for (int i = 0; i < this._board.Length; i += 3)
        {
            table.AddRow(
                this.FormatCell(this._board[i], i == selectedIndex),
                this.FormatCell(this._board[i + 1], i + 1 == selectedIndex),
                this.FormatCell(this._board[i + 2], i + 2 == selectedIndex));
        }

        return Align.Center(new Rows(
            Align.Center(header),
            new Text(string.Empty),
            Align.Center(table),
            new Text(string.Empty),
            Align.Center(status)));
    }

    private string FormatCell(Symbol symbol, bool isSelected)
    {
        string display = symbol == Symbol.Empty ? " " : symbol.ToString();
        string padded = $"  {display}  ";

        if (isSelected)
        {
            return $"[black on aqua]{padded}[/]";
        }

        return symbol switch
        {
            Symbol.X => $"[bold aqua]{padded}[/]",
            Symbol.O => $"[bold yellow]{padded}[/]",
            _ => padded
        };
    }
}
