using UnityEngine;

public class StaminaController : MonoBehaviour
{
    [Tooltip("������������ ������� ������������")]
    public float maxStamina = 100f;

    [Tooltip("�������� �������������� ������������ � �������")]
    public float staminaRecoveryRate = 10f;

    [Tooltip("�������� �������� ������������ ��� ���� � �������")]
    public float staminaDepletionRate = 20f;

    [Tooltip("�������� ����� ��������������� ������������ ����� ������� ���������, � ��������")]
    public float staminaRecoveryDelay = 5f;

    [Tooltip("������� ������ ������������ ����������������� ����� ����� ��������")]
    public float immediateRecoveryAmount = 60f;

    private float currentStamina;
    private float lastRunTime;
    private bool isRunning = false;
    private bool isShiftPressedWhileExhausted = false; // ���� ��� ������������ ��������� ��������� "Shift" ��� ���������� ������������

    void Start()
    {
        currentStamina = maxStamina; // ������������� ��������� ������� ������������
    }

    void Update()
    {
        if (isRunning)
        {
            currentStamina -= staminaDepletionRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            if (currentStamina <= 0)
            {
                lastRunTime = Time.time;
                isShiftPressedWhileExhausted = true; // ������������� ����, ���� ������������ ���������
            }
        }
        else
        {
            // ���� ������ ���������� ������� � ������� ���������� ���� � Shift �� ������������
            if (Time.time > lastRunTime + staminaRecoveryDelay && !Input.GetKey(KeyCode.LeftShift))
            {
                // �������������� ������������
                if (isShiftPressedWhileExhausted)
                {
                    // ����������� �������������� ����� ������������
                    currentStamina = Mathf.Clamp(currentStamina + immediateRecoveryAmount, 0, maxStamina);
                    isShiftPressedWhileExhausted = false; // ���������� ����
                }

                // ����������� �������������� ������������
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }
        }

        // ��� ������������ ������������ ������� � �������� � �������
        Debug.Log("Current Stamina: " + currentStamina);
    }

    public void StartRunning()
    {
        isRunning = true;
    }

    public void StopRunning()
    {
        isRunning = false;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public bool IsStaminaDepleted()
    {
        return currentStamina <= 0;
    }
}