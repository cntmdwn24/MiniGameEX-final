using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public int fishTypeIndex;
    private float speed;
    private Vector2 direction;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("RectTransform component is missing from this GameObject.");
            enabled = false;
        }
    }

    public void Initialize(float fishSpeed)
    {
        speed = fishSpeed;
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        UpdateRotation();
        InvokeRepeating("ChangeDirection", 1f, Random.Range(1f, 3f));
    }

    void Update()
    {
        if (rectTransform == null) return;

        rectTransform.anchoredPosition += direction * speed * Time.deltaTime;
    }

    void ChangeDirection()
    {
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        UpdateRotation();
    }

    void UpdateRotation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}