using TicTacToe.Models.Enums;

namespace TicTacToe.Services;

/// <summary>
/// Contains the game logics.
/// </summary>
internal class SnakeWaterGunService
{
    /// <summary>
    /// Represents the output for the win.
    /// </summary>
    public const int UserWins = 1;

    /// <summary>
    /// Represents the output for the computer win.
    /// </summary>
    public const int ComputerWins = -1;

    private readonly Random _random = new Random();

    /// <summary>
    /// Determines the winners in the game.
    /// </summary>
    /// <param name="userChoice">The choice selected by the user.</param>
    /// <param name="computerChoice">The choice selected by the computer.</param>
    /// <returns>The winner.</returns>
    public int DetermineWinner(SnakeWaterGunChoice userChoice, SnakeWaterGunChoice computerChoice)
    {
        if (userChoice == computerChoice)
        {
            return 0;
        }

        if ((userChoice.Equals(SnakeWaterGunChoice.Snake) && computerChoice.Equals(SnakeWaterGunChoice.Water)) ||
           (userChoice.Equals(SnakeWaterGunChoice.Water) && computerChoice.Equals(SnakeWaterGunChoice.Gun)) ||
           (userChoice.Equals(SnakeWaterGunChoice.Gun) && computerChoice.Equals(SnakeWaterGunChoice.Snake)))
        {
            return UserWins;
        }

        return ComputerWins;
    }

    /// <summary>
    /// Gets the computer choice randomly.
    /// </summary>
    /// <returns>The choice chosen by the computer.</returns>
    public SnakeWaterGunChoice GetComputerChoice()
    {
        int choice = this._random.Next(1, 4);
        return (SnakeWaterGunChoice)choice;
    }
}
