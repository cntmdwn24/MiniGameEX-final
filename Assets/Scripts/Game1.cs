using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Game1 : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject PausePanel;
    public GameObject StartPanel;

    [Header("Score UI")]
    public TMP_Text scoreText;

    [Header("Game Elements")]
    public GameObject player;
    public GameObject obstaclePrefab;

    public Image Heart1;
    public Image Heart2;
    public Image Heart3;

    private int score = 0;
    private int hearts = 3;
    private bool isPaused = true;
    private bool isGrounded = true;

    private Rigidbody2D playerRb;

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody2D>();
        PauseGame();
        UpdateHearts();
        UpdateScoreUI();
    }

    public void StartGame()
    {
        score = 0;
        UpdateScoreUI();
        StartPanel.SetActive(false);
        ResumeGame();
        StartCoroutine(SpawnObstacles());
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

    private void Update()
    {
        // 점프 처리
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && isGrounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        playerRb.velocity = new Vector2(playerRb.velocity.x, 7f);
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            DecreaseHearts();
            if (hearts <= 0)
            {
                GameOver();
            }
        }
    }

    private IEnumerator SpawnObstacles()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1.5f, 3f)); // 장애물 생성 간격
            Vector2 spawnPosition = new Vector2(10f, -1.5f); // 화면 오른쪽에서 생성
            GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);

            // 장애물이 왼쪽으로 이동하도록 설정
            Rigidbody2D obstacleRb = obstacle.GetComponent<Rigidbody2D>();
            obstacleRb.velocity = Vector2.left * 5f; // 장애물 속도

            // 장애물 삭제
            Destroy(obstacle, 10f);
        }
    }

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

    private void UpdateHearts()
    {
        Heart1.gameObject.SetActive(hearts > 0);
        Heart2.gameObject.SetActive(hearts > 1);
        Heart3.gameObject.SetActive(hearts > 2);
    }

    private void GameOver()
    {
        PauseGame();
        Debug.Log("Game Over. Final Score: " + score);
    }
}