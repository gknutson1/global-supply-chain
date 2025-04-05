using UnityEngine;
using UnityEngine.UI;

public class OptionToggle : MonoBehaviour
{
    public bool defaultValue;

    void Start()
    {
        GetComponent<Toggle>().SetIsOnWithoutNotify(PlayerPrefs.GetInt(transform.parent.name, defaultValue ? 1 : 0) == 1);
    }

    public void SaveSetting()
    {
        PlayerPrefs.SetInt(transform.parent.name, GetComponent<Toggle>().isOn ? 1 : 0);
    }
}
