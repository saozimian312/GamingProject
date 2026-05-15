using UnityEngine;
using TMPro;

public class CoreHealth : MonoBehaviour
{
    [Header("Core HP")]
    public int maxHealth = 100;
    public int currentHealth = 100;

    public TMP_Text hpText;

    private void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHpText();
    }

    public void TakeDamage(int amount)
    {
        // amount > 0 = 扣血
        // amount < 0 = 治疗
        currentHealth -= amount;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHpText();

        if (currentHealth <= 0)
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.LoseGame();
            }
        }
    }

    private void UpdateHpText()
    {
        if (hpText != null)
        {
            hpText.text = "Core HP: " + currentHealth + "/" + maxHealth;
        }
    }
}