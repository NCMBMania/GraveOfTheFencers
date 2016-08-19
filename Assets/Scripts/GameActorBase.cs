using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player, Enemy, NPC Base
/// </summary>

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class GameActorBase : MonoBehaviour, IGameActor
{
    protected State_InGame state_InGame;
    protected Animator animator;
    protected NavMeshAgent navMeshAgent;
    protected AudioSource audioSource;
    public List<AudioClip> audioClips;

    public OnTriggerEnterDetector hitArea;
    public Collider attackArea;

    public bool IsPaused = false;

    public virtual void Awake()
    {
        hitArea.callBack = OnAttackHit;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;

        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public virtual void Start()
    {
        //GameActorManagerに切り離す//
        state_InGame = State_InGame.Instance;
        state_InGame.GameActorList.Add(this);

        attackArea.enabled = false;
        hitArea.enabled = true;
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
        audioSource.clip = audioClips.Find(clip => clip.name == seName);
        audioSource.Play();
    }

    public virtual void RecieveAnimationState(string name, bool enabled)
    {
    }

    public virtual void OnAttackHit(Collider col)
    {

    }

    public virtual void OnDead()
    {
        hitArea.enabled = false;
        attackArea.enabled = false;

    }

    public virtual void OnPause()
    {
        IsPaused = true;
        animator.speed = 0f;
        hitArea.enabled = false;
        audioSource.Pause();

        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.Stop();
        }
    }


    public virtual void OnResume()
    {
        IsPaused = false;
        animator.speed = 1f;
        hitArea.enabled = true;
        audioSource.UnPause();

        if (navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.Resume();
        }
    }
}