using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Repositories;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Services;

/// <summary>
/// Contains business logic for storing the game record.
/// </summary>
internal class TicTacToeService
{
    private readonly RepositorySelector _repositorySelector;

    /// <summary>
    /// Initializes a new instance of the <see cref="TicTacToeService"/> class.
    /// </summary>
    /// <param name="repositorySelector">The instance of repository selector.</param>
    internal TicTacToeService(RepositorySelector repositorySelector)
    {
        this._repositorySelector = repositorySelector;
    }

    /// <summary>
    /// Gets all the game records for the user based on the user session.
    /// </summary>
    /// <param name="session">The user session of the user.</param>
    /// <returns>The list of game record.</returns>
    internal List<TicTacToeGameRecord> GetGameHistory(UserSession session)
    {
        return this._repositorySelector.GetTicTacToeRepository(session)
            .GetByUserName(session.UserName)
            .OrderByDescending(x => x.PlayedAt)
            .Take(100)
            .ToList();
    }

    /// <summary>
    /// Gets the game replay.
    /// </summary>
    /// <param name="session">The game session of the user.</param>
    /// <returns>List of the game record.</returns>
    internal List<TicTacToeGameRecord> GetGameReplay(UserSession session)
    {
        return this._repositorySelector.GetTicTacToeRepository(session)
            .GetByUserName(session.UserName)
            .OrderByDescending(x => x.PlayedAt)
            .Take(10)
            .ToList();
    }

    /// <summary>
    /// Checks if the match is the draw.
    /// </summary>
    /// <param name="moves">The current state of the game.</param>
    /// <returns>True if the match is draw, otherwise false.</returns>
    internal bool IsDraw(Symbol[] moves)
    {
        return !moves.Any(x => x == Symbol.Empty);
    }

    /// <summary>
    /// Toggles the current symbol.
    /// </summary>
    /// <param name="currentSymbol">Current symbol to be toggled.</param>
    /// <returns>Toggled symbol.</returns>
    internal Symbol ToggleSymbol(Symbol currentSymbol)
    {
        if (currentSymbol == Symbol.X)
        {
            return Symbol.O;
        }
        else
        {
            return Symbol.X;
        }
    }

    /// <summary>
    /// Check if there is any winner.
    /// </summary>
    /// <param name="moves">The state of the game.</param>
    /// <returns>The symbol of the winner who won, otherwise <see cref="Symbol.Empty"/>.</returns>
    internal Symbol IsWinCondition(Symbol[] moves)
    {
        // Row
        if (moves[0] == moves[1] && moves[1] == moves[2] && moves[2] != Symbol.Empty)
        {
            return moves[0];
        }

        if (moves[3] == moves[4] && moves[4] == moves[5] && moves[5] != Symbol.Empty)
        {
            return moves[3];
        }

        if (moves[6] == moves[7] && moves[7] == moves[8] && moves[8] != Symbol.Empty)
        {
            return moves[6];
        }

        // Diagonals
        if (moves[0] == moves[4] && moves[8] == moves[4] && moves[8] != Symbol.Empty)
        {
            return moves[0];
        }

        if (moves[2] == moves[4] && moves[4] == moves[6] && moves[6] != Symbol.Empty)
        {
            return moves[2];
        }

        // Column
        if (moves[0] == moves[3] && moves[3] == moves[6] && moves[6] != Symbol.Empty)
        {
            return moves[0];
        }

        if (moves[1] == moves[4] && moves[7] == moves[4] && moves[7] != Symbol.Empty)
        {
            return moves[1];
        }

        if (moves[2] == moves[5] && moves[8] == moves[5] && moves[8] != Symbol.Empty)
        {
            return moves[2];
        }

        return Symbol.Empty;
    }

    /// <summary>
    /// Creates a new game record based on the game mode, timer option, and user session.
    /// </summary>
    /// <param name="mode">The game mode selected.</param>
    /// <param name="timer">The timer countdown for each move</param>
    /// <param name="session">The user session creating the game.</param>
    /// <returns>The game instance created.</returns>
    internal TicTacToeGameRecord CreateGame(GameMode mode, TimerOption timer, UserSession session)
    {
        TicTacToeGameRecord game = new TicTacToeGameRecord()
        {
            PlayerOneName = session.UserName,
            Mode = mode,
            PlayedAt = DateTime.UtcNow,
            TimeLimitSeconds = timer,
        };

        if (session.IsStart)
        {
            game.PlayerOneSymbol = Symbol.X;
            game.PlayerTwoSymbol = Symbol.O;
            session.IsStart = false;
        }
        else
        {
            game.PlayerOneSymbol = Symbol.O;
            game.PlayerTwoSymbol = Symbol.X;
            session.IsStart = true;
        }

        return game;
    }

    /// <summary>
    /// Records a single move into the game's move history.
    /// </summary>
    /// <param name="game">The game instance created.</param>
    /// <param name="symbol">The symbol of the move.</param>
    /// <param name="position">The position of the move.</param>
    internal void AddMove(TicTacToeGameRecord game, Symbol symbol, int position)
    {
        game.Moves.Add(new MoveRecord
        {
            Cell = position,
            Symbol = symbol,
            Timestamp = DateTime.UtcNow,
        });
    }

    /// <summary>
    /// Finalizes the game record outcome and saves it for both players' sessions.
    /// </summary>
    /// <param name="game">The game instance created.</param>
    /// <param name="endReason">The end reason of the game.</param>
    /// <param name="winnerName">The username of the winner.</param>
    /// <param name="playerOneSession">The user session of player one.</param>
    /// <param name="playerTwoSession">The user session of player two, if available.</param>
    internal void CompleteAndSaveGame(TicTacToeGameRecord game, EndReason endReason, string? winnerName, UserSession playerOneSession, UserSession? playerTwoSession = null)
    {
        game.EndReason = endReason;
        game.WinnerName = winnerName ?? string.Empty;

        this.AddGameRecord(playerOneSession, game);

        if (playerTwoSession is not null)
        {
            IGameRepository<TicTacToeGameRecord> playerOneRepo = this._repositorySelector.GetTicTacToeRepository(playerOneSession);
            IGameRepository<TicTacToeGameRecord> playerTwoRepo = this._repositorySelector.GetTicTacToeRepository(playerTwoSession);

            if (!ReferenceEquals(playerOneRepo, playerTwoRepo))
            {
                playerTwoRepo.AddGameRecord(game);
            }
        }
    }

    /// <summary>
    /// Clears the guest data for the given user session.
    /// </summary>
    /// <param name="session">The user session to create.</param>
    internal void ClearGuestData(UserSession session)
    {
        this._repositorySelector.ClearGuest(session);
    }

    /// <summary>
    /// Adds the game record to the repository based on the user session.
    /// </summary>
    /// <param name="session">The user session of the user.</param>
    /// <param name="gameRecord">The game record to add.</param>
    private void AddGameRecord(UserSession session, TicTacToeGameRecord gameRecord)
    {
        this._repositorySelector.GetTicTacToeRepository(session).AddGameRecord(gameRecord);
    }
}
