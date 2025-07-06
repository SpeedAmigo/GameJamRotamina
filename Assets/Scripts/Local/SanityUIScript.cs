using UnityEngine;
using UnityEngine.UI;

public class SanityUIScript : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
    }

    public void SetCurrentValue(float value)
    {
        slider.value = value;
    }
}
