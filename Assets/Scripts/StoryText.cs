using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StoryText : MonoBehaviour
{
    public TMP_FontAsset dyslexiaFriendlyFont;
    TMP_FontAsset defaultFont;
    bool currentDyslexiaFriendlyFontSetting = false;
    bool currentAdhdFriendlyTextSetting = false;

    PersistentVariables persistentVariables;
    TextMeshProUGUI textComponent;
    int level;
    int line = -1;
    bool isAnimating = false;
    bool isPaused = false;
    Coroutine coroutine;


    readonly List<List<string>> storyText = new() {
        // Chapter 0 text
        new() {
            "Welcome to the Navy, Lieutenant. Are you ready to defend the world's oceans from hostile forces?",
            "Congrats, your ship is a corvette-class: nimble yet powerful. Perfect for hunting pirates in these waters.",
            
        },
        //Chapter 1
        new() {
            "Chapter 1: Strait of Hormuz", 
            "This vital oil route winds to a narrow strip as the Persian Gulf and the Arabian Sea converge.",
            "Warning: Hostiles seen in these waters. "
        }
        //Chapter 2
        new() {
            "Chapter 2: Strait of Malacca",
            "Commander, the narrow chokepoints through the strait cause many captains to slow down ",
            "which makes their ships easy targets for pirates. "
        }
    };

    void Start()
    {
        persistentVariables = FindAnyObjectByType<PersistentVariables>();
        level = persistentVariables.level;
        textComponent = GetComponent<TextMeshProUGUI>();
        UpdateTextSettings();
        DisplayNextLine();
    }

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !isPaused)
            if (isAnimating)
                SkipAnimation();
            else if (line < storyText[level].Count - 1)
                DisplayNextLine();
            else
                SceneManager.LoadScene(2);
    }

    public void Pause(bool pause)
    {
        isPaused = pause;
    }

    void DisplayNextLine()
    {
        textComponent.text = string.Empty;
        line++;
        coroutine = StartCoroutine(AnimateText());
    }

    List<string> FormatStoryTextForAnimation(string text)
    {
        var storyText = new List<string>();

        foreach (var character in text)
            storyText.Add($"{character}");

        if (currentAdhdFriendlyTextSetting)
        {
            var regex = new Regex("[A-Za-z]+(?:'[A-Za-z]+)*");
            foreach (Match match in regex.Matches(text))
                for (var i = match.Index; i < match.Index + Math.Ceiling((float)match.Length / 2); i++)
                    storyText[i] = $"<b>{storyText[i]}</b>";
        }

        return storyText;
    }

    IEnumerator AnimateText()
    {
        isAnimating = true;
        var formattedText = FormatStoryTextForAnimation(storyText[level][line]);
        foreach (var character in formattedText)
        {
            textComponent.text += character;
            do
                yield return new WaitForSeconds(0.05f);
            while (isPaused);
        }
        isAnimating = false;
    }

    void SkipAnimation()
    {
        StopCoroutine(coroutine);
        isAnimating = false;
        textComponent.text = currentAdhdFriendlyTextSetting ? generateAdhdFriendlyText(storyText[level][line]) : storyText[level][line];
    }

    void UpdateTextSettings()
    {
        var newDyslexiaFriendlyFontSetting = PlayerPrefs.GetInt("Dyxlexia-friendly Font", currentDyslexiaFriendlyFontSetting ? 1 : 0) == 1;
        if (newDyslexiaFriendlyFontSetting != currentDyslexiaFriendlyFontSetting)
            setDyslexiaFriendlyFontSetting(newDyslexiaFriendlyFontSetting);

        var newAdhdFriendlyFontSetting = PlayerPrefs.GetInt("ADHD-friendly Text", currentAdhdFriendlyTextSetting ? 1 : 0) == 1;
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
        currentAdhdFriendlyTextSetting = isEnabled;

        if (isAnimating)
        {
            StopCoroutine(coroutine);
            line--;
            DisplayNextLine();
        }
        else if(line >= 0)
        {
            textComponent.text = isEnabled ? generateAdhdFriendlyText(storyText[level][line]) : storyText[level][line];
        }
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
