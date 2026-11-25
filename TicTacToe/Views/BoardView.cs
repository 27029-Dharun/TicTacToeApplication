using System.Diagnostics;
using Spectre.Console;
using Spectre.Console.Rendering;
using TicTacToe.Models.Enums;
using TicTacToe.Validators;

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
    private string _playerName = string.Empty;
    private GameMode _mode;
    private TimerOption _timer;
    private LiveDisplayContext? _liveContext;

    /// <summary>
    /// Starts a live display session for the match.
    /// </summary>
    /// <param name="board">The board state.</param>
    /// <param name="playerName">Player name.</param>
    /// <param name="mode">Mode of the game.</param>
    /// <param name="timer">Timer countdown for each move.</param>
    /// <param name="gameLoop">Game loop logic.</param>
    internal void RunMatch(Symbol[] board, string playerName, GameMode mode, TimerOption timer, Action gameLoop)
    {
        this._board = board;
        this._playerName = playerName;
        this._mode = mode;
        this._timer = timer;

        AnsiConsole.Clear();

        AnsiConsole.Live(this.BuildScreen(-1, Symbol.X, (int)timer))
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
            int remainingSeconds = timeLimit == 0
                ? 0
                : Math.Max(0, (int)Math.Ceiling(timeLimit - stopwatch.Elapsed.TotalSeconds));

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
    /// Gets the player move with arrow keys.
    /// </summary>
    /// <returns>The index of the selected cell.</returns>
    internal string GetPlayerName()
    {
        int remainingAttempts = 3;

        string password = AnsiConsole.Prompt(
            new TextPrompt<string>("[grey]Enter the username of player 2: [/]"));

        while (!Validator.IsValidName(password))
        {
            if (remainingAttempts == 1)
            {
                throw new InvalidDataException("No attempt left, Please try again.\n");
            }

            Ui.Error($"Enter a valid username with minimum {Validator.MinimumNameLength} characters\n");
            Ui.Error($"Tries left: {--remainingAttempts}\n");

            password = AnsiConsole.Prompt(
            new TextPrompt<string>("[grey]Enter the username of player 2: [/]"));
        }

        return password;
    }

    private IRenderable BuildScreen(int selectedIndex, Symbol currentTurn, int remainingSeconds, string? result = null)
    {
        var header = new Markup(
            $"[bold aqua]TIC TAC TOE[/]\n" +
            $"[green]{Markup.Escape(this._playerName)}()[/] Vs  " +
            $"Mode: [yellow]{this._mode}[/]");

        string statusLine = result ?? (
            this._timer == TimerOption.None
                ? $"Turn: [bold]{currentTurn}[/]  [grey](Arrows to move, Enter to place, Esc to quit)[/]"
                : $"Turn: [bold]{currentTurn}[/]  [grey] | [/]  Time Left: [bold yellow]{remainingSeconds}s[/]");

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
