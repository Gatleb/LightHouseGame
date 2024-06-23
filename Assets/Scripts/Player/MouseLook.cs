using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody; // Ссылка на тело персонажа
    public Transform playerCamera; // Ссылка на камеру игрока
    public float mouseSensitivity = 100f; // Чувствительность мыши
    public float smoothTime = 0.1f; // Время сглаживания движения мыши

    private float yaw; // Угол поворота по оси Y
    private float pitch; // Угол поворота по оси X
    private Vector2 currentRotation; // Текущая плавная интерполяция вращения
    private Vector2 rotationSmoothVelocity; // Скорость плавной интерполяции вращения

    public float maxLeanAngle = 15f; // Максимальный угол наклона камеры
    public float maxLeanOffset = 0.5f; // Максимальное смещение камеры по оси X при наклоне

    public float leanSpeed = 10f; // Скорость наклона камеры
    public float leanOffsetSpeed = 10f; // Скорость сдвига камеры

    private float currentLeanOffset = 0f; // Текущее смещение камеры по оси X

    // Переменная для хранения исходного положения камеры
    private Vector3 initialCameraPosition;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Блокируем курсор в центре экрана
        Cursor.visible = false; // Делаем курсор невидимым

        if (playerBody == null)
        {
            playerBody = transform;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }

        // Сохраняем исходное положение камеры
        initialCameraPosition = playerCamera.localPosition;
    }

    void Update()
    {
        // Управление взглядом мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f); // Ограничиваем угол поворота по оси X

        // Плавная интерполяция текущего вращения к целевому
        currentRotation = Vector2.SmoothDamp(currentRotation, new Vector2(pitch, yaw), ref rotationSmoothVelocity, smoothTime);

        // Применяем вращение к телу игрока
        playerBody.localRotation = Quaternion.Euler(0f, currentRotation.y, 0f);

        // Применяем вращение и наклон к камере
        playerCamera.localRotation = Quaternion.Euler(currentRotation.x, 0f, playerCamera.localRotation.eulerAngles.z);

        // Управление наклонами камеры
        if (Input.GetKey(KeyCode.Q))
        {
            LeanCamera(maxLeanAngle); // Наклон камеры влево
        }
        else if (Input.GetKey(KeyCode.E))
        {
            LeanCamera(-maxLeanAngle); // Наклон камеры вправо
        }
        else
        {
            LeanCamera(0); // Возврат камеры в центральное положение
        }

        // Применение смещения камеры по оси X при наклоне
        ApplyCameraOffset();
    }

    // Функция для наклона камеры
    void LeanCamera(float targetLeanAngle)
    {
        // Плавная интерполяция текущего угла наклона к целевому
        float currentLeanAngle = Mathf.LerpAngle(playerCamera.localRotation.eulerAngles.z, targetLeanAngle, leanSpeed * Time.deltaTime);
        Vector3 leanRotation = playerCamera.localRotation.eulerAngles;
        leanRotation.z = currentLeanAngle;
        playerCamera.localRotation = Quaternion.Euler(leanRotation);

        // Обновляем текущее смещение камеры на основе угла наклона
        float targetOffset = (targetLeanAngle / maxLeanAngle) * maxLeanOffset;
        // Сдвиг должен быть в противоположную сторону углу наклона
        currentLeanOffset = Mathf.Lerp(currentLeanOffset, -targetOffset, leanOffsetSpeed * Time.deltaTime);
    }

    // Функция для применения смещения камеры по оси X
    void ApplyCameraOffset()
    {
        Vector3 cameraPosition = playerCamera.localPosition;
        cameraPosition.x = currentLeanOffset;

        // Проверка на погрешности, если смещение близко к нулю, устанавливаем точно 0
        if (Mathf.Abs(currentLeanOffset) < 0.0001f)
        {
            cameraPosition.x = initialCameraPosition.x; // Возвращаемся в точное исходное положение по X
        }

        playerCamera.localPosition = cameraPosition;
    }
}
