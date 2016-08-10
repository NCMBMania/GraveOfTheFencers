using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class State_Login : MonoBehaviour
{
    private enum LogInState { LogIn, SignUp};

    private LogInState currentLogInState;

    private UserAuth userAuth;

    public InputField inputFieldID;
    public InputField inputFieldPass;
    public InputField inputFieldMail;

    public Canvas canvasLogIn;
    public Canvas canvasSignUp;

    private void Awake()
    {
        userAuth = UserAuth.Instance;

        OnLogInMode();
    }

    public void OnLogInMode()
    {
        currentLogInState = LogInState.LogIn;

        canvasLogIn.enabled = true;
        canvasSignUp.enabled = false;

        inputFieldMail.text = string.Empty;
    }

    public void OnSignUpMode()
    {
        currentLogInState = LogInState.SignUp;

        canvasLogIn.enabled = false;
        canvasSignUp.enabled = true;
    }

    public void StartLogInProcess()
    {
        string id = inputFieldID.text;
        string pass = inputFieldPass.text;

        if (!string.IsNullOrEmpty(id) &&
            !string.IsNullOrEmpty(pass))
        {
            userAuth.logIn(id, pass, StartGame);
        }
        else
        {
            Debug.Log("Log in form is not filled out.");
        }
    }

    public void StartSignUpProcess()
    {
        string id = inputFieldID.text;
        string pass = inputFieldPass.text;
        string mail = inputFieldMail.text;

        if (!string.IsNullOrEmpty(id) &&
            !string.IsNullOrEmpty(pass) &&
            !string.IsNullOrEmpty(mail))
        {
            userAuth.signUp(id, mail, pass, StartGame);
        }
        else
        {
            Debug.Log("Sign up form is not filled out.");
        }
    }

    public void StartGame()
    {
        Main.Instance.OnInGame();
    }
}