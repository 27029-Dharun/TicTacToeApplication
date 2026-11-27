using Spectre.Console;
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
}
