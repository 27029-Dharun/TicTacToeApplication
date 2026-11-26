using Spectre.Console;
using TicTacToe.Models;
using TicTacToe.Models.Enums;

namespace TicTacToe.Views;

/// <summary>
/// Handles the main screens, menus, and banners shown to the user in the console.
/// </summary>
internal class AppView
{
    /// <summary>
    /// Displays the main menu and gets the option chosen by the user.
    /// </summary>
    /// <param name="session">The current logged-in user session.</param>
    /// <param name="totalPoints">Total points by the user.</param>
    /// <param name="wins">Total wins by the user.</param>
    /// <returns>The selected <see cref="MainMenuOption"/>.</returns>
    internal MainMenuOption GetMenuOption(UserSession session, int totalPoints, int wins)
    {
        AnsiConsole.Clear();

        this.Banner("MAIN MENU", session.UserName, totalPoints, wins);

        var mainMenu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an option to continue")
                    .AddChoices("Play New Game", "History", "Match Replay", "Profile", "Leaderboard", "Logout"));

        return mainMenu switch
        {
            "Play New Game" => MainMenuOption.NewGame,
            "History" => MainMenuOption.History,
            "Match Replay" => MainMenuOption.MatchReplay,
            "Profile" => MainMenuOption.Profile,
            "Leaderboard" => MainMenuOption.LeaderBoard,
            "Logout" => MainMenuOption.Logout,
            _ => throw new InvalidOperationException("Invalid choice")
        };
    }

    /// <summary>
    /// Gets the game to replay.
    /// </summary>
    /// <returns>The game record selected.</returns>
    internal int GetGameToReplay()
    {
        return AnsiConsole.Prompt(new TextPrompt<int>("Enter an game number to watch replay:")
            .Validate(n =>
            {
                if (n < 1 || n > 10)
                {
                    return ValidationResult.Error("[red]Number must be between 1 and 100[/]");
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
    /// Displays a banner at the top of the screen with the title and player details.
    /// </summary>
    /// <param name="title">The heading text to show on the banner.</param>
    /// <param name="userName">The username of the player.</param>
    /// <param name="totalPoints">Total win points.</param>
    /// <param name="wins">Total wins.</param>
    internal void Banner(string title, string userName, int totalPoints, int wins)
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
    /// Displays a confirm message to the user.
    /// </summary>
    /// <returns>A boolean true if the user confirms;otherwise, false.</returns>
    internal bool ConfirmLogout()
    {
        return Ui.Confirm("Logging out will delete the user data\nDo you really want to quit?");
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
}