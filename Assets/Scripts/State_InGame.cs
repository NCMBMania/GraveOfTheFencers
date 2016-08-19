using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TSM;

public class State_InGame : SingletonMonoBehaviour<State_InGame>
{
    public enum InGameState { Game, Pause, ShowMessage, PlayerDead, PlayerWin, Loading }

    [SerializeField]
    private InGameState currentInGameState = InGameState.Game;

    public UI_InGame ui_InGame;
    public GameObject ui_SetDeathMessage;
    public GameObject ui_InGameObject;

    public List<IGameActor> GameActorList = new List<IGameActor>();
    public List<Enemy> EnemyList = new List<Enemy>();

    public Player player;
    private Grave currentGrave;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
    }

    void OnValidate()
    {
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        ui_SetDeathMessage.SetActive(false);
        ui_InGameObject.SetActive(true);

        Restart();
    }

    public void Restart()
    {
        currentInGameState = InGameState.Loading;
        SoundManager.Instance.StopBGM();
        DataStoreManager.Instance.FetchGraveData(StartGame);
    }

    private void StartGame()
    {
        currentInGameState = InGameState.Game;

        player.OnWalk();

        Main.Instance.DisableUI_Connecting();
        SoundManager.Instance.PlayBGM("Dysipe_1_loop");
    }


    void Update()
    {
        switch (currentInGameState)
        {
            case InGameState.Game:
                
                if (allEnemyDead)
                {
                    PlayerWin();
                }
                break;
            default:
                break;
        }
    }

    bool allEnemyDead
    {
        get {
            return !EnemyList.Any(enemy => enemy.IsAlive);
        }
    }


    public void SaveGraveInfo(string deathMessage, GraveInfo.CurseType curseType)
    {
        string userName = UserAuth.Instance.CurrentPlayerName();
        Vector3 deathPosition = player.gameObject.transform.position;
        DataStoreManager.Instance.SaveGraveInfo(userName, deathMessage, curseType, deathPosition);
    }

    public void Pause()
    {
        ui_InGame.Pause();
        player.OnPause();
        EnemyList.ForEach(list => list.OnPause());

        currentInGameState = InGameState.Pause;
    }

    public void Resume()
    {
        ui_InGame.Resume();
        player.OnResume();
        EnemyList.ForEach(list => list.OnResume());

        currentInGameState = InGameState.Game;
    }

    public void ShowDeathMessage(Grave grave)
    {
        currentGrave = grave;

        string userNameAndDate = "-  " + currentGrave.graveInfo.userName;
        ui_InGame.ShowDeathMessageWindow(currentGrave.graveInfo.deathMessage, userNameAndDate);
        player.OnPause();
        EnemyList.ForEach(list => list.OnPause());

        currentInGameState = InGameState.ShowMessage;
    }

    public void CloseDeathMessageWithPray()
    {
        if(currentGrave == null)
        {

        }

        switch (currentGrave.graveInfo.curseType)
        {
            case GraveInfo.CurseType.None:
                break;

            case GraveInfo.CurseType.Damage:
                player.AddLifePoint(-30);
                player.ShowDamageEffect();
                SoundManager.Instance.PlaySE("Damage");
                break;

            case GraveInfo.CurseType.Heal:

                player.AddLifePoint(30);
                player.ShowHealEffect();
                SoundManager.Instance.PlaySE("Heal");

                break;

            default:
                break;
        }

        currentGrave.graveInfo.isUsed = true;
        CloseDeathMessage();
    }

    public void CloseDeathMessage()
    {
        ui_InGame.CloseDeathMessageWindow();
        player.OnResume();
        EnemyList.ForEach(list => list.OnResume());

        currentInGameState = InGameState.Game;
    }

    public void PlayerDead(Vector3 deathPosition)
    {
        currentInGameState = InGameState.PlayerDead;

        ui_SetDeathMessage.SetActive(true);
        ui_InGameObject.SetActive(false);

        player.OnPlayerDead();
        EnemyList.ForEach(list => list.OnPlayerDead());

        deathPosition.y = 0;
        GraveObjectsManager.Instance.InstallationTempGrave(deathPosition);
    }

    public void BoardMessageInput()
    {
        ui_SetDeathMessage.SetActive(false);
    }

    public void PlayerWin()
    {
        player.OnWait();
        currentInGameState = InGameState.PlayerWin;
        ui_InGame.ShowWin();
    }

    public void Replay()
    {
        Main.Instance.OnInGame();
    }
}