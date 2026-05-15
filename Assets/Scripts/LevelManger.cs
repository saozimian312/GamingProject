using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public LevelEntry[] levels;
    public EnemySpawner enemySpawner;
    public HeightManager heightManager;
    public BlockSpawner blockSpawner;

    public TMP_Text levelText;
    public TMP_Text levelStartText;
    public TMP_Text resultText;
    public GameObject centerMessagePanel;

    public float levelStartShowTime = 1.2f;

    private int currentLevelIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
    if (centerMessagePanel != null)
    {
        centerMessagePanel.SetActive(false);
    }

    currentLevelIndex = Mathf.Clamp(MenuStartData.startLevelIndex, 0, levels.Length - 1);
    StartCoroutine(StartLevelRoutine(currentLevelIndex));
    }

    public void OnLevelCleared()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Length)
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.WinGame();
            }
            return;
        }

        StartCoroutine(StartLevelRoutine(currentLevelIndex));
    }

    private IEnumerator StartLevelRoutine(int index)
    {
        ActivateLevel(index);
        ClearAllBlocks();

        if (heightManager != null)
        {
            heightManager.ResetForNewLevel();
        }

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.ResetGold(100);
            ShopManager.Instance.RefreshOffers();
        }

        UpdateLevelUI(index);
        ShowLevelStartText(index);

        yield return new WaitForSeconds(levelStartShowTime);

        HideCenterMessagePanel();

        if (enemySpawner != null)
        {
            enemySpawner.ApplyLevelConfig(levels[index].config);
            enemySpawner.StartLevel();
        }
    }

    private void ActivateLevel(int index)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].levelRoot != null)
            {
                levels[i].levelRoot.SetActive(i == index);
            }
        }
    }

    private void ClearAllBlocks()
    {
        FallingBlockController[] blocks = FindObjectsByType<FallingBlockController>();
        foreach (FallingBlockController block in blocks)
        {
            if (block != null)
            {
                Destroy(block.gameObject);
            }
        }

        if (blockSpawner != null && blockSpawner.mobileMoveInput != null)
        {
            blockSpawner.mobileMoveInput.currentBlock = null;
        }
    }

    private void UpdateLevelUI(int index)
    {
        if (levelText != null)
        {
            levelText.text = "Level " + (index + 1);
        }
    }

    private void ShowLevelStartText(int index)
    {
        if (levelStartText != null)
        {
            levelStartText.text = "Level " + (index + 1);
        }

        if (resultText != null)
        {
            resultText.text = "";
        }

        if (centerMessagePanel != null)
        {
            centerMessagePanel.SetActive(true);
        }
    }

    private void HideCenterMessagePanel()
    {
        if (centerMessagePanel != null)
        {
            centerMessagePanel.SetActive(false);
        }
    }
    public int CurrentLevelIndex => currentLevelIndex;

    public void RestartCurrentLevel()
    {
    MenuStartData.startLevelIndex = currentLevelIndex;
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}