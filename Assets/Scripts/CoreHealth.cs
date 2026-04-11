using UnityEngine;
using TMPro;

public class CoreHealth : MonoBehaviour
{
    public int health = 10;
    public TMP_Text coreHpText;

    private void Start()
    {
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
    health -= damage;

    if (health < 0)
    {
        health = 0;
    }

    UpdateUI();

    if (health <= 0)
{
    if (GameStateManager.Instance != null)
    {
        GameStateManager.Instance.LoseGame();
    }
}
    }

    private void UpdateUI()
    {
        if (coreHpText != null)
        {
            coreHpText.text = "Core HP: " + health;
        }
    }
}