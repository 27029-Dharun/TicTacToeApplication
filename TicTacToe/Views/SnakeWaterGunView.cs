using Spectre.Console;
using TicTacToe.Models;
using TicTacToe.Models.Enums;

namespace TicTacToe.Views;

/// <summary>
/// Displays the Snake Water Gun game.
/// </summary>
internal class SnakeWaterGunView
{
    /// <summary>
    /// Displays the live scoreboard at the top of each round.
    /// </summary>
    /// <param name="playerName">The name of the player.</param>
    /// <param name="userScore">User's point</param>
    /// <param name="computerScore">Computers point</param>
    /// <param name="round">Rounds completed.</param>
    /// <param name="bestOf">The best of rounds.</param>
    internal void DisplayScoreHeader(string playerName, int userScore, int computerScore, int round, int bestOf)
    {
        AnsiConsole.Clear();

        Ui.Title("Snake Water Gun");

        int winsNeeded = (bestOf / 2) + 1;

        var scoreTable = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn(new TableColumn($"[bold cyan]{Markup.Escape(playerName)}[/]").Centered())
            .AddColumn(new TableColumn($"Round {round} (Best of {bestOf})").Centered())
            .AddColumn(new TableColumn("[bold orange1]Computer[/]").Centered());

        scoreTable.AddRow(
            $"{userScore}",
            $"[grey]First to {winsNeeded}[/]",
            $"{computerScore}");

        AnsiConsole.Write(scoreTable);
        AnsiConsole.WriteLine();
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
    /// Displays the choices made by the user and the computer in the Snake Water Gun game.
    /// </summary>
    /// <param name="userChoice">The choice selected by user.</param>
    /// <param name="computerChoice">The choice selected by the computer.</param>
    /// <param name="result">The result of the round.</param>
    internal void DisplayChoices(SnakeWaterGunChoice userChoice, SnakeWaterGunChoice computerChoice, int result)
    {
        AnsiConsole.MarkupLine($"[bold cyan]You chose:[/] [white]{userChoice}[/]  |  [bold magenta]Computer chose:[/] [white]{computerChoice}[/]");

        if (result == 1)
        {
            AnsiConsole.MarkupLine("You got +1 point.");
        }
        else if (result == -1)
        {
            AnsiConsole.MarkupLine($"Computer got +1 point.");
        }
        else
        {
            AnsiConsole.MarkupLine($"It's draw.");
        }

        AnsiConsole.MarkupLine("[grey]Press any key for next round...[/]");
        Console.ReadKey();
    }

    /// <summary>
    /// Displays the game instructions for Snake Water Gun.
    /// </summary>
    internal void DisplayGameInstructions()
    {
        AnsiConsole.Clear();

        Ui.Title("Snake Water Gun Game");

        AnsiConsole.MarkupLine("[bold yellow]Instructions:[/]");
        AnsiConsole.MarkupLine("[bold yellow]1.[/] Snake beats Water");
        AnsiConsole.MarkupLine("[bold yellow]2.[/] Water beats Gun");
        AnsiConsole.MarkupLine("[bold yellow]3.[/] Gun beats Snake");
    }

    /// <summary>
    /// Displays the result of a round in the Snake Water Gun game.
    /// </summary>
    /// <param name="userScore">The score earned by the user.</param>
    /// <param name="computerScore">The score earned by the computer.</param>
    internal void DisplayMatchResult(int userScore, int computerScore)
    {
        if (userScore > computerScore)
        {
            AnsiConsole.WriteLine("You won!");
        }
        else
        {
            AnsiConsole.WriteLine("Computer wins!");
        }
    }

    /// <summary>
    /// Gets the number of rounds (Best of) for the Snake Water Gun game from the console.
    /// </summary>
    /// <returns>The choice selected by the user.</returns>
    internal SnakeWaterGunRounds GetBestOfChoice()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<SnakeWaterGunRounds>()
                .Title("[bold yellow]Enter the number of rounds (Best of):[/]")
                .AddChoices(Enum.GetValues<SnakeWaterGunRounds>()));
    }

    /// <summary>
    /// Gets the player's choice for Snake Water Gun from the console.
    /// </summary>
    /// <returns>The choice selected by the user.</returns>
    internal SnakeWaterGunChoice GetChoice()
    {
        return AnsiConsole.Prompt(new SelectionPrompt<SnakeWaterGunChoice>()
            .Title("[bold yellow]Enter your choice (Snake, Water, Gun):[/]")
            .AddChoices(Enum.GetValues<SnakeWaterGunChoice>()));
    }

    /// <summary>
    /// Gets the menu option from the user.
    /// </summary>
    /// <returns>The option selected by the user.</returns>
    internal SnakeWaterGunMenuOption GetMenuOption()
    {
        AnsiConsole.Clear();

        Ui.Title("SNAKE WATER GUN");

        var mainMenu = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an option to continue")
                    .AddChoices("Play New Game", "History", "Leaderboard", "Profile", "Back"));

        return mainMenu switch
        {
            "Play New Game" => SnakeWaterGunMenuOption.NewGame,
            "History" => SnakeWaterGunMenuOption.History,
            "Leaderboard" => SnakeWaterGunMenuOption.Leaderboard,
            "Profile" => SnakeWaterGunMenuOption.Profile,
            "Back" => SnakeWaterGunMenuOption.Back,
            _ => throw new InvalidOperationException("Invalid choice")
        };
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
            .AddColumn("Result")
            .AddColumn("Score");

        int i = 1;

        foreach (var game in gameRecords.OrderByDescending(g => g.PlayedAt).Take(100))
        {
            string result;
            int score = 0;

            if (game.EndReason == EndReason.Quit)
            {
                result = "[grey]QUIT[/]";
                score = 0;
            }
            else if (game.WinnerName == session.UserName)
            {
                result = "[green]WIN[/]";
                score = 1;
            }
            else
            {
                result = "[red]LOSS[/]";
                score = 0;
            }

            table.AddRow(
                i.ToString(),
                game.PlayedAt.ToString("dd-MM-yyyy"),
                result,
                score.ToString());

            i++;
        }

        AnsiConsole.Write(table);
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
    /// Displays the message.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    internal void DisplayInfo(string message)
    {
        Ui.Info(message);
    }

    /// <summary>
    /// Displays the profile
    /// </summary>
    /// <param name="score">The score of the user.</param>
    internal void DisplayProfile(Score score)
    {
        AnsiConsole.Clear();

        Ui.Banner(
            "PROFILE",
            $"Username: [grey]{score.Username}[/]\n");

        Ui.Banner(
            "STATISTICS",
            $"Game played: [grey]{score.SnakeWaterGunWins + score.SnakeWaterGunLosses}[/]\n" +
            $"Wins: [green]{score.SnakeWaterGunWins}[/]\nLosses: [red]{score.SnakeWaterGunLosses}[/]\n" +
            $"Total points: [yellow]{score.SnakeWaterGunPoints}[/]\n");
    }
}
