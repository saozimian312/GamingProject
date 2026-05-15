using UnityEngine;

public class IncomeBlock : MonoBehaviour
{
    public int goldAmount = 5;
    public float incomeInterval = 2f;

    private float timer = 0f;

    private void Update()
    {
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsGameOver) return;

        timer += Time.deltaTime;

        if (timer >= incomeInterval)
        {
            timer = 0f;

            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.AddGold(goldAmount);
            }
        }
    }
}