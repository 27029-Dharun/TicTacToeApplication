using Spectre.Console;
using TicTacToe.Models.Enums;

namespace TicTacToe.Views;

/// <summary>
/// Handles the main screens, menus, and banners shown to the user in the console.
/// </summary>
internal class AppView
{
    /// <summary>
    /// Selects the game to play from the main menu.
    /// </summary>
    /// <returns>The main menu option selected.</returns>
    internal GameOption SelectGame()
    {
        AnsiConsole.Clear();

        Ui.Title("WELCOME TO GAMES APP");

        return AnsiConsole.Prompt(new SelectionPrompt<GameOption>()
            .Title("[bold yellow]Select a game to play:[/]")
            .AddChoices(Enum.GetValues<GameOption>()));
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
    /// Displays the profile of the player
    /// </summary>
    /// <param name="username">the username of the player</param>
    /// <param name="fullName">Full name of the player.</param>
    /// <param name="email">Email of the player.</param>
    /// <param name="joinDate">Join date of the player.</param>
    internal void DisplayProfile(string username, string fullName, string email, DateTime joinDate)
    {
        AnsiConsole.Clear();
        Ui.Title("PROFILE");

        var grid = new Grid()
            .AddColumn(new GridColumn().NoWrap().PadRight(3))
            .AddColumn();

        grid.AddRow("[bold cyan]Username[/]", $"[white]{Markup.Escape(username)}[/]");
        grid.AddRow("[bold cyan]Full Name[/]", $"[white]{Markup.Escape(fullName)}[/]");
        grid.AddRow("[bold cyan]Email[/]", $"[white]{Markup.Escape(email)}[/]");
        grid.AddRow("[bold cyan]Joined On[/]", $"[yellow]{joinDate:dd MMM yyyy}[/]");

        var profilePanel = new Panel(grid)
            .Header($"[bold orange1] {Markup.Escape(username)}'s Profile [/]")
            .Border(BoxBorder.Rounded)
            .BorderStyle(new Style(Color.Cyan1))
            .Padding(2, 1);

        AnsiConsole.Write(profilePanel);
    }

    /// <summary>
    /// Gets the profile edit option from the user.
    /// </summary>
    /// <returns>Returns the option to select.</returns>
    internal EditOption GetProfileEditOption()
    {
        return AnsiConsole.Prompt(new SelectionPrompt<EditOption>()
            .Title("Profile option")
            .AddChoices(Enum.GetValues<EditOption>()));
    }

    /// <summary>
    /// Pauses the console until a key is pressed.
    /// </summary>
    internal void Pause()
    {
        Ui.Pause();
    }

    /// <summary>
    /// Displays the error message.
    /// </summary>
    /// <param name="message">The error message to be displayed.</param>
    internal void DisplayError(string message)
    {
        Ui.Error(message);
        Ui.Pause();
    }

    /// <summary>
    /// Displays the error message.
    /// </summary>
    /// <param name="message">The error message to be displayed.</param>
    internal void DisplayInfo(string message)
    {
        Ui.Info(message);
        Ui.Pause();
    }
}