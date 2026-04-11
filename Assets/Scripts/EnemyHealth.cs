using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;
    public int goldReward = 10;
    public TMP_Text hpText;

    private Renderer enemyRenderer;
    private Color originalColor;

    private void Awake()
    {
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }

        UpdateHPText();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateHPText();
        StartCoroutine(FlashRed());

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

        Destroy(gameObject);
    }

    private void UpdateHPText()
    {
        if (hpText != null)
        {
            hpText.text = health.ToString();
        }
    }

    private IEnumerator FlashRed()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            enemyRenderer.material.color = originalColor;
        }
    }
}