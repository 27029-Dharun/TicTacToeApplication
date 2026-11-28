using TicTacToe.Models.Enums;

namespace TicTacToe.Constants;

/// <summary>
/// Contains the configurable constants.
/// </summary>
internal static class Configurable
{
    /// <summary>
    /// Points awarded for tic tac toe draw.
    /// </summary>
    public const int PointForTicTacToeDraw = 1;

    /// <summary>
    /// Points awarded for the snake water gun win.
    /// </summary>
    public const int PointForSnakeWaterGunWin = 1;

    /// <summary>
    /// Points awarded for each game mode.
    /// </summary>
    /// <param name="mode">The game mode selected</param>
    /// <returns>The points for the respective game mode.</returns>
    public static int GetPointsForMode(GameMode mode)
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
