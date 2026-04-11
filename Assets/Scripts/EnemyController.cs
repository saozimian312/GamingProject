using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform targetPoint;
    public EnemySpawner spawner;

    private bool canMove = true;

    private void Update()
    {
        if (!canMove || targetPoint == null) return;

        Vector3 direction = (targetPoint.position - transform.position).normalized;
        direction.y = 0f;
        direction.z = 0f;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
 {
    if (other.CompareTag("Core"))
    {
        CoreHealth core = other.GetComponent<CoreHealth>();
        if (core != null)
        {
            core.TakeDamage(1);
        }

        Destroy(gameObject);
    }
   
 }
 private void OnDestroy()
 {
    if (!Application.isPlaying) return;

    if (spawner != null)
    {
        spawner.NotifyEnemyDestroyed();
    }
 }
}