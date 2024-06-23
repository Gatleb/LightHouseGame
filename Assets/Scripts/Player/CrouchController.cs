using UnityEngine;

public class CrouchController : MonoBehaviour
{
    public Transform playerCamera;

    public float crouchHeight = 0.5f;  // ������ ������ � �������
    public float standHeight = 0.545f;   // ������ ������ � ������� ���������
    public float idleCrouchHeight = 0.3f; // �������������� ������ ���������� ����� �����������
    public float crouchTransitionSpeed = 0.1f; // �������� �������� � ������

    [Tooltip("�������� ������������ � �������")]
    public float crouchMovementSpeed = 3f; // �������� ������������ � �������

    public float crouchSpeedMultiplier = 1.0f; // ��������� �������� ��� �������

    private bool isCrouching = false;
    private float idleTimer = 0f;
    private float idleTimeThreshold = 0.1f; // ����� ������� ����������� ��� ��������� ��������������� ����������
    private float heightSmoothVelocity;    // ���������� ��� �������� ��������� ������ ������

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>().transform;
            if (playerCamera == null)
            {
                Debug.LogError("������ ������ �� �������!");
            }
        }

        // ������������� ��������� ������ ������
        playerCamera.localPosition = new Vector3(0f, standHeight, 0f);
    }

    void Update()
    {
        if (playerCamera == null)
        {
            Debug.LogError("������ ������ �� ���������!");
            return;
        }

        // ��������� ��������� ����������
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            ResetIdleTimer(); // ���������� ������ ����������� ��� ������ ������� C
        }

        // ������ ������ ��� ���������� � �������
        float targetHeight = isCrouching ? (idleTimer >= idleTimeThreshold ? (crouchHeight - idleCrouchHeight) : crouchHeight) : standHeight;

        // ���������� SmoothDamp ��� �������� ��������� ������ ������
        float currentHeight = playerCamera.localPosition.y;
        float smoothHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightSmoothVelocity, crouchTransitionSpeed);

        playerCamera.localPosition = new Vector3(
            playerCamera.localPosition.x,
            smoothHeight,
            playerCamera.localPosition.z
        );

        // ��������� ������ �����������
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

    // ���������� ������� ��������� ����������
    public bool IsCrouching()
    {
        return isCrouching;
    }

    // ���������� �������� ������������ � �������
    public float GetCrouchMovementSpeed()
    {
        return crouchMovementSpeed * crouchSpeedMultiplier;
    }
}