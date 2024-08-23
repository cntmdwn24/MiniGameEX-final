using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Game2 : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject PausePanel;
    public GameObject StartPanel;
    public GameObject ColorButtonsPanel;

    [Header("Score UI")]
    public TMP_Text scoreText;

    [Header("Game Elements")]
    public GameObject objectToDestroy;
    public Image Heart1;
    public Image Heart2;
    public Image Heart3;
    public Image marbleImage;

    [Header("Color Buttons")]
    public Button[] colorButtons;

    [Header("Game Settings")]
    public Color[] colors = new Color[9];

    private List<int> sequence = new List<int>();
    private int score = 0;
    private int hearts = 3;
    private bool isPaused = true;
    private bool isPlayerTurn = false;
    private int currentStep = 0;

    private GameManager gameManager;

    private void Start()
    {
        PauseGame();
        UpdateHearts();
        gameManager = GameManager.Instance;
        UpdateScoreUI();
    }

    #region Game Flow
    public void StartGame()
    {
        StartPanel.SetActive(false);
        ResumeGame();
        StartCoroutine(GameTurn());
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        PauseGame();
    }

    public void ResumeGame()
    {
        PausePanel.SetActive(false);
        UnpauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void LeaveGame()
    {
        ActivateCanvasObjects();
        DestroyObjectToDestroy();
        if (gameManager != null)
        {
            gameManager.EndMiniGame(score);
        }
    }

    private void ActivateCanvasObjects()
    {
        Canvas[] allCanvasObjects = GameObject.FindObjectsOfType<Canvas>(true);
        foreach (Canvas canvasObject in allCanvasObjects)
        {
            canvasObject.gameObject.SetActive(true);
        }
    }

    private void DestroyObjectToDestroy()
    {
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
        }
    }
    #endregion

    #region Game Logic
    private IEnumerator GameTurn()
    {
        isPlayerTurn = false;
        currentStep = 0;

        sequence.Add(Random.Range(0, colors.Length));

        for (int i = 0; i < sequence.Count; i++)
        {
            marbleImage.color = colors[sequence[i]];
            yield return new WaitForSeconds(1f);
            marbleImage.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }

        isPlayerTurn = true;
        ColorButtonsPanel.SetActive(true);
    }

    public void OnColorButtonClick(int colorIndex)
    {
        if (!isPlayerTurn) return;

        if (colorIndex == sequence[currentStep])
        {
            currentStep++;
            if (currentStep >= sequence.Count)
            {
                score += 10 * sequence.Count;
                UpdateScoreUI();
                StartCoroutine(GameTurn());
                ColorButtonsPanel.SetActive(false);
            }
        }
        else
        {
            DecreaseHearts();
            ColorButtonsPanel.SetActive(false);
            if (hearts <= 0)
            {
                GameOver();
            }
            else
            {
                StartCoroutine(GameTurn());
            }
        }
    }
    #endregion

    #region Score Management
    public void IncreaseScore(int amount)
    {
        if (amount > 0)
        {
            score += amount;
            UpdateScoreUI();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void DecreaseHearts()
    {
        if (hearts > 0)
        {
            hearts--;
            UpdateHearts();
        }
    }

    private void GameOver()
    {
        if (gameManager != null)
        {
            gameManager.EndMiniGame(score);
        }
        LeaveGame();
    }
    #endregion

    #region Heart Management
    private void UpdateHearts()
    {
        Heart1.gameObject.SetActive(hearts > 0);
        Heart2.gameObject.SetActive(hearts > 1);
        Heart3.gameObject.SetActive(hearts > 2);
    }
    #endregion
}