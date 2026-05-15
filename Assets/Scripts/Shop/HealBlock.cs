using UnityEngine;

public class HealBlock : MonoBehaviour
{
    public int healAmount = 1;
    public float healInterval = 2f;

    private float timer = 0f;
    private CoreHealth coreHealth;

    private void Start()
    {
        coreHealth = FindAnyObjectByType<CoreHealth>();
    }

    private void Update()
    {
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsGameOver) return;
        if (coreHealth == null) return;

        timer += Time.deltaTime;

        if (timer >= healInterval)
        {
            timer = 0f;
            coreHealth.TakeDamage(-healAmount);
        }
    }
}