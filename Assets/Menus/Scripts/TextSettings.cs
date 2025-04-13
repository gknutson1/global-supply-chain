using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TextSettings : MonoBehaviour
{
    public TMP_FontAsset dyslexiaFriendlyFont;
    public bool alwaysAdhdFriendly;

    TextMeshProUGUI textComponent;
    string defaultText;
    TMP_FontAsset defaultFont;
    bool defaultIsBold;

    bool currentDyslexiaFriendlyFontSetting = false;
    bool currentAdhdFriendlyTextSetting = false;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        defaultText = textComponent.text;
        defaultFont = textComponent.font;
        defaultIsBold = (textComponent.fontStyle & FontStyles.Bold) == FontStyles.Bold;

        UpdateTextSettings();
    }

    public void ReinitializeDefaultText()
    {
        defaultText = textComponent.text;
        currentAdhdFriendlyTextSetting = false;

        UpdateTextSettings();
    }

    void UpdateTextSettings()
    {
        var newDyslexiaFriendlyFontSetting = PlayerPrefs.GetInt("Dyxlexia-friendly Font", currentDyslexiaFriendlyFontSetting ? 1 : 0) == 1;
        if (newDyslexiaFriendlyFontSetting != currentDyslexiaFriendlyFontSetting)
            setDyslexiaFriendlyFontSetting(newDyslexiaFriendlyFontSetting);

        var newAdhdFriendlyFontSetting = PlayerPrefs.GetInt("ADHD-friendly Text", currentAdhdFriendlyTextSetting ? 1 : 0) == 1 | alwaysAdhdFriendly;
        if (newAdhdFriendlyFontSetting != currentAdhdFriendlyTextSetting)
            setAdhdFriendlyTextSetting(newAdhdFriendlyFontSetting);
    }

    void setDyslexiaFriendlyFontSetting(bool isEnabled)
    {
        textComponent.font = isEnabled ? dyslexiaFriendlyFont : defaultFont;
        currentDyslexiaFriendlyFontSetting = isEnabled;
    }

    void setAdhdFriendlyTextSetting(bool isEnabled)
    {
        textComponent.text = isEnabled ? generateAdhdFriendlyText(textComponent.text) : defaultText;
        textComponent.fontStyle ^= defaultIsBold ? FontStyles.Bold : FontStyles.Normal;
        currentAdhdFriendlyTextSetting = isEnabled;
    }

    string generateAdhdFriendlyText(string text)
    {
        var regex = new Regex("[A-Za-z]+(?:'[A-Za-z]+)*");
        var adhdFriendlyText = string.Empty;
        var currentIndex = 0;
        foreach (Match match in regex.Matches(text))
        {
            adhdFriendlyText += text.Substring(currentIndex, match.Index - currentIndex)
                + "<b>"
                + match.Value[..(int)Math.Ceiling((float)match.Length / 2)]
                + "</b>"
                + match.Value[(int)Math.Ceiling((float)match.Length / 2)..];
            currentIndex = match.Index + match.Length;
        }
        adhdFriendlyText += text[currentIndex..];
        return adhdFriendlyText;
    }
}
