using UnityEngine;

public class Enemy : MonoBehaviour, IGameActor
{
    public GameObject target;
    public Animator thisAnimator;
    public NavMeshAgent navMeshAgent;

    private enum EnemyState { Roaming, FollowPlayer, Attack, Dead, Disable }

    [SerializeField]
    private EnemyState currentEnemyState = EnemyState.Roaming;

    public bool IsPaused = false;

    private float playerFindDistance = 5f;
    private float playerAttackDistance = 1.3f;
    private float defaultMoveSpeed;

    public Collider roamingArea;
    private Vector3 defaultPosition;
    private float attackInterval = 2f;
    private float attackTimer = 0f;

    private State_InGame state_InGame;

    public OnTriggerEnterDetector hitArea;
    public Collider attackArea;

    public void RecieveAnimationState(string name, bool enabled)
    {

    }

    private void Awake()
    {
        defaultPosition = this.gameObject.transform.position;

        defaultMoveSpeed = navMeshAgent.speed;

        hitArea.callBack = AttackHit;
    }

    private void AttackHit(Collider col)
    {
        if (col.tag == "Player")
        {
            OnDead();
        }
    }

    private void Start()
    {
        state_InGame = State_InGame.Instance;
        state_InGame.EnemyList.Add(this);
        //state_InGame.EnemyList.Add(this);
        hitArea.enabled = true;
    }

    private void Update()
    {
        if (IsPaused)
        {
            return;
        }

        switch (currentEnemyState)
        {
            case EnemyState.Roaming:
                UpdateRoaming();
                UpdateWalkAnimation();
                break;

            case EnemyState.FollowPlayer:
                UpdateFollowPlayer();
                UpdateWalkAnimation();
                break;

            case EnemyState.Attack:
                UpdateAttack();

                break;

            case EnemyState.Dead:
                break;

            case EnemyState.Disable:
                break;

            default:
                break;
        }
    }

    private void OnAttack()
    {
        thisAnimator.SetTrigger("Attack");
        currentEnemyState = EnemyState.Attack;
        navMeshAgent.speed = 0f;
    }

    private void UpdateAttack()
    {
        navMeshAgent.SetDestination(target.transform.position);
        attackTimer += Time.deltaTime;
        if (attackTimer > attackInterval)
        {
            attackTimer = 0f;
            OnRoaming();
        }
    }

    private void OnFollowPlayer()
    {
        currentEnemyState = EnemyState.FollowPlayer;
    }

    void UpdateWalkAnimation()
    {
        thisAnimator.SetFloat("Walk", navMeshAgent.velocity.sqrMagnitude);
    }

    private void UpdateFollowPlayer()
    {
        if (navMeshAgent.isOnNavMesh == true)
        {
            //ターゲットを追う//
            navMeshAgent.SetDestination(target.transform.position);

            //プレイヤーとの距離が近かったら、Attackモードになる//

            //navMeshAgent.remainingDistanceはゲーム再開時に一瞬0になることがある//

            if (target.activeSelf == true &&
                navMeshAgent.remainingDistance < playerAttackDistance)
            {
                //Debug.Log("navMeshAgent.remainingDistance " + navMeshAgent.remainingDistance);
                OnAttack();
            }
        }
    }

    private void OnRoaming()
    {
        currentEnemyState = EnemyState.Roaming;
        navMeshAgent.speed = defaultMoveSpeed;
        SetRandomDestination();
    }

    private void UpdateRoaming()
    {
        if (navMeshAgent.remainingDistance < 1.0f)
        {
            SetRandomDestination();
        }

        //プレイヤーとの距離が近かったら、Followモードになる
        if (target.activeSelf == true &&
            Vector3.Distance(target.transform.position, this.transform.position) < playerFindDistance)
        {
            OnFollowPlayer();
        }
    }

    private void OnDead()
    {
        currentEnemyState = EnemyState.Dead;

        hitArea.enabled = false;
        attackArea.enabled = false;
        navMeshAgent.speed = 0f;
        thisAnimator.SetTrigger("Death");
    }

    private void SetRandomDestination()
    {
        Vector3 point;
        if (RandomPointInRoamingArea(roamingArea.bounds, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);

            if (navMeshAgent.isOnNavMesh == true)
                navMeshAgent.SetDestination(point);
        }
    }

    private bool RandomPointInRoamingArea(Bounds area, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            ;
            Vector3 center = area.center;

            float x = Random.Range(center.x - area.extents.x, center.x + area.extents.x);
            float y = Random.Range(center.y - area.extents.y, center.y + area.extents.y);
            float z = Random.Range(center.z - area.extents.z, center.z + area.extents.z);

            Vector3 position = new Vector3(x, y, z);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    public void Initialize()
    {
        currentEnemyState = EnemyState.Roaming;

        this.gameObject.transform.position = defaultPosition;
    }

    public void OnPause()
    {
        IsPaused = true;
        thisAnimator.speed = 0f;
        hitArea.enabled = false;

        if (navMeshAgent.isOnNavMesh == true)
        {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.Stop();
        }
    }

    public void OnResume()
    {
        IsPaused = false;
        thisAnimator.speed = 1f;
        hitArea.enabled = true;

        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.Resume();
        }
    }

    public void OnPlayerDead()
    {
        currentEnemyState = EnemyState.Roaming;
    }

    public bool IsAlive {
        get { return currentEnemyState != EnemyState.Dead; }
    }

}