using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionSlider : MonoBehaviour
{
    public AudioMixer audioMixer;
    Slider slider;
    string settingName;
    public float defaultValue;

    void Start()
    {
        slider = GetComponent<Slider>();
        settingName = transform.parent.name;
        var value = PlayerPrefs.GetFloat(settingName, defaultValue);
        slider.SetValueWithoutNotify(value);
        if (value > 0) audioMixer.SetFloat(settingName, Mathf.Log10(value) * 40);
        else audioMixer.SetFloat(settingName, -80);
    }

    public void SaveSetting()
    {
        var value = slider.value;
        PlayerPrefs.SetFloat(transform.parent.name, value);
        if (value > 0) audioMixer.SetFloat(settingName, Mathf.Log10(value) * 40);
        else audioMixer.SetFloat(settingName, -80);
    }
}
