using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using TSM;

public class Main : SingletonMonoBehaviour<Main>
{
    public enum State { Login, InGame }

    [SerializeField]
    private State currentState;

    public Canvas canvas_Connecting;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {

#if UNITY_EDITOR

        if (SceneManager.GetSceneByName("Login").IsValid())
        {
            currentState = State.Login;
            DisableUI_Connecting();
        }
        else if(SceneManager.GetSceneByName("InGame").IsValid())
        {
            currentState = State.InGame;
            EnableUI_Connecting();
        }
        else
        {
            OnLogin();
        }

#else
        OnLogin();
#endif
    }

    public void OnLogin()
    {
        currentState = State.Login;
        DisableUI_Connecting();

        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }

    public void OnInGame()
    {
        currentState = State.InGame;
        EnableUI_Connecting();
        SoundManager.Instance.ClearAudioListenerPos();
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }

    public void EnableUI_Connecting()
    {
        canvas_Connecting.enabled = true;
    }

    public void DisableUI_Connecting()
    {
        canvas_Connecting.enabled = false;
    }
}