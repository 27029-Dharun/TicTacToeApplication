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
        this._snakeWaterGunView.DisplayGameInstructions();

        int bestOf = this._snakeWaterGunView.GetBestOfChoice();

        while (true)
        {
            int userScore = 0;
            int computerScore = 0;

            while (userScore + computerScore < bestOf)
            {
                SnakeWaterGunChoice userChoice = this.GetUserChoice();
                SnakeWaterGunChoice computerChoice = this._snakeWaterGunService.GetComputerChoice();
                this._snakeWaterGunView.DisplayChoices(userChoice, computerChoice);
                int result = this._snakeWaterGunService.DetermineWinner(userChoice, computerChoice);

                if (result == SnakeWaterGunService.UserWins)
                {
                    userScore++;
                }
                else
                {
                    computerScore++;
                }
            }

            this._snakeWaterGunView.DisplayRoundResult(userScore, computerScore);

            if (!this._snakeWaterGunView.AskPlayAgain())
            {
                break;
            }
        }
    }

    private SnakeWaterGunChoice GetUserChoice()
    {
        return this._snakeWaterGunView.GetChoice();
    }
}
