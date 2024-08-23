using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class Game3 : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject PausePanel;
    public GameObject StartPanel;
    public GameObject ChoosePanel;

    [Header("Score UI")]
    public TMP_Text scoreText;

    [Header("Game Elements")]
    public Image Heart1;
    public Image Heart2;
    public Image Heart3;

    [Header("Fish Prefabs")]
    public GameObject[] fishPrefabs;
    public Transform[] spawnPoints;

    private List<GameObject> spawnedFishes = new List<GameObject>();
    private int score = 0;
    private int hearts = 3;
    private bool isPaused = true;
    private bool isPlayerTurn = false;

    private int fishSpawnCount;
    private float minFishSpeed = 70f;
    private float maxFishSpeed = 100f;
    private int round;

    private int fishTypeWithExtra;

    private void Start()
    {
        PauseGame();
        UpdateHearts();
        UpdateScoreUI();
        fishSpawnCount = 5;
        round = 0;
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
    }

    private void ActivateCanvasObjects()
    {
        Canvas[] allCanvasObjects = GameObject.FindObjectsOfType<Canvas>(true);
        foreach (Canvas canvasObject in allCanvasObjects)
        {
            canvasObject.gameObject.SetActive(true);
        }
    }

    #endregion

    #region Game Logic

    private IEnumerator GameTurn()
    {
        isPlayerTurn = false;

        SpawnFishes();

        yield return new WaitForSeconds(10f);

        foreach (GameObject fish in spawnedFishes)
        {
            Destroy(fish);
        }
        spawnedFishes.Clear();

        fishSpawnCount += 3;

        minFishSpeed += 10f;
        maxFishSpeed += 10f;

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        isPlayerTurn = true;
        Debug.Log("Player Turn! Choose the fish type with the most numbers.");
        ChoosePanel.SetActive(true);
    }

    private void SpawnFishes()
    {
        int baseCount = fishSpawnCount / fishPrefabs.Length;
        fishTypeWithExtra = Random.Range(0, fishPrefabs.Length);

        for (int i = 0; i < fishPrefabs.Length; i++)
        {
            int spawnCount = baseCount;

            if (i == fishTypeWithExtra)
            {
                spawnCount++;
            }

            for (int j = 0; j < spawnCount; j++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject spawnedFish = Instantiate(fishPrefabs[i], spawnPoint.position, Quaternion.identity, spawnPoint);

                FishMovement fishMovement = spawnedFish.GetComponent<FishMovement>();
                if (fishMovement != null)
                {
                    fishMovement.Initialize(Random.Range(minFishSpeed, maxFishSpeed));
                }

                spawnedFishes.Add(spawnedFish);
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
        if (hearts <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over! No more hearts.");
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

    #region Player Turn

    public void OnFishTypeSelected(int fishTypeIndex)
    {
        ChoosePanel.SetActive(false);
        if (isPlayerTurn)
        {
            int fishCount = spawnedFishes.FindAll(fish => fish.GetComponent<FishMovement>().fishTypeIndex == fishTypeIndex).Count;

            if (fishTypeIndex == fishTypeWithExtra)
            {
                IncreaseScore(10 + (round * 20));
                Debug.Log("Correct! You chose the right fish type.");
            }
            else
            {
                DecreaseHearts();
                Debug.Log("Wrong! Try again.");
            }

            StartCoroutine(GameTurn());
            round++;
        }
    }

    #endregion
}