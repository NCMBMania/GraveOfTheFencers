using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class State_Login : MonoBehaviour
{
    private enum LogInState { LogIn, SignUp, Guest};

    private LogInState currentLogInState;

    private UserAuth userAuth;

    public InputField inputField_ID;
    public InputField inputField_Pass;
    public InputField inputField_Mail;
    public InputField inputField_Name;

    public Canvas canvas_General;
    public Canvas canvas_LogIn;
    public Canvas canvas_SignUp;
    public Canvas canvas_Guest;

    private void Awake()
    {
        userAuth = UserAuth.Instance;

        OnLogInMode();
    }

    public void OnLogInMode()
    {
        currentLogInState = LogInState.LogIn;

        canvas_General.enabled = true;
        canvas_LogIn.enabled = true;
        canvas_SignUp.enabled = false;
        canvas_Guest.enabled = false;

        inputField_Mail.text = string.Empty;
    }

    public void OnSignUpMode()
    {
        currentLogInState = LogInState.SignUp;

        canvas_General.enabled = true;
        canvas_LogIn.enabled = false;
        canvas_SignUp.enabled = true;
        canvas_Guest.enabled = false;
    }

    public void OnGuestMode()
    {
        currentLogInState = LogInState.Guest;

        canvas_General.enabled = false;
        canvas_LogIn.enabled = false;
        canvas_SignUp.enabled = false;
        canvas_Guest.enabled = true;
    }

    public void StartLogInProcess()
    {
        string id = inputField_ID.text;
        string pass = inputField_Pass.text;

        if (!string.IsNullOrEmpty(id) &&
            !string.IsNullOrEmpty(pass))
        {
            userAuth.LogIn(id, pass, StartGame);
        }
        else
        {
            Debug.Log("Log in form is not filled out.");
        }
    }

    public void StartSignUpProcess()
    {
        string id = inputField_ID.text;
        string pass = inputField_Pass.text;
        string mail = inputField_Mail.text;

        if (!string.IsNullOrEmpty(id) &&
            !string.IsNullOrEmpty(pass) &&
            !string.IsNullOrEmpty(mail))
        {
            userAuth.SignUp(id, mail, pass, StartGame);
        }
        else
        {
            Debug.Log("Sign up form is not filled out.");
        }
    }

    public void StartAsGuest()
    {
        string name = inputField_Name.text;
        userAuth.SetGuestName(name,StartGame);
    }
    
    public void StartGame()
    {
        Main.Instance.OnInGame();
    }
}