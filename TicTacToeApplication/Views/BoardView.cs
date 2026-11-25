using System.Diagnostics;
using Spectre.Console;
using Spectre.Console.Rendering;
using TicTacToeApplication.Models.Enums;

namespace TicTacToeApplication.Views;

internal class BoardView
{
    public const int QuitMove = -1;
    public const int TimeoutMove = -2;

    private Symbol[] _board = Array.Empty<Symbol>();
    private string _playerName;
    private GameMode _mode;
    private TimerOption _timer;
    private LiveDisplayContext? _liveContext;

    /// <summary>
    /// Starts a live display session for the match.
    /// </summary>
    internal void RunMatch(Symbol[] board, string playerName, GameMode mode, TimerOption timer, Action gameLoop)
    {
        _board = board;
        _playerName = playerName;
        _mode = mode;
        _timer = timer;

        AnsiConsole.Clear();

        AnsiConsole.Live(BuildScreen(-1, Symbol.X, (int)timer))
            .AutoClear(false)
            .Start(ctx =>
            {
                _liveContext = ctx;
                gameLoop();
                _liveContext = null;
            });
    }

    /// <summary>
    /// Reads arrow-key navigation and handles the move timer for the human player.
    /// </summary>
    internal int GetPlayerMove(Symbol currentSymbol)
    {
        int selectedIndex = Array.FindIndex(_board, cell => cell == Symbol.Empty);
        if (selectedIndex == -1)
        {
            return QuitMove;
        }

        int timeLimit = (int)_timer;
        Stopwatch stopwatch = Stopwatch.StartNew();

        while (true)
        {
            int remainingSeconds = timeLimit == 0
                ? 0
                : Math.Max(0, (int)Math.Ceiling(timeLimit - stopwatch.Elapsed.TotalSeconds));

            _liveContext?.UpdateTarget(BuildScreen(selectedIndex, currentSymbol, remainingSeconds));

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
                    return QuitMove;

                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    if (_board[selectedIndex] == Symbol.Empty)
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
    internal void UpdateBoard(Symbol currentSymbol)
    {
        _liveContext?.UpdateTarget(BuildScreen(-1, currentSymbol, (int)_timer));
    }

    /// <summary>
    /// Displays the final board state along with the winner or draw banner.
    /// </summary>
    internal void DisplayResult(Symbol winner, string? customMessage = null)
    {
        string resultText = customMessage ?? winner switch
        {
            Symbol.X => $"[bold green]{Markup.Escape(_playerName)} (X) wins the match![/]",
            Symbol.O => "[bold red]Computer (O) wins the match![/]",
            _ => "[bold yellow]It's a draw![/]"
        };

        _liveContext?.UpdateTarget(BuildScreen(-1, Symbol.Empty, 0, resultText));
    }

    private IRenderable BuildScreen( int selectedIndex, Symbol currentTurn, int remainingSeconds, string? result = null)
    {
        var header = new Markup(
            $"[bold aqua]TIC TAC TOE[/]\n" +
            $"Player: [green]{Markup.Escape(_playerName)}[/]  [grey]|[/]  " +
            $"Mode: [yellow]{_mode}[/]");

        string statusLine = result ?? (
            _timer == TimerOption.None
                ? $"Turn: [bold]{currentTurn}[/]  [grey](Arrows to move, Enter to place, Esc to quit)[/]"
                : $"Turn: [bold]{currentTurn}[/]  [grey]|[/]  Time Left: [bold yellow]{remainingSeconds}s[/]");

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

        for (int i = 0; i < _board.Length; i += 3)
        {
            table.AddRow(
                FormatCell(_board[i], i == selectedIndex),
                FormatCell(_board[i + 1], i + 1 == selectedIndex),
                FormatCell(_board[i + 2], i + 2 == selectedIndex));
        }

        return Align.Center(new Rows(Align.Center(header),
            new Text(""),
            Align.Center(table),
            new Text(""),
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
