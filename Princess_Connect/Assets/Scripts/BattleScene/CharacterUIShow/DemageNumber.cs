using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageNumberController : MonoBehaviour
{
    [Header("图集设置")]
    public Sprite[] numberSprites; // 0-9 顺序
    public float digitSpacing = 15f;

    [Header("动画参数")]
    public float floatHeight = 2f;
    public float duration = 1f;
    public AnimationCurve fadeCurve;

    private Image[] digits;
    private RectTransform container;

    void Awake()
    {
        container = transform.Find("Canvas/DamageContainer").GetComponent<RectTransform>();
    }

    public void ShowDamage(int damage)
    {
        InitializeDigits(damage.ToString());
        StartCoroutine(PlayAnimation());
    }

    void InitializeDigits(string numberStr)
    {
        // 清理旧数字
        if (digits != null)
        {
            foreach (var d in digits) Destroy(d.gameObject);
        }

        digits = new Image[numberStr.Length];

        // 生成新数字
        for (int i = 0; i < numberStr.Length; i++)
        {
            int num = int.Parse(numberStr[i].ToString());
            CreateDigit(i, num);
        }

        // 水平居中
        float totalWidth = CalculateTotalWidth();
        container.anchoredPosition = new Vector2(-totalWidth / 2, 0);
    }

    void CreateDigit(int index, int number)
    {
        var obj = new GameObject($"Digit_{index}");
        obj.transform.SetParent(container, false);

        var img = obj.AddComponent<Image>();
        img.sprite = numberSprites[number];
        img.SetNativeSize();

        // 设置位置
        var rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(
            index * (rt.rect.width + digitSpacing),
            0
        );

        digits[index] = img;
    }

    IEnumerator PlayAnimation()
    {
        float elapsed = 0;
        Vector3 startPos = transform.position;
        CanvasGroup cg = this.transform.Find("Canvas").GetComponent<CanvasGroup>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 上浮运动
            transform.position = startPos +
                Vector3.up * floatHeight * t;

            // 淡出效果
            cg.alpha = fadeCurve.Evaluate(t);

            // 轻微缩放
            transform.localScale = Vector3.one *
                Mathf.Lerp(1.2f, 0.8f, t);

            yield return null;
        }

        Destroy(gameObject);
    }

    float CalculateTotalWidth()
    {
        if (digits.Length == 0) return 0;

        float width = 0;
        foreach (var d in digits)
        {
            width += d.rectTransform.rect.width + digitSpacing;
        }
        return width - digitSpacing; // 最后一个不需要间距
    }
}