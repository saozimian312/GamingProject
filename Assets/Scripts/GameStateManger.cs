using UnityEngine;
using TMPro;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public bool IsGameOver { get; private set; }
    public TMP_Text resultText;

    private void Awake()
    {
        Instance = this;

        if (resultText != null)
        {
            resultText.gameObject.SetActive(false);
        }
    }

    public void WinGame()
    {
        if (IsGameOver) return;

        IsGameOver = true;

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = "You Win";
        }
    }

    public void LoseGame()
    {
        if (IsGameOver) return;

        IsGameOver = true;

        if (resultText != null)
        {
            resultText.gameObject.SetActive(true);
            resultText.text = "Game Over";
        }
    }
}