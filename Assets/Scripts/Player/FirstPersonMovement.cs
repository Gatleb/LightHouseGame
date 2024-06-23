using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public CharacterController controller; // ������ �� ��������� CharacterController
    public float walkSpeed = 6f;    // �������� ������
    public float runSpeed = 12f;    // �������� ����
    public float gravity = -9.81f;  // ���� ����������

    private Vector3 velocity; // ������ �������� ���������
    private bool isGrounded;  // ���������� ��� ��������, ��������� �� �������� �� �����
    private CrouchController crouchController; // ������ �� ��������� CrouchController

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

        float currentSpeed;
        if (crouchController != null && crouchController.IsCrouching())
        {
            currentSpeed = crouchController.GetCrouchMovementSpeed();
        }
        else
        {
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        }

        // ��������� ����������� ��������
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // ��������� ����������
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}