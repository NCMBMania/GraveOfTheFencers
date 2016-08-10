using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class State_InGame : SingletonMonoBehaviour<State_InGame>
{
    public enum InGameState { Default, Pause, ShowMessage, PlayerDead, PlayerWin }

    [SerializeField]
    private InGameState currentInGameState = InGameState.Default;

    public UI_InGame ui_InGame;
    public GameObject ui_SetDeathMessage;
    public GameObject ui_InGameObject;

    public List<Enemy> EnemyList = new List<Enemy>();
    private Player player;
    private Grave currentGrave;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        ui_SetDeathMessage.SetActive(false);
        ui_InGameObject.SetActive(true);

        EnemyList.ForEach(list => list.Initialize());
        player = Player.Instance;

        DataStoreManager.Instance.FetchGraveData(StartGame);
    }

    void Update()
    {
        switch (currentInGameState)
        {
            case InGameState.Default:
                
                if (!EnemyList.Any(enemy => enemy.IsAlive))
                {
                    PlayerWin();
                }
                break;
            case InGameState.Pause:
                break;
            case InGameState.ShowMessage:
                break;
            case InGameState.PlayerDead:
                break;
            case InGameState.PlayerWin:
                break;
            default:
                break;
        }
    }

    private void StartGame()
    {
        currentInGameState = InGameState.Default;

        Main.Instance.DisableUI_Connecting();
        SoundManager.PlayBGM("Dysipe_1_loop");
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

        currentInGameState = InGameState.Default;
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
        switch (currentGrave.graveInfo.curseType)
        {
            case GraveInfo.CurseType.None:
                break;

            case GraveInfo.CurseType.Damage:
                player.AddLifePoint(-30);
                player.ShowDamageEffect();
                SoundManager.PlaySE("Damage");
                break;

            case GraveInfo.CurseType.Heal:

                player.AddLifePoint(30);
                player.ShowHealEffect();
                SoundManager.PlaySE("Heal");

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

        currentInGameState = InGameState.Default;
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
        player.OnPause();
        currentInGameState = InGameState.PlayerWin;
        ui_InGame.ShowWin();
    }

    public void Replay()
    {
        Main.Instance.OnInGame();
    }
}