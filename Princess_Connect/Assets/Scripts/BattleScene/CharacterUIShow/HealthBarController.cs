using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public RectTransform healthFill;
    private int maxLength;
    public int currentHealth;
    public CanvasGroup canvasGroup;
    public Transform target;
    public Vector3 offset = new Vector3(0, 2.5f, 0);

    void Update()
    {
        // 动态跟踪目标位置
        if (target != null)
        {
            transform.position = target.position + offset;
        }

        // 自动隐藏满血血条
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha,
            (currentHealth < maxLength) ? 1 : 0,
            Time.deltaTime * 5);
    }

    public void Initialize(int maxLength, int currentHealth)
    {
        this.maxLength = maxLength;
        healthFill.sizeDelta = new Vector2((float)currentHealth / (float)maxLength * 200f, healthFill.sizeDelta.y);
        this.currentHealth = currentHealth;
    }
}