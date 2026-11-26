using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Services;
using TicTacToe.Views;

namespace TicTacToe.Controllers;

/// <summary>
/// Contains the logic for controlling the Snake Water Gun game.
/// </summary>
internal class SnakeWaterGunController
{
    private readonly SnakeWaterGunView _snakeWaterGunView;
    private readonly SnakeWaterGunService _snakeWaterGunService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnakeWaterGunController"/> class.
    /// </summary>
    /// <param name="snakeWaterGunView">The instance of snake water gun view.</param>
    /// <param name="snakeWaterGunService">The instance of the snake water gun service.</param>
    public SnakeWaterGunController(SnakeWaterGunView snakeWaterGunView, SnakeWaterGunService snakeWaterGunService)
    {
        this._snakeWaterGunView = snakeWaterGunView;
        this._snakeWaterGunService = snakeWaterGunService;
    }

    /// <summary>
    /// Runs the Snake Water Gun game by displaying instructions and starting the game loop.
    /// </summary>
    /// <param name="session">The user session created.</param>
    public void RunSnakeWaterGunGame(UserSession session)
    {
        while (true)
        {
            this._snakeWaterGunView.DisplayGameInstructions();
            int bestOf = this._snakeWaterGunView.GetBestOfChoice();
            int winsNeeded = (bestOf / 2) + 1;

            int userScore = 0;
            int computerScore = 0;
            int round = 1;

            while (userScore < winsNeeded && computerScore < winsNeeded)
            {
                this._snakeWaterGunView.DisplayScoreHeader(session.UserName, userScore, computerScore, round, bestOf);

                SnakeWaterGunChoice userChoice = this._snakeWaterGunView.GetChoice();
                SnakeWaterGunChoice computerChoice = this._snakeWaterGunService.GetComputerChoice();

                int result = this._snakeWaterGunService.DetermineWinner(userChoice, computerChoice);

                if (result == SnakeWaterGunService.UserWins)
                {
                    userScore++;
                }
                else if (result == SnakeWaterGunService.ComputerWins)
                {
                    computerScore++;
                }

                this._snakeWaterGunView.DisplayScoreHeader(session.UserName, userScore, computerScore, round, bestOf);
                this._snakeWaterGunView.DisplayChoices(userChoice, computerChoice);

                round++;
            }

            this._snakeWaterGunView.DisplayMatchResult(userScore, computerScore);

            if (!this._snakeWaterGunView.AskPlayAgain())
            {
                break;
            }
        }
    }
}
