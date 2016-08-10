using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
public class AnimatorEventReceiver : MonoBehaviour
{
    public Collider attackArea;
    public AudioSource audioSource;

    void Awake()
    {
        attackArea.enabled = false;
    }

    public void EnableAttackArea()
    {
        attackArea.enabled = true;
    }

    public void DisableAttackArea()
    {
        attackArea.enabled = false;
    }

    public void PlaySE3D(string seName)
    {
        audioSource.clip = SoundManager.GetSEAudioClipFromLoadedList(seName);
        audioSource.Play();
        //audioSource.PlayOneShot(SoundManager.GetSEAudioClipFromLoadedList(seName), 0.3f);
    }
}
