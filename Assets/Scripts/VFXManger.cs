using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    public GameObject attackMuzzlePrefab;
    public GameObject attackHitPrefab;
    public GameObject attackBulletPrefab;

    public GameObject incomeBurstPrefab;
    public GameObject healAuraPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayAttackMuzzle(Vector3 position)
    {
        SpawnAndPlay(attackMuzzlePrefab, position);
    }

    public void PlayAttackHit(Vector3 position)
    {
        SpawnAndPlay(attackHitPrefab, position);
    }

    public void PlayIncomeBurst(Vector3 position)
    {
        SpawnAndPlay(incomeBurstPrefab, position);
    }

    public void PlayHealAura(Vector3 position)
    {
        SpawnAndPlay(healAuraPrefab, position);
    }

    public void SpawnAttackBullet(Vector3 startPosition, EnemyHealth target, int damage)
    {
        if (attackBulletPrefab == null || target == null) return;

        GameObject bullet = Instantiate(attackBulletPrefab, startPosition, Quaternion.identity);

        ProjectileBullet projectile = bullet.GetComponent<ProjectileBullet>();
        if (projectile != null)
        {
            projectile.Init(target, damage);
        }
    }

    private void SpawnAndPlay(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;

        GameObject vfx = Instantiate(prefab, position, Quaternion.identity);

        ParticleSystem[] particleSystems = vfx.GetComponentsInChildren<ParticleSystem>(true);
        float longestLifetime = 1f;

        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play();

            float totalLifetime = ps.main.duration + ps.main.startLifetime.constantMax;
            if (totalLifetime > longestLifetime)
            {
                longestLifetime = totalLifetime;
            }
        }

        Destroy(vfx, longestLifetime + 0.2f);
    }
}