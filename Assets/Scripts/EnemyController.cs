using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public Transform targetPoint;
    public EnemySpawner spawner;

    [Header("Attack")]
    public float attackRange = 2.5f;
    public float attackInterval = 1f;
    public int coreDamage = 1;

    [Header("Separation")]
    public float separationRadius = 1.0f;
    public float separationStrength = 1.2f;
    public float turnSmoothSpeed = 4f;

    private Rigidbody rb;
    private CoreHealth coreHealth;
    private bool isDead = false;
    private float attackTimer = 0f;

    public bool IsDead => isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        coreHealth = FindAnyObjectByType<CoreHealth>();
    }

    private void Update()
    {
        if (isDead) return;
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsGameOver) return;
        if (targetPoint == null) return;

        Vector3 targetPos = targetPoint.position;
        Vector3 currentPos = transform.position;

        Vector3 flatTarget = new Vector3(targetPos.x, currentPos.y, targetPos.z);
        float distance = Vector3.Distance(currentPos, flatTarget);

        if (distance > attackRange)
        {
            MoveToTarget(flatTarget);
        }
        else
        {
            AttackCore();
        }
    }

    private void MoveToTarget(Vector3 flatTarget)
    {
        attackTimer = 0f;

        Vector3 directionToTarget = (flatTarget - transform.position).normalized;
        Vector3 separationForce = GetSeparationForce();

        Vector3 finalDirection = directionToTarget + separationForce * separationStrength;
        finalDirection.y = 0f;

        if (finalDirection.sqrMagnitude > 0.001f)
        {
            finalDirection.Normalize();

            Vector3 smoothDirection = Vector3.Lerp(
                transform.forward,
                finalDirection,
                turnSmoothSpeed * Time.deltaTime
            ).normalized;

            transform.position += smoothDirection * moveSpeed * Time.deltaTime;
            transform.forward = smoothDirection;
        }
    }

    private Vector3 GetSeparationForce()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, separationRadius);
        Vector3 force = Vector3.zero;

        foreach (Collider col in nearby)
        {
            if (col.gameObject == gameObject) continue;

            EnemyController other = col.GetComponentInParent<EnemyController>();
            if (other == null) continue;
            if (other == this) continue;
            if (other.IsDead) continue;

            Vector3 away = transform.position - other.transform.position;
            away.y = 0f;

            float dist = away.magnitude;
            if (dist > 0.001f)
            {
                force += away.normalized / dist;
            }
        }

        return force;
    }

    private void AttackCore()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;

            if (coreHealth != null)
            {
                coreHealth.TakeDamage(coreDamage);
            }
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        
        if (spawner != null)
        {
            spawner.NotifyEnemyDestroyed();
        }

        
        Destroy(gameObject);
    }
}