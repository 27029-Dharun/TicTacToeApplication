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
    private readonly ScoreService _scoreService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnakeWaterGunController"/> class.
    /// </summary>
    /// <param name="snakeWaterGunView">The instance of snake water gun view.</param>
    /// <param name="snakeWaterGunService">The instance of the snake water gun service.</param>
    /// <param name="scoreService">The instance of the score service.</param>
    public SnakeWaterGunController(SnakeWaterGunView snakeWaterGunView, SnakeWaterGunService snakeWaterGunService, ScoreService scoreService)
    {
        this._snakeWaterGunView = snakeWaterGunView;
        this._snakeWaterGunService = snakeWaterGunService;
        this._scoreService = scoreService;
    }

    /// <summary>
    /// Runs the Snake Water Gun game by displaying instructions and starting the game loop.
    /// </summary>
    /// <param name="session">The user session created.</param>
    public void RunSnakeWaterGunGame(UserSession session)
    {
        while (true)
        {
            SnakeWaterGunMenuOption option = this._snakeWaterGunView.GetMenuOption();

            switch (option)
            {
                case SnakeWaterGunMenuOption.NewGame:
                    this.StartGame(session);
                    break;

                case SnakeWaterGunMenuOption.History:
                    this.DisplayHistory(session);
                    break;

                case SnakeWaterGunMenuOption.Leaderboard:
                    if (session.IsGuest)
                    {
                        this._snakeWaterGunView.DisplayInfo("Login as user to the see the leaderboard\n");
                        break;
                    }

                    this.DisplayLeaderboard(session);
                    break;

                case SnakeWaterGunMenuOption.Profile:
                    this.DisplayProfile(session);
                    break;

                case SnakeWaterGunMenuOption.Back:
                    return;
            }

            Ui.Pause();
        }
    }

    private void DisplayProfile(UserSession session)
    {
        Score scores = this._scoreService.GetScoreByUserName(session);
        this._snakeWaterGunView.DisplayProfile(scores);
    }

    private void DisplayLeaderboard(UserSession session)
    {
        List<(string Username, int TotalPoints, int Wins)> scores = this._scoreService.GetSnakeWaterGunLeaderBoard(session);
        this._snakeWaterGunView.DisplayLeaderBoard(session.UserName, scores);
    }

    private void DisplayHistory(UserSession session)
    {
        List<GameRecord> history = this._snakeWaterGunService.GetGameHistory(session);
        this._snakeWaterGunView.DisplayHistory(history, session);
    }

    private void StartGame(UserSession session)
    {
        while (true)
        {
            this._snakeWaterGunView.DisplayGameInstructions();
            int bestOf = (int)this._snakeWaterGunView.GetBestOfChoice();

            if (bestOf == 0)
            {
                return;
            }

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
                this._snakeWaterGunView.DisplayChoices(userChoice, computerChoice, result);

                round++;
            }

            this._scoreService.RecordSnakeWaterGunWin(session);
            this._snakeWaterGunService.SaveGame(session, bestOf, userScore, computerScore);
            this._snakeWaterGunView.DisplayMatchResult(userScore, computerScore);

            if (!this._snakeWaterGunView.AskPlayAgain())
            {
                break;
            }
        }
    }
}
