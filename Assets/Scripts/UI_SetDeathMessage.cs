using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_SetDeathMessage : MonoBehaviour {

    public Text text_Dead;

    public InputField deathMessageField;
    private State_InGame state_InGame;

    void Start()
    {
        state_InGame = GameObject.FindObjectOfType<State_InGame>();
    }

    public void SaveGraveDataWithDamage()
    {
        SaveGraveData(GraveInfo.CurseType.Damage);
    }

    public void SaveGraveDataWithRecover()
    {
        SaveGraveData(GraveInfo.CurseType.Heal);
    }
    public void SaveGraveDataWithNone()
    {
        SaveGraveData(GraveInfo.CurseType.None);
    }

    void SaveGraveData(GraveInfo.CurseType curseType) //Call from contine Button
    {
        string deathMessage = deathMessageField.text;

        if (string.IsNullOrEmpty(deathMessage))
        {
            deathMessage = "何も刻まれていない";
        }

        state_InGame.SaveGraveInfo(deathMessage, curseType);


        deathMessageField.text = string.Empty;
    }

}
