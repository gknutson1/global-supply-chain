using UnityEngine;
using UnityEngine.UI;

public class OptionSlider : MonoBehaviour
{
    public float defaultValue;

    void Start()
    {
        GetComponent<Slider>().SetValueWithoutNotify(PlayerPrefs.GetFloat(transform.parent.name, defaultValue));
    }

    public void SaveSetting()
    {
        PlayerPrefs.SetFloat(transform.parent.name, GetComponent<Slider>().value);
    }
}
