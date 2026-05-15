using UnityEngine;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;
    public int goldReward = 10;
    public TMP_Text hpText;

    private EnemyController controller;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
        UpdateHpText();
    }

    private void Start()
    {
        UpdateHpText();
    }

    public void TakeDamage(int damage)
    {
        if (controller != null && controller.IsDead) return;

        health -= damage;
        UpdateHpText();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.AddGold(goldReward);
        }

        if (controller != null)
        {
            controller.Die();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdateHpText()
    {
        if (hpText != null)
        {
            hpText.text = health.ToString();
        }
    }
}