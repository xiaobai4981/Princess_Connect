using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNextLastBtn : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("从中心到左侧的距离")]
    public float leftDistance = 10f;

    [Tooltip("从中心到右侧的距离")]
    public float rightDistance = 10f;

    [Tooltip("完成一次移动所需时间（秒）")]
    public float movementDuration = 0.8f;

    [Tooltip("到达边界后的暂停时间（秒）")]
    public float pauseDuration = 0f;

    [Tooltip("是否在开始时向右移动")]
    public bool startMovingRight = true;

    [Header("Advanced")]
    [Tooltip("移动缓动曲线")]
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 centerPosition;
    private float timer;
    private bool isMovingRight;
    private bool isPausing;
    private float pauseTimer;

    void Start()
    {
        // 记录初始位置作为中心点
        centerPosition = transform.position;
        isMovingRight = startMovingRight;
        timer = 0f;
        isPausing = false;
    }

    void Update()
    {
        if (isPausing)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                isPausing = false;
                pauseTimer = 0f;
                isMovingRight = !isMovingRight; // 反转方向
            }
            return;
        }

        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / movementDuration);
        float curvedProgress = movementCurve.Evaluate(progress);

        // 计算当前X偏移量
        float currentOffset = isMovingRight
            ? Mathf.Lerp(-leftDistance, rightDistance, curvedProgress)
            : Mathf.Lerp(rightDistance, -leftDistance, curvedProgress);

        // 以中心位置为基准进行移动
        transform.position = new Vector3(
            centerPosition.x + currentOffset,
            centerPosition.y,
            centerPosition.z
        );

        // 当移动完成后进入暂停状态
        if (progress >= 1f)
        {
            timer = 0f;
            isPausing = true;
        }
    }
}
