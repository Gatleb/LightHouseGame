using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class DayAndNightCykle : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0f, 24f)]
    public float currentTime;
    public float timeSpeed = 1f;

    [Header("CurrentTime")]
    public string currentTimeString;

    [Header("Sun Settings")]
    public Light sunLight;
    public float sunPosition = 1f;
    public float sunIntensity = 1f;
    public AnimationCurve sunIntensityMultiplier;
    public AnimationCurve sunTemperatureCurve;


    public bool isDay = true;
    public bool sunActive = true;
    public bool moonActive = true;

    [Header("Moon Settings")]
    public Light moonLight;
    public float moonIntensity = 1f;
    public AnimationCurve moonIntensityMultiplier;
    public AnimationCurve moonTemperatureCurve;
    void Start()
    {
        UpdateTimeText();
        CheckShadowStatus();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime * timeSpeed;

        if (currentTime >= 24)
        { 
            currentTime = 0;
        }

        UpdateTimeText();
        UpdateLight();
        CheckShadowStatus();
    }

    private void OnValidate()
    { 
        UpdateLight();
    }
    void UpdateTimeText()
    {
        currentTimeString = Mathf.Floor(currentTime).ToString("00") + ":" + ((currentTime % 1) * 60).ToString("00");
    }

    void UpdateLight()
    {
        float sunRotation = currentTime / 24f * 360f;
        sunLight.transform.rotation = Quaternion.Euler(sunRotation - 90f, sunPosition, 0f);
        moonLight.transform.rotation = Quaternion.Euler(sunRotation + 90f, sunPosition, 0f);


        float normalizedTime = currentTime / 24f;
        float sunIntensityCurve = sunIntensityMultiplier.Evaluate(normalizedTime);
        float moonIntensityCurve = moonIntensityMultiplier.Evaluate(normalizedTime);

        HDAdditionalLightData sunLightData = sunLight.GetComponent<HDAdditionalLightData>();
        HDAdditionalLightData moonLightData = moonLight.GetComponent<HDAdditionalLightData>();

        if (sunLightData != null )
        {
            sunLightData.intensity = sunIntensityCurve * sunIntensity;
        }

        if (moonLightData != null)
        {
            moonLightData.intensity = moonIntensityCurve * moonIntensity;
        }

        float sunTemperatureMultiplier = sunTemperatureCurve.Evaluate(normalizedTime);
        float moonTemperatureMultiplier = moonTemperatureCurve.Evaluate(normalizedTime);
        Light sunLightComponent = sunLight.GetComponent<Light>();
        Light moonLightComponent = moonLight.GetComponent<Light>();

        if (sunLightComponent != null)
        { 
           sunLightComponent.colorTemperature = sunTemperatureMultiplier * 10000f;
        }

        if (moonLightComponent != null)
        {
            moonLightComponent.colorTemperature = moonTemperatureMultiplier * 10000f;
        }
    }

    void CheckShadowStatus()
    {
        HDAdditionalLightData sunLightData = sunLight.GetComponent<HDAdditionalLightData>();
        HDAdditionalLightData moonLightData = moonLight.GetComponent<HDAdditionalLightData>();
        float currentSunRotation = currentTime;
        if (currentSunRotation >= 6f && currentSunRotation <= 18f)
        {
            sunLightData.EnableShadows(true);
            moonLightData.EnableShadows(false);
            isDay = true;
        }

        else 
        {
            sunLightData.EnableShadows(false);
            moonLightData.EnableShadows(true);
            isDay = false;
        }

        if (currentSunRotation >= 5.7f && currentSunRotation <= 18.3f)
        {
            sunLight.gameObject.SetActive(true);
            sunActive = true;
        }

        else
        { 
            sunLight.gameObject.SetActive(false);
            sunActive = false;
        }

        if (currentSunRotation >= 6.3f && currentSunRotation <= 17.7f)
        {
            moonLight.gameObject.SetActive(false);
            moonActive = false;
        }

        else
        {
            moonLight.gameObject.SetActive(true);
            moonActive = true;
        }
    }
}
