using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public CharacterController controller; // Ссылка на компонент CharacterController
    public float walkSpeed = 6f;    // Скорость ходьбы
    public float runSpeed = 12f;    // Скорость бега
    public float gravity = -9.81f;  // Сила гравитации

    private Vector3 velocity; // Вектор скорости персонажа
    private bool isGrounded;  // Переменная для проверки, находится ли персонаж на земле
    private CrouchController crouchController; // Ссылка на компонент CrouchController

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

        float currentSpeed;
        if (crouchController != null && crouchController.IsCrouching())
        {
            currentSpeed = crouchController.GetCrouchMovementSpeed();
        }
        else
        {
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        }

        // Обновляем направление движения
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Обновляем гравитацию
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}