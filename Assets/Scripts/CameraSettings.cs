using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
                 
public class CameraSettings : MonoBehaviour
{
    public TMPro.TextMeshProUGUI exposureValueText;
    public Slider exposureValueSlider;

    private float minExposureTargetBias;
    private float maxExposureTargetBias;

#if UNITY_IPHONE
    [DllImport ("__Internal")]
    private static extern void _CameraDeviceInitialize();
    [DllImport ("__Internal")]
    private static extern float _CameraDeviceGetMinExposureTargetBias();
    [DllImport ("__Internal")]
    private static extern float _CameraDeviceGetMaxExposureTargetBias();
    [DllImport ("__Internal")]
    private static extern float _CameraDeviceGetExposureTargetBias();
    [DllImport ("__Internal")]
    private static extern void _CameraDeviceSetExposureTargetBias(float bias);
#endif

    void Start()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        _CameraDeviceInitialize();

        minExposureTargetBias = _CameraDeviceGetMinExposureTargetBias();
        maxExposureTargetBias = _CameraDeviceGetMaxExposureTargetBias();
#else
        minExposureTargetBias = 0.0f;
        maxExposureTargetBias = 1.0f;
#endif

        exposureValueSlider.minValue = minExposureTargetBias;
        exposureValueSlider.maxValue = maxExposureTargetBias;

#if UNITY_IPHONE && !UNITY_EDITOR
        float exposureTargetBias = _CameraDeviceGetExposureTargetBias();
        exposureValueSlider.value = exposureTargetBias;
#else
        exposureValueSlider.value = 0.0f;
#endif

        exposureValueSlider.onValueChanged.AddListener(delegate { SliderValueChangeCheck(); });
        exposureValueText.SetText("Exposure: " + exposureValueSlider.value.ToString());
    }

    public void SliderValueChangeCheck()
    {
        if (exposureValueSlider.value >= minExposureTargetBias
            && exposureValueSlider.value <= maxExposureTargetBias)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            _CameraDeviceSetExposureTargetBias(exposureValueSlider.value);
#endif
            exposureValueText.SetText("Exposure: " + exposureValueSlider.value.ToString());

            Debug.Log("Changed slider value to: " + exposureValueSlider.value);
        }
        else
        {
            Debug.LogError("Slider tried to set invalid value: " + exposureValueSlider.value);
        }

    }
}