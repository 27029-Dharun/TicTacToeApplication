using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Services;
using TicTacToe.Views;

namespace TicTacToe.Controllers;

/// <summary>
/// Controls the authentication flow in the application.
/// </summary>
internal class AuthController
{
    private readonly AuthService _authService;
    private readonly AuthView _authView;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authService">Instance of the authentication service.</param>
    /// <param name="authView">Instance of the authentication view.</param>
    public AuthController(AuthService authService, AuthView authView)
    {
        this._authService = authService;
        this._authView = authView;
    }

    /// <summary>
    /// Performs authentication.
    /// </summary>
    /// <returns>The user session created.</returns>
    public UserSession? HandleAuth()
    {
        while (true)
        {
            try
            {
                WelcomeChoice choice = this._authView.ShowWelcomeMenu();
                switch (choice)
                {
                    case WelcomeChoice.Login:
                        UserSession? session = this.HandleLogIn();

                        if (session != null)
                        {
                            return session;
                        }

                        this._authView.DisplayError("Please enter a valid username and password");
                        break;

                    case WelcomeChoice.SignUp:
                        this.HandleSignUp();
                        break;

                    case WelcomeChoice.Guest:
                        return this._authService.LogInGuest();

                    case WelcomeChoice.Exit:
                        return null;
                }

                this._authView.Pause();
            }
            catch (Exception e)
            {
                this._authView.DisplayError(e.Message);
            }
        }
    }

    /// <summary>
    /// Get the credentials of player 2.
    /// </summary>
    /// <returns>The user session of the player 2</returns>
    internal UserSession? GetPlayer2()
    {
        while (true)
        {
            try
            {
                WelcomeChoice choice = this._authView.ShowPlayer2Menu();
                switch (choice)
                {
                    case WelcomeChoice.Login:
                        UserSession? session = this.HandleLogIn();

                        if (session != null)
                        {
                            return session;
                        }

                        this._authView.DisplayError("Please enter a valid username and password");
                        break;

                    case WelcomeChoice.Guest:
                        return this._authService.LogInGuest();

                    case WelcomeChoice.Exit:
                        return null;
                }

                this._authView.Pause();
            }
            catch (Exception e)
            {
                this._authView.DisplayError(e.Message);
            }
        }
    }

    private UserSession? HandleLogIn()
    {
        this._authView.DisplayTitle("LOGIN PAGE");

        string username = this._authView.GetUserName();
        string password = this._authView.GetPassword();

        return this._authService.LogIn(username, password);
    }

    private void HandleSignUp()
    {
        this._authView.DisplayTitle("SIGN UP PAGE");

        string username = this._authView.GetUserName();
        string name = this._authView.GetName();
        string email = this._authView.GetEmail();
        string password = this._authView.GetPassword();

        var (success, message) = this._authService.CreateUser(username, name, password, email);

        if (success)
        {
            this._authView.DisplaySuccess(message);
        }
        else
        {
            this._authView.DisplayError(message);
        }
    }
}
