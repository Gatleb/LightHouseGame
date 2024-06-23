using UnityEngine;

public class StaminaController : MonoBehaviour
{
    [Tooltip("ћаксимальный уровень выносливости")]
    public float maxStamina = 100f;

    [Tooltip("—корость восстановлени€ выносливости в секунду")]
    public float staminaRecoveryRate = 10f;

    [Tooltip("—корость снижени€ выносливости при беге в секунду")]
    public float staminaDepletionRate = 20f;

    [Tooltip("«адержка перед восстановлением выносливости после полного истощени€, в секундах")]
    public float staminaRecoveryDelay = 5f;

    [Tooltip("—колько единиц выносливости восстанавливаетс€ сразу после задержки")]
    public float immediateRecoveryAmount = 60f;

    private float currentStamina;
    private float lastRunTime;
    private bool isRunning = false;
    private bool isShiftPressedWhileExhausted = false; // ‘лаг дл€ отслеживани€ состо€ни€ удержани€ "Shift" при истощенной выносливости

    void Start()
    {
        currentStamina = maxStamina; // ”станавливаем начальный уровень выносливости
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
                isShiftPressedWhileExhausted = true; // ”станавливаем флаг, если выносливость исчерпана
            }
        }
        else
        {
            // ≈сли прошло достаточно времени с момента последнего бега и Shift не удерживаетс€
            if (Time.time > lastRunTime + staminaRecoveryDelay && !Input.GetKey(KeyCode.LeftShift))
            {
                // ¬осстановление выносливости
                if (isShiftPressedWhileExhausted)
                {
                    // Ќемедленное восстановление части выносливости
                    currentStamina = Mathf.Clamp(currentStamina + immediateRecoveryAmount, 0, maxStamina);
                    isShiftPressedWhileExhausted = false; // —брасываем флаг
                }

                // ѕродолжение восстановлени€ выносливости
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }
        }

        // ƒл€ тестировани€ выносливости выводим еЄ значение в консоль
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