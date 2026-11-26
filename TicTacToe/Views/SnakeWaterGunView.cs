using Spectre.Console;
using TicTacToe.Models.Enums;

namespace TicTacToe.Views;

/// <summary>
/// Displays the Snake Water Gun game.
/// </summary>
internal class SnakeWaterGunView
{
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
    internal void DisplayChoices(SnakeWaterGunChoice userChoice, SnakeWaterGunChoice computerChoice)
    {
        AnsiConsole.MarkupLine($"[bold yellow]You chose:[/] {userChoice}");
        AnsiConsole.MarkupLine($"[bold yellow]Computer chose:[/] {computerChoice}");
    }

    /// <summary>
    /// Displays the game instructions for Snake Water Gun.
    /// </summary>
    internal void DisplayGameInstructions()
    {
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
    internal void DisplayRoundResult(int userScore, int computerScore)
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
    internal int GetBestOfChoice()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("[bold yellow]Enter the number of rounds (Best of):[/]")
                .AddChoices(1, 3, 5, 7, 9));
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
