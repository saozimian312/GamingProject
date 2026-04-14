using UnityEngine;

public class ProjectileBullet : MonoBehaviour
{
    public float speed = 12f;
    public float hitDistance = 0.2f;

    private EnemyHealth target;
    private int damage;

    public void Init(EnemyHealth targetEnemy, int bulletDamage)
    {
        target = targetEnemy;
        damage = bulletDamage;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = target.transform.position + Vector3.up * 0.5f;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        Vector3 dir = targetPos - transform.position;
        if (dir.sqrMagnitude > 0.0001f)
        {
            transform.forward = dir.normalized;
        }

        if (Vector3.Distance(transform.position, targetPos) <= hitDistance)
        {
            target.TakeDamage(damage);

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayAttackHit(targetPos);
            }

            Destroy(gameObject);
        }
    }
}