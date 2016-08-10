using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : SingletonMonoBehaviour<UI_InGame>
{
    public List<GraveNameText> graveNameTextList = new List<GraveNameText>();

    public Canvas canvas_Game;
    public Canvas canvas_MobileController;
    public Canvas canvas_Pause;
    public Canvas canvas_DeathMessage;
    public Canvas canvas_Win;

    public Text text_UserNameAndDate;
    public Text text_DeathMessage;
    public Text text_LifePoint;
    // private Canvas thisCanvas;

    public Image image_LifeBar;
    private float defaultLifeBarWidth;

    public Color lifebar_green;
    public Color lifebar_yellow;
    public Color lifebar_red;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        graveNameTextList.ForEach(graveNameText => graveNameText.enabled = false);
    }

    private void Start()
    {
        defaultLifeBarWidth = image_LifeBar.rectTransform.sizeDelta.x;
        //Debug.Log("defaultLifeBarWidth" + defaultLifeBarWidth);
        canvas_Win.enabled = false;

        Resume();
        CloseDeathMessageWindow();
    }

    public void SetLifeBar(int lifePoint)
    {
        text_LifePoint.text = lifePoint.ToString();

        image_LifeBar.rectTransform.localPosition = new Vector3(
            Mathf.InverseLerp(100, 0, lifePoint) * defaultLifeBarWidth,
            image_LifeBar.rectTransform.localPosition.y,
            image_LifeBar.rectTransform.localPosition.z);

        if (lifePoint > 70)
        {
            image_LifeBar.color = lifebar_green;
        }
        else if (lifePoint > 40)
        { 
            image_LifeBar.color = lifebar_yellow;

        }else{
            image_LifeBar.color = lifebar_red;
        }


    }

    public void Pause()
    {
        canvas_Pause.enabled = true;

        graveNameTextList.ForEach(graveNameText => graveNameText.Pause());
    }

    public void Resume()
    {
        canvas_Pause.enabled = false;

        graveNameTextList.ForEach(graveNameText => graveNameText.Resume());
    }

    public void ShowDeathMessageWindow(string deathMessage, string userNameAndDate)
    {
        text_UserNameAndDate.text = userNameAndDate;
        text_DeathMessage.text = deathMessage;

        canvas_Game.enabled = false;
        canvas_MobileController.enabled = false;
        canvas_DeathMessage.enabled = true;

        graveNameTextList.ForEach(graveNameText => graveNameText.Clear());
    }

    public void CloseDeathMessageWindow()
    {
        canvas_Game.enabled = true;
        canvas_MobileController.enabled = true;        
        canvas_DeathMessage.enabled = false;
    }

    public void SetGraveName(Transform textFloatPointTransform, Grave grave)
    {
        if (graveNameTextList.Any(graveNameText => graveNameText.CurrentGraveMessageID == grave.graveInfo.messageId) == false)
        {
            GraveNameText graveNameText = graveNameTextList.FirstOrDefault(t => t.enabled == false);

            if (graveNameText != null)
            {
                graveNameText.SetGraveReference(textFloatPointTransform, grave);
            }
        }
    }

    public void ShowWin()
    {
        canvas_Game.enabled = false;
        canvas_MobileController.enabled = false;
        canvas_Win.enabled = true;
    }

}