using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public CharacterController controller; // Ссылка на компонент CharacterController
    public float walkSpeed = 6f;    // Скорость ходьбы
    public float runSpeed = 12f;    // Скорость бега
    public float slowRunSpeed = 8f; // Скорость медленного бега (между ходьбой и бегом)
    public float gravity = -9.81f;  // Сила гравитации

    public float smoothSpeedChange = 5f; // Скорость изменения скорости (чем больше значение, тем быстрее смена)

    private Vector3 velocity; // Вектор скорости персонажа
    private bool isGrounded;  // Переменная для проверки, находится ли персонаж на земле
    private CrouchController crouchController; // Ссылка на компонент CrouchController
    private StaminaController staminaController; // Ссылка на компонент StaminaController

    private float currentSpeed; // Текущая скорость персонажа
    private float targetSpeed;  // Целевая скорость, к которой мы стремимся

    private bool isForcedSlowRun; // Флаг принудительного медленного бега
    private bool isShiftPressed; // Флаг для отслеживания состояния удержания "Shift"

    void Awake()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        crouchController = GetComponent<CrouchController>();
        if (crouchController == null)
        {
            Debug.LogError("CrouchController не найден на объекте!");
        }

        staminaController = GetComponent<StaminaController>();
        if (staminaController == null)
        {
            Debug.LogError("StaminaController не найден на объекте!");
        }
    }

    void Update()
    {
        if (controller == null)
        {
            Debug.LogError("CharacterController не назначен!");
            return;
        }

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Обновление вертикальной скорости при касании земли
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Проверка состояния "Shift"
        isShiftPressed = Input.GetKey(KeyCode.LeftShift);

        // Определение целевой скорости в зависимости от состояния персонажа
        if (crouchController != null && crouchController.IsCrouching())
        {
            targetSpeed = crouchController.GetCrouchMovementSpeed();
        }
        else
        {
            if (isShiftPressed)
            {
                if (staminaController.GetCurrentStamina() > 0)
                {
                    targetSpeed = runSpeed;
                    staminaController.StartRunning();
                    isForcedSlowRun = false; // Сбрасываем флаг принудительного медленного бега
                }
                else
                {
                    targetSpeed = slowRunSpeed;
                    isForcedSlowRun = true; // Устанавливаем флаг принудительного медленного бега
                }
            }
            else
            {
                if (isForcedSlowRun)
                {
                    // Если клавиша Shift отпущена после истощения выносливости
                    isForcedSlowRun = false;
                }
                targetSpeed = walkSpeed;
                staminaController.StopRunning();
            }
        }

        // Плавное изменение текущей скорости к целевой скорости
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, smoothSpeedChange * Time.deltaTime);

        // Обновляем направление движения
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Обновляем гравитацию
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}