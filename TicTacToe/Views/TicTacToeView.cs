using System.Diagnostics;
using Spectre.Console;
using Spectre.Console.Rendering;
using TicTacToe.Models;
using TicTacToe.Models.Enums;

namespace TicTacToe.Views;

/// <summary>
/// Display the board and handles user input for moves in the Tic Tac Toe game.
/// </summary>
internal class TicTacToeView
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
                AnsiConsole.Clear();

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
    /// Displays the error message.
    /// </summary>
    /// <param name="message">The error message to be displayed.</param>
    internal void DisplayError(string message)
    {
        Ui.Error(message);
    }

    /// <summary>
    /// Displays the main menu and gets the option chosen by the user.
    /// </summary>
    /// <param name="session">The current logged-in user session.</param>
    /// <param name="totalPoints">Total points by the user.</param>
    /// <param name="wins">Total wins by the user.</param>
    /// <returns>The selected <see cref="TicTacToeMenuOption"/>.</returns>
    internal TicTacToeMenuOption GetMenuOption(UserSession session, int totalPoints, int wins)
    {
        AnsiConsole.Clear();

        Banner("MAIN MENU", session.UserName, totalPoints, wins);

        var mainMenu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an option to continue")
                    .AddChoices("Play New Game", "History", "Match Replay", "Profile", "Leaderboard", "Back"));

        return mainMenu switch
        {
            "Play New Game" => TicTacToeMenuOption.NewGame,
            "History" => TicTacToeMenuOption.History,
            "Match Replay" => TicTacToeMenuOption.MatchReplay,
            "Profile" => TicTacToeMenuOption.Profile,
            "Leaderboard" => TicTacToeMenuOption.LeaderBoard,
            "Back" => TicTacToeMenuOption.Back,
            _ => throw new InvalidOperationException("Invalid choice")
        };
    }

    /// <summary>
    /// Gets the game to replay.
    /// </summary>
    /// <param name="length">The number of games available.</param>
    /// <returns>The game record selected.</returns>
    internal int GetGameToReplay(int length)
    {
        return AnsiConsole.Prompt(new TextPrompt<int>("Enter an game number to watch replay:")
            .Validate(n =>
            {
                if (n < 1 || n > length)
                {
                    return ValidationResult.Error($"[red]Number must be between 1 and {length}[/]");
                }

                return ValidationResult.Success();
            }));
    }

    /// <summary>
    /// Displays the game mode menu and gets the difficulty or mode selected by the user.
    /// </summary>
    /// <returns>The selected <see cref="GameMode"/>.</returns>
    internal GameMode GetGameMode()
    {
        AnsiConsole.Clear();

        Ui.Title("Game Modes");

        var mainMenu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a game mode to start")
                    .AddChoices("Easy", "Medium", "Hard", "Manual"));

        return mainMenu switch
        {
            "Easy" => GameMode.Easy,
            "Medium" => GameMode.Medium,
            "Hard" => GameMode.Hard,
            "Manual" => GameMode.Manual,
            _ => throw new InvalidOperationException("Invalid choice")
        };
    }

    /// <summary>
    /// Displays the timer options menu and gets the time limit chosen by the user.
    /// </summary>
    /// <returns>The selected <see cref="TimerOption"/>.</returns>
    internal TimerOption GetTimerOption()
    {
        AnsiConsole.Clear();

        Ui.Title("Timer Options");

        var mainMenu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select a timer option")
                    .AddChoices("No time limit", "Five second per move", "Ten second per move"));

        return mainMenu switch
        {
            "No time limit" => TimerOption.None,
            "Five second per move" => TimerOption.FiveSecond,
            "Ten second per move" => TimerOption.TenSecond,
            _ => throw new InvalidOperationException("Invalid choice")
        };
    }

    /// <summary>
    /// Displays the leader board.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <param name="leaderboard">The leaderboard data.</param>
    internal void DisplayLeaderBoard(string userName, List<(string Username, int TotalPoints, int Wins)> leaderboard)
    {
        AnsiConsole.Clear();
        Ui.Title("LEADERBOARD (TOP 20)");

        if (leaderboard.Count == 0)
        {
            Ui.Info("No scores recorded yet.\n");
            Ui.Pause();
            return;
        }

        Table table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Aqua);

        table.AddColumn(new TableColumn("[bold yellow]Rank[/]").Centered());
        table.AddColumn(new TableColumn("[bold yellow]Username[/]").LeftAligned());
        table.AddColumn(new TableColumn("[bold yellow]Total Points[/]").Centered());
        table.AddColumn(new TableColumn("[bold yellow]Wins[/]").Centered());

        for (int i = 0; i < leaderboard.Count; i++)
        {
            var entry = leaderboard[i];
            int rank = i + 1;
            bool isCurrentUser = entry.Username.Equals(userName, StringComparison.OrdinalIgnoreCase);

            if (isCurrentUser)
            {
                table.AddRow(
                    $"[aqua] #{rank} [/]",
                    $"[aqua] {Markup.Escape(entry.Username)} (You) [/]",
                    $"[aqua] {entry.TotalPoints} [/]",
                    $"[aqua] {entry.Wins} [/]");
            }
            else
            {
                string rankColor = rank switch
                {
                    1 => "bold gold1",
                    2 => "bold silver",
                    3 => "bold yellow",
                    _ => "grey"
                };

                table.AddRow(
                    $"[{rankColor}]#{rank}[/]",
                    $"[white]{entry.Username}[/]",
                    $"[yellow]{entry.TotalPoints}[/]",
                    $"[green]{entry.Wins}[/]");
            }
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    /// Displays the user profile.
    /// </summary>
    /// <param name="score">The score records of the player.</param>
    internal void DisplayProfile(Score score)
    {
        AnsiConsole.Clear();

        Ui.Banner(
            "PROFILE",
            $"Username: [grey]{score.Username}[/]\n");

        Ui.Banner(
            "STATISTICS",
            $"Game played: [grey]{score.Wins + score.Losses + score.Draws}[/]\n" +
            $"Wins: [green]{score.Wins}[/]\nLosses: [red]{score.Losses}[/]\n" +
            $"Draws: [yellow]{score.Draws}[/]\nTotal points: [yellow]{score.TotalPoints}[/]\n");

        var chart = new BarChart()
        .Width(50)
        .Label("[bold aqua]Points by Game Mode[/]")
        .CenterLabel()
        .AddItem("Easy", score.PointsByMode.GetValueOrDefault(GameMode.Easy), Color.Green)
        .AddItem("Medium", score.PointsByMode.GetValueOrDefault(GameMode.Medium), Color.Yellow)
        .AddItem("Hard", score.PointsByMode.GetValueOrDefault(GameMode.Hard), Color.Red)
        .AddItem("Manual", score.PointsByMode.GetValueOrDefault(GameMode.Manual), Color.Aqua);

        AnsiConsole.Write(Align.Center(chart));
    }

    /// <summary>
    /// Displays the info to the player.
    /// </summary>
    /// <param name="message">The message to be printed.</param>
    internal void DisplayInfo(string message)
    {
        Ui.Info(message);
    }

    /// <summary>
    /// Displays the top 100 game history.
    /// </summary>
    /// <param name="gameRecords">The list of game records.</param>
    /// <param name="session">User session</param>
    internal void DisplayHistory(List<GameRecord> gameRecords, UserSession session)
    {
        Ui.Title("HISTORY");

        if (gameRecords == null || !gameRecords.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No game history found.[/]");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Game No")
            .AddColumn("Date")
            .AddColumn("Opponent")
            .AddColumn("Result")
            .AddColumn("Mode")
            .AddColumn("Score");

        int i = 1;

        foreach (var game in gameRecords.OrderByDescending(g => g.PlayedAt).Take(100))
        {
            string opponent = game.PlayerOneName == session.UserName ? game.PlayerTwoName : game.PlayerOneName;
            string result;
            int score = 0;

            if (game.EndReason == EndReason.Quit)
            {
                result = "[grey]QUIT[/]";
                score = 0;
            }
            else if (string.IsNullOrWhiteSpace(game.WinnerName))
            {
                result = "[yellow]DRAW[/]";
                score = 1;
            }
            else if (game.WinnerName == session.UserName)
            {
                result = "[green]WIN[/]";
                score = this.GetPointsForMode(game.Mode);
            }
            else
            {
                result = "[red]LOSS[/]";
                score = 0;
            }

            table.AddRow(
                i.ToString(),
                game.PlayedAt.ToString("dd-MM-yyyy"),
                opponent,
                result,
                game.Mode.ToString(),
                score.ToString());

            i++;
        }

        AnsiConsole.Write(table);
    }

    private int GetPointsForMode(GameMode mode)
    {
        return mode switch
        {
            GameMode.Easy => 1,
            GameMode.Medium => 2,
            GameMode.Hard => 3,
            GameMode.Manual => 2,
            _ => 0
        };
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

    /// <summary>
    /// Displays a banner at the top of the screen with the title and player details.
    /// </summary>
    /// <param name="title">The heading text to show on the banner.</param>
    /// <param name="userName">The username of the player.</param>
    /// <param name="totalPoints">Total win points.</param>
    /// <param name="wins">Total wins.</param>
    private void Banner(string title, string userName, int totalPoints, int wins)
    {
        var playerInfo = new Panel(Align.Center(
            new Markup(
                $"[grey]Hello[/] [bold green]{Markup.Escape(userName)}[/]\n" +
                $"[grey]Total point[/] [bold yellow]{totalPoints} [/]" +
                $"[bold green] {wins} wins [/]")))
            .Header("[bold aqua]" + title + "[/]", Justify.Center)
            .Border(BoxBorder.Rounded)
            .BorderStyle(Color.Aqua)
            .Expand();

        AnsiConsole.Write(playerInfo);
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
