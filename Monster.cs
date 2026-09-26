using UnityEngine;
using Pathfinding;

public class Monster : LivingEntity
{
    [Header("AI")]
    public Transform target;
    public AIPath aiPath;
    public float retargetInterval = 0.3f;

    [Header("Melee")]
    public LayerMask whatToHit; // 在 Inspector 中勾選玩家所屬的 Layer
    public float damage = 1f;   // 每次撞擊扣 1 顆心

    private Animator _animator;
    private float _retargetTimer;

    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
    }

    private void Update()
    {
        _retargetTimer -= Time.deltaTime;
        if (_retargetTimer <= 0f)
        {
            _retargetTimer = retargetInterval;
            FindNearestPlayer();
        }

        if (target == null) return;

        aiPath.destination = target.position;
    }

    private void FindNearestPlayer()
    {
        Player nearest = null;
        float nearestDistSqr = float.MaxValue;
        Vector3 myPos = transform.position;

        foreach (var player in Player.ActivePlayers)
        {
            if (player == null) continue;

            float distSqr = (player.transform.position - myPos).sqrMagnitude;
            if (distSqr < nearestDistSqr)
            {
                nearestDistSqr = distSqr;
                nearest = player;
            }
        }

        target = nearest != null ? nearest.transform : null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 檢查進入觸發器的圖層是否在 whatToHit 中
        if (((1 << other.gameObject.layer) & whatToHit.value) == 0)
        {
            return;
        }

        // 檢查被撞擊對象是否為玩家或具備 LivingEntity
        LivingEntity entity = other.GetComponent<LivingEntity>();

        if (entity == null)
        {
            return;
        }

        // 對碰撞到的玩家扣血（會觸發 Player.cs 的共享血量扣除）
        entity.TakeDamage(damage);

        // 怪物撞到玩家後立即自毀消失
        Destroy(gameObject);
    }
}