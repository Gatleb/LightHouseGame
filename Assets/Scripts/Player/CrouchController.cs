using UnityEngine;

public class CrouchController : MonoBehaviour
{
    public Transform playerCamera;

    public float crouchHeight = 0.5f;  // Высота камеры в приседе
    public float standHeight = 0.545f;   // Высота камеры в обычном состоянии
    public float idleCrouchHeight = 0.3f; // Дополнительная высота приседания после бездействия
    public float crouchTransitionSpeed = 0.1f; // Скорость перехода в присед

    [Tooltip("Скорость передвижения в приседе")]
    public float crouchMovementSpeed = 3f; // Скорость передвижения в приседе

    public float crouchSpeedMultiplier = 1.0f; // Множитель скорости при приседе

    private bool isCrouching = false;
    private float idleTimer = 0f;
    private float idleTimeThreshold = 0.1f; // Порог времени бездействия для активации дополнительного приседания
    private float heightSmoothVelocity;    // Переменная для плавного изменения высоты камеры

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>().transform;
            if (playerCamera == null)
            {
                Debug.LogError("Камера игрока не найдена!");
            }
        }

        // Устанавливаем начальную высоту камеры
        playerCamera.localPosition = new Vector3(0f, standHeight, 0f);
    }

    void Update()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Камера игрока не назначена!");
            return;
        }

        // Обработка состояния приседания
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            ResetIdleTimer(); // Сбрасываем таймер бездействия при каждом нажатии C
        }

        // Высота камеры при приседании и стоянии
        float targetHeight = isCrouching ? (idleTimer >= idleTimeThreshold ? (crouchHeight - idleCrouchHeight) : crouchHeight) : standHeight;

        // Используем SmoothDamp для плавного изменения высоты камеры
        float currentHeight = playerCamera.localPosition.y;
        float smoothHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightSmoothVelocity, crouchTransitionSpeed);

        playerCamera.localPosition = new Vector3(
            playerCamera.localPosition.x,
            smoothHeight,
            playerCamera.localPosition.z
        );

        // Обновляем таймер бездействия
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            idleTimer += Time.deltaTime;
        }
        else
        {
            ResetIdleTimer();
        }
    }

    void ResetIdleTimer()
    {
        idleTimer = 0f;
    }

    // Возвращает текущее состояние приседания
    public bool IsCrouching()
    {
        return isCrouching;
    }

    // Возвращает скорость передвижения в приседе
    public float GetCrouchMovementSpeed()
    {
        return crouchMovementSpeed * crouchSpeedMultiplier;
    }
}