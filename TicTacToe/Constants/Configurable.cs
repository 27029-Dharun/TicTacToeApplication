using TicTacToe.Models.Enums;

namespace TicTacToe.Constants;

internal static class Configurable
{
    public const int PointForTicTacToeDraw = 1;

    public const int PointForSnakeWaterGunWin = 1;

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
