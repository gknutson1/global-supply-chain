using TMPro;
using UnityEngine;

public class OptionDropdown : MonoBehaviour
{
    public int defaultValue;

    void Start()
    {
        GetComponent<TMP_Dropdown>().SetValueWithoutNotify(PlayerPrefs.GetInt(transform.parent.name, defaultValue));
    }

    public void SaveSetting()
    {
        PlayerPrefs.SetInt(transform.parent.name, GetComponent<TMP_Dropdown>().value);
    }
}
