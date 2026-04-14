using UnityEngine;

public class AttackBlock : MonoBehaviour
{
    public float attackRange = 20f;
    public float attackInterval = 1f;
    public int damage = 1;

    private float timer;
    private FallingBlockController fallingBlock;
    private BlockVFX vfx;

    private void Awake()
    {
        fallingBlock = GetComponent<FallingBlockController>();
        vfx = GetComponent<BlockVFX>();
    }

    private void Update()
    {
        if (fallingBlock == null) return;
        if (!fallingBlock.IsPlaced) return;

        timer += Time.deltaTime;

        if (timer >= attackInterval)
        {
            timer = 0f;
            ShootNearestEnemy();
        }
    }

    private void ShootNearestEnemy()
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

        if (nearest == null) return;

        Vector3 center = fallingBlock.GetEffectCenter();

        if (vfx != null)
        {
            vfx.FlashAttack();
            vfx.PlayAttackLine(nearest.transform.position + Vector3.up * 0.5f);
        }

        if (VFXManager.Instance != null)
        {
            VFXManager.Instance.PlayAttackMuzzle(center);
            VFXManager.Instance.SpawnAttackBullet(center, nearest, damage);
        }
    }
}