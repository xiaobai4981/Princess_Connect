using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NormalBtnChange : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler,
    IPointerExitHandler // ��������Ƴ�ʱ�ɿ������
{
    [SerializeField] float pressedScale = 0.9f;
    [SerializeField] float scaleSpeed = 10f;

    private Vector3 originalScale;
    private bool isPressed;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // ƽ�����ɵ�Ŀ��ߴ�
        Vector3 targetScale = isPressed ?
            originalScale * pressedScale : originalScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPressed) isPressed = false;
    }
}