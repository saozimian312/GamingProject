using UnityEngine;

public class IncomeBlock : MonoBehaviour
{
    public int goldPerTick = 5;
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

            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.AddGold(goldPerTick);
            }
        }
    }
}