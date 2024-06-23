using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    public float smoothTime = 0.1f;

    private float yaw;
    private float pitch;
    private Vector2 currentRotation;
    private Vector2 rotationSmoothVelocity;

    public float maxLeanAngle = 15f;
    public float maxLeanOffset = 0.5f;
    public float leanSpeed = 10f;
    public float leanOffsetSpeed = 10f;

    private float currentLeanOffset = 0f;
    private Vector3 initialCameraPosition;

    private bool qPressed = false;
    private bool ePressed = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerBody == null)
        {
            playerBody = transform;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }

        initialCameraPosition = playerCamera.localPosition;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        currentRotation = Vector2.SmoothDamp(currentRotation, new Vector2(pitch, yaw), ref rotationSmoothVelocity, smoothTime);

        playerBody.localRotation = Quaternion.Euler(0f, currentRotation.y, 0f);

        playerCamera.localRotation = Quaternion.Euler(currentRotation.x, 0f, playerCamera.localRotation.eulerAngles.z);

        // Обработка нажатий клавиш Q и E
        if (Input.GetKeyDown(KeyCode.Q))
        {
            qPressed = true;
            ePressed = false;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ePressed = true;
            qPressed = false;
        }
        else if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
        {
            qPressed = false;
            ePressed = false;
        }

        // Управление наклонами камеры
        if (qPressed)
        {
            LeanCamera(maxLeanAngle); // Наклон камеры влево
        }
        else if (ePressed)
        {
            LeanCamera(-maxLeanAngle); // Наклон камеры вправо
        }
        else
        {
            LeanCamera(0); // Возврат камеры в центральное положение
        }

        ApplyCameraOffset();
    }

    void LeanCamera(float targetLeanAngle)
    {
        float currentLeanAngle = Mathf.LerpAngle(playerCamera.localRotation.eulerAngles.z, targetLeanAngle, leanSpeed * Time.deltaTime);
        Vector3 leanRotation = playerCamera.localRotation.eulerAngles;
        leanRotation.z = currentLeanAngle;
        playerCamera.localRotation = Quaternion.Euler(leanRotation);

        float targetOffset = (targetLeanAngle / maxLeanAngle) * maxLeanOffset;
        currentLeanOffset = Mathf.Lerp(currentLeanOffset, -targetOffset, leanOffsetSpeed * Time.deltaTime);
    }

    void ApplyCameraOffset()
    {
        Vector3 cameraPosition = playerCamera.localPosition;
        cameraPosition.x = currentLeanOffset;

        if (Mathf.Abs(currentLeanOffset) < 0.0001f)
        {
            cameraPosition.x = initialCameraPosition.x;
        }

        playerCamera.localPosition = cameraPosition;
    }
}
