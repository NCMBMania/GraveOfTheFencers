using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(Button))]
public class GraveNameText : MonoBehaviour
{
    private Transform textShowPointTransform;
    public float lifeTime = 3f;
    private float timer = 0f;
    private bool isPaused = false;

    private string deathMessage;
    public Grave currentGrave;

    private Button thisButton;
    private Text thisText;

    private State_InGame state_InGame;

    private void Awake()
    {
        thisText = GetComponent<Text>();
        thisText.enabled = false;

        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(ShowThisGraveMessage);

        state_InGame = State_InGame.Instance;
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(thisText.text) && !isPaused)
        {
            UpdateTextPosition();
            timer += Time.deltaTime;
            if (timer > lifeTime)
            {
                Clear();
            }
        }
    }

    public string CurrentGraveMessageID {
        get {
            if (currentGrave != null) {
                return currentGrave.graveInfo.messageId;
            }
            else
            {
                return string.Empty;
            }
        }
    }

    private void ShowThisGraveMessage()
    {
        state_InGame.ShowDeathMessage(currentGrave);
    }

    public void SetGraveReference(Transform gravePointTransform, Grave grave)
    {
        textShowPointTransform = gravePointTransform;
        currentGrave = grave;

        thisText.enabled = true;
        thisText.text = currentGrave.graveInfo.userName;

        UpdateTextPosition();

        this.enabled = true;
        thisButton.enabled = true;
    }

    public void Pause()
    {
        isPaused = true;
        thisButton.enabled = false;
    }

    public void Resume()
    {
        isPaused = false;
        thisButton.enabled = false;
    }

    public void Clear()
    {
        timer = 0f;

        currentGrave = null;//nullにしないと参照が残り同じIDのメッセージが表示されなくなる//
        thisText.text = string.Empty;
        thisText.enabled = false; //空でも他のテキストが重なるとEventが取れなくなるので非表示にする//
        thisButton.enabled = false;
        this.enabled = false;
    }

    private void UpdateTextPosition()
    {
        if (textShowPointTransform != null)
        {
            thisText.rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, textShowPointTransform.position);
        }
    }
}