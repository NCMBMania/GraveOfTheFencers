using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(CharacterController))]
public class Player : SingletonMonoBehaviour<Player>, IGameActor
{
    private Vector3 originPos;
    private CharacterController characterController;
    public Animator playerAnimator;

    public int currentLifePoint = 0;
    private const int DEFAULT_LIFEPOINT = 100;
    private State_InGame state_InGame;

    public float moveSpeed = 10f;
    public float rotateSpeed = 5f;

    public OnTriggerEnterDetector hitArea;

    public ParticleSystem particle_Heal;
    public ParticleSystem particle_Damage;

    private enum State { Walk, Dead, Attack }

    [SerializeField]
    private State currentState = State.Walk;

    private bool isPaused = false;
    public bool isAttacking = false;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        characterController = GetComponent<CharacterController>();
        hitArea.callBack = AttackHit;
    }

    private void AttackHit(Collider col)
    {
        if (col.tag == "Enemy")
        {
            AddLifePoint(-40);
        }
    }

    private void Start()
    {
        state_InGame = State_InGame.Instance;
        originPos = this.gameObject.transform.position;

        Initialize();
    }

    public void Initialize()
    {
        currentLifePoint = DEFAULT_LIFEPOINT;
        UI_InGame.Instance.SetLifeBar(currentLifePoint);

        this.gameObject.SetActive(true);
        this.gameObject.transform.position = originPos;
    }

    public void OnPause()
    {
        playerAnimator.speed = 0f;
        hitArea.enabled = false;
        isPaused = true;
    }

    public void OnResume()
    {
        playerAnimator.speed = 1f;
        hitArea.enabled = true;

        isPaused = false;
    }

    public void AddLifePoint(int point)
    {
        currentLifePoint += point;

        if (currentLifePoint > 100) currentLifePoint = 100;

        if (currentLifePoint <= 0)
        {
            state_InGame.PlayerDead(this.gameObject.transform.localPosition);
        }

        UI_InGame.Instance.SetLifeBar(currentLifePoint);
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

    public void UpdateWalk()
    {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");


        Vector3 moveDirection = h * Vector3.right + v * Vector3.forward;
        moveDirection = moveSpeed * moveDirection.normalized;

        characterController.SimpleMove(moveDirection);

        float walkAmount = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v));
        playerAnimator.SetFloat("Walk", walkAmount);

        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            OnAttack();
        }

        moveDirection.y = 0;

        if (moveDirection.sqrMagnitude > 0.001)
        {
            float step = rotateSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, moveDirection, step, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    private void OnAttack()
    {
        currentState = State.Attack;
        playerAnimator.SetTrigger("Attack");
    }

    public void RecieveAnimationState(string name, bool enabled)
    {
        if (name == "Attack")
        {
            isAttacking = enabled;
        }
    }

    private void UpdateAttack()
    {
        if (isAttacking == false)
        {
            OnWalk();
        }
    }

    private void Update()
    {
        if (isPaused) return;

        switch (currentState)
        {
            case State.Walk:
                UpdateWalk();
                break;

            case State.Dead:
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