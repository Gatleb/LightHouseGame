using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public CharacterController controller; // ������ �� ��������� CharacterController
    public float walkSpeed = 6f;    // �������� ������
    public float runSpeed = 12f;    // �������� ����
    public float slowRunSpeed = 8f; // �������� ���������� ���� (����� ������� � �����)
    public float gravity = -9.81f;  // ���� ����������

    public float smoothSpeedChange = 5f; // �������� ��������� �������� (��� ������ ��������, ��� ������� �����)

    private Vector3 velocity; // ������ �������� ���������
    private bool isGrounded;  // ���������� ��� ��������, ��������� �� �������� �� �����
    private CrouchController crouchController; // ������ �� ��������� CrouchController
    private StaminaController staminaController; // ������ �� ��������� StaminaController

    private float currentSpeed; // ������� �������� ���������
    private float targetSpeed;  // ������� ��������, � ������� �� ���������

    private bool isForcedSlowRun; // ���� ��������������� ���������� ����
    private bool isShiftPressed; // ���� ��� ������������ ��������� ��������� "Shift"

    void Awake()
    {
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        crouchController = GetComponent<CrouchController>();
        if (crouchController == null)
        {
            Debug.LogError("CrouchController �� ������ �� �������!");
        }

        staminaController = GetComponent<StaminaController>();
        if (staminaController == null)
        {
            Debug.LogError("StaminaController �� ������ �� �������!");
        }
    }

    void Update()
    {
        if (controller == null)
        {
            Debug.LogError("CharacterController �� ��������!");
            return;
        }

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // ���������� ������������ �������� ��� ������� �����
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // �������� ��������� "Shift"
        isShiftPressed = Input.GetKey(KeyCode.LeftShift);

        // ����������� ������� �������� � ����������� �� ��������� ���������
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
                    isForcedSlowRun = false; // ���������� ���� ��������������� ���������� ����
                }
                else
                {
                    targetSpeed = slowRunSpeed;
                    isForcedSlowRun = true; // ������������� ���� ��������������� ���������� ����
                }
            }
            else
            {
                if (isForcedSlowRun)
                {
                    // ���� ������� Shift �������� ����� ��������� ������������
                    isForcedSlowRun = false;
                }
                targetSpeed = walkSpeed;
                staminaController.StopRunning();
            }
        }

        // ������� ��������� ������� �������� � ������� ��������
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, smoothSpeedChange * Time.deltaTime);

        // ��������� ����������� ��������
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // ��������� ����������
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}