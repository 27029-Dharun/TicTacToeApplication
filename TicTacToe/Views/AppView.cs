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
        return AnsiConsole.Prompt(new SelectionPrompt<GameOption>()
            .Title("[bold yellow]Select the game:[/]")
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
}