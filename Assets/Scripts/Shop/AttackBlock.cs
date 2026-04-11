using UnityEngine;

public class AttackBlock : MonoBehaviour
{
    public float attackRange = 20f;
    public float attackInterval = 1f;
    public int damage = 1;

    private float timer;
    private FallingBlockController fallingBlock;

    private void Awake()
    {
        fallingBlock = GetComponent<FallingBlockController>();
    }

    private void Update()
    {
        if (fallingBlock == null) return;
        if (!fallingBlock.IsPlaced) return;

        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            timer = 0f;
            AttackNearestEnemy();
        }
    }

    private void AttackNearestEnemy()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>();

        EnemyHealth nearest = null;
        float nearestDistance = attackRange;

        foreach (EnemyHealth enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy;
            }
        }

        if (nearest != null)
        {
            nearest.TakeDamage(damage);
        }
    }
}