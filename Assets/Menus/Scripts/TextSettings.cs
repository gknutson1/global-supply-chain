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
        print($"Start: {textComponent.text}");
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
        print($"Updating dyslexia font settings: {textComponent.text} - {currentDyslexiaFriendlyFontSetting}, {newDyslexiaFriendlyFontSetting}");
        if (newDyslexiaFriendlyFontSetting != currentDyslexiaFriendlyFontSetting)
            setDyslexiaFriendlyFontSetting(newDyslexiaFriendlyFontSetting);

        var newAdhdFriendlyFontSetting = PlayerPrefs.GetInt("ADHD-friendly Text", currentAdhdFriendlyTextSetting ? 1 : 0) == 1 | alwaysAdhdFriendly;
        if (newAdhdFriendlyFontSetting != currentAdhdFriendlyTextSetting)
            setAdhdFriendlyTextSetting(newAdhdFriendlyFontSetting);
    }

    void setDyslexiaFriendlyFontSetting(bool isEnabled)
    {
        print($"Setting dyslexia friendly font: {textComponent.text}");
        textComponent.font = isEnabled ? dyslexiaFriendlyFont : defaultFont;
        currentDyslexiaFriendlyFontSetting = isEnabled;
    }

    void setAdhdFriendlyTextSetting(bool isEnabled)
    {
        if (isEnabled)
        {
            var regex = new Regex("[A-Za-z]+");
            var adhdFriendlyText = string.Empty;
            var currentIndex = 0;
            foreach (Match match in regex.Matches(textComponent.text))
            {
                adhdFriendlyText += textComponent.text.Substring(currentIndex, match.Index - currentIndex)
                    + "<b>"
                    + match.Value.Substring(0, (int)Math.Ceiling((float)match.Length / 2))
                    + "</b>"
                    + match.Value.Substring((int)Math.Ceiling((float)match.Length / 2));
                currentIndex = match.Index + match.Length;
            }
            adhdFriendlyText += textComponent.text.Substring(currentIndex);
            textComponent.text = adhdFriendlyText;
        }
        else
            textComponent.text = defaultText;

        if (defaultIsBold)
            textComponent.fontStyle ^= FontStyles.Bold;

        currentAdhdFriendlyTextSetting = isEnabled;
    }
}
