using UnityEngine;

public class IncomeBlock : MonoBehaviour
{
    public int goldPerTick = 5;
    public float interval = 2f;

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

        if (timer >= interval)
        {
            timer = 0f;

            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.AddGold(goldPerTick);
            }

            if (vfx != null)
            {
                vfx.FlashIncome();
            }

            if (VFXManager.Instance != null)
            {
                VFXManager.Instance.PlayIncomeBurst(fallingBlock.GetEffectCenter());
            }
        }
    }
}