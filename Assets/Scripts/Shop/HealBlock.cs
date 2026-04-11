using UnityEngine;

public class HealBlock : MonoBehaviour
{
    public int healAmount = 1;
    public float interval = 2f;

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

        if (timer >= interval)
        {
            timer = 0f;

            CoreHealth core = FindAnyObjectByType<CoreHealth>();
            if (core != null)
            {
                core.TakeDamage(-healAmount);
            }
        }
    }
}