namespace TicTacToe.Models;

internal class GameStats
{
    public int Wins { get; set; }

    public int Losses { get; set; }

    public int Points { get; set; }

    public int GamePlayed => this.Wins + this.Losses;
}
