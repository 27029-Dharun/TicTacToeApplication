namespace TicTacToe.Models;

internal class SnakeWaterGunGame
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string GameResult { get; set; }

    public int UserScore { get; set; }

    public int ComputerScore { get; set; }

    public DateTime PlayedAt { get; set; }
}
