using UnityEngine;
using TMPro;


public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public bool IsGameOver { get; private set; }

    public TMP_Text resultText;
    public GameObject centerMessagePanel;

    private void Awake()
    {
        Instance = this;

        
        Time.timeScale = 1f;

        if (centerMessagePanel != null)
        {
            centerMessagePanel.SetActive(false);
        }
    }

    public void WinGame()
    {
    if (IsGameOver) return;

    IsGameOver = true;
    Time.timeScale = 0f;

    if (resultText != null)
       {
        resultText.text = "You Win";
       } 

    if (centerMessagePanel != null)
       {
        centerMessagePanel.SetActive(true);
       }

    if (restartButtonObject != null)
       {
        restartButtonObject.SetActive(false);
       }
    }

    public GameObject restartButtonObject;

    public void LoseGame()
   {
    if (IsGameOver) return;

    IsGameOver = true;
    Time.timeScale = 0f;

    if (resultText != null)
       {
        resultText.text = "Game Over";
       }

    if (centerMessagePanel != null)
       {
        centerMessagePanel.SetActive(true);
       }

    if (restartButtonObject != null)
       {
        restartButtonObject.SetActive(true);
       }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResetGameState()
    {
    IsGameOver = false;
    Time.timeScale = 1f;

    if (restartButtonObject != null)
       {
        restartButtonObject.SetActive(false);
       }
    }
}