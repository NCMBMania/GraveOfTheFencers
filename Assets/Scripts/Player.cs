using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof (NavMeshAgent))]
public class Player : GameActorBase, IGameActor
{
    private UI_InGame ui_InGame;

    public int lifePoint = 100;
    public int currentLifePoint = 0;

    public float moveSpeed = 10f;
    public float rotateSpeed = 5f;

    public ParticleSystem particle_Heal;
    public ParticleSystem particle_Damage;

    private enum State { Wait, Walk, Dead, Attack }

    [SerializeField]
    private State currentState = State.Wait;
    public bool isAttackAnimation = false;

    public override void Awake()
    {
        base.Awake();

        ui_InGame = UI_InGame.Instance;
    }

    public override void OnAttackHit(Collider col)
    {
        if (col.tag == "Enemy")
        {
            AddLifePoint(-40);
        }
    }

    public override void Start()
    {
        base.Start();
        AddLifePoint(lifePoint);
    }

    public void AddLifePoint(int point)
    {
        currentLifePoint += point;

        if (currentLifePoint > 100)
        {
            currentLifePoint = 100;
        }
        else if (currentLifePoint <= 0)
        {
            state_InGame.PlayerDead(this.gameObject.transform.localPosition);
        }

        ui_InGame.SetLifeBar(currentLifePoint);
    }

    public void OnPlayerDead()
    {
        currentState = State.Dead;
        this.gameObject.SetActive(false);
    }

    public void OnWalk()
    {
        currentState = State.Walk;
    }

    public void OnWait()
    {
        currentState = State.Wait;
        animator.SetFloat("Walk", 0f);
        animator.SetTrigger("Idle");
    }

    public void UpdateWalk()
    {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

        //移動//
        Vector3 moveDirection = h * Vector3.right + v * Vector3.forward;
        moveDirection = moveSpeed * moveDirection.normalized;
        navMeshAgent.velocity = moveDirection;

        //回転//
        if (moveDirection.sqrMagnitude > 0.001)
        {
            float step = rotateSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, moveDirection, step, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        //アニメーション更新//
        float walkAmount = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));
        animator.SetFloat("Walk", walkAmount);

        //攻撃ボタン判定//
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            OnAttack();
        }
    }

    private void OnAttack()
    {
        currentState = State.Attack;
        animator.SetTrigger("Attack");
    }

    public override void RecieveAnimationState(string name, bool enabled)
    {
        if (name == "Attack")
        {
            isAttackAnimation = enabled;
        }
    }

    private void UpdateAttack()
    {
        if (isAttackAnimation == false)
        {
            OnWalk();
        }
    }

    public void Update()
    {
        if (IsPaused)
        {
            return;
        }

        switch (currentState)
        {
            case State.Walk:
                UpdateWalk();
                break;
            case State.Attack:
                UpdateAttack();
                break;

            default:
                break;
        }
    }

    public void ShowHealEffect()
    {
        particle_Heal.Play();
    }

    public void ShowDamageEffect()
    {
        particle_Damage.Play();
    }
}