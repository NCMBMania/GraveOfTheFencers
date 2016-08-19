using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class Enemy : GameActorBase, IGameActor
{
    private enum EnemyState { Roaming, FollowPlayer, Attack, Dead, Disable }

    [SerializeField]
    private EnemyState currentEnemyState = EnemyState.Roaming;

    public float playerFindDistance = 5f;
    public float playerAttackDistance = 1.3f;
    public float attackInterval = 2f;
    private float attackTimer = 0f;

    public Collider roamingArea;//行動範囲//

    private GameObject target;
    private Vector3 defaultPosition;
    private float defaultMoveSpeed;

    public override void Awake()
    {
        base.Awake();

        target = FindObjectOfType<Player>().gameObject;

        defaultPosition = this.gameObject.transform.position;
        defaultMoveSpeed = navMeshAgent.speed;
    }

    public override void Start()
    {
        base.Start();

        state_InGame.EnemyList.Add(this);
        currentEnemyState = EnemyState.Roaming;

        this.gameObject.transform.position = defaultPosition;
    }

    public override void OnAttackHit(Collider col)
    {
        if (col.tag == "Player")
        {
            OnDead();
        }
    }

    public void Update()
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
        animator.SetTrigger("Attack");
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

    private void UpdateWalkAnimation()
    {
        animator.SetFloat("Walk", navMeshAgent.velocity.sqrMagnitude);
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
        if (navMeshAgent.enabled == true && //nullが出ることがある？
            navMeshAgent.remainingDistance < 1.0f)
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

    public override void OnDead()
    {
        currentEnemyState = EnemyState.Dead;
        animator.SetTrigger("Death");

        navMeshAgent.speed = 0f;
        navMeshAgent.enabled = false;

        base.OnDead();
    }

    public override void OnPause()
    {
        base.OnPause();
    }

    public override void OnResume()
    {
        base.OnResume();
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

    public void OnPlayerDead()
    {
        currentEnemyState = EnemyState.Roaming;
    }

    public bool IsAlive
    {
        get { return currentEnemyState != EnemyState.Dead; }
    }
}