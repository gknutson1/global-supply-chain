using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            "Welcome to the Navy, Ensign. Are you ready to defend the world's oceans from hostile forces?",
            "Congrats, your ship is a corvette-class: nimble yet powerful. Perfect for hunting pirates in these waters.",
            "Let's get oriented with the controls first. Click the ship to select. See the green selection ring. Great!",
            "Now let's get this ship moving. Left-click and drag the mouse to select a destination Lieutenant. Right-Click to set it. ",
            "Ship speed, and turning are upgradeable with experience points. Fire power and accuracy can also be upgraded.",
            "The green health bar beneath the ship indicates current ship health points.",
            "Ships automatically target enemies within range. To fire, navigate toward the enemy ship. Once in range, your crew will start firing.",
             // to do continue tutorial
            "Global Supply Chain is a real time naval strategy game. To win you must outmaneuver your opponents. Protect your crew and survive.",
            "Protect your fleet and navigate while taking fire from enemies big and small.",
            "Rank up to unlock new ships and missions as you face new enemies throughout the world",
            "Survive and rule the supply chain",
            
        },
        //Chapter 1
        new() {
            "Chapter 1: Strait of Hormuz", 
            "This vital oil route winds to a narrow strip as the Persian Gulf and the Arabian Sea converge.",
            "Warning: Hostiles seen in these waters.",
            "Use your superior fire-power to secure the area for commercial traffic."
        },
        //Chapter 2
        new() {
            "Chapter 2: Strait of Malacca",
            "Congratulations on your promotion, Junior Lieutenant, the narrow chokepoints through the strait cause many captains to slow down ",
            "making their ships easy targets for pirates. Be prepared for combat. These aren't the pirates of the Caribbean. "
        },
         //Chapter 3
        new() {
            "Chapter 3: Taiwan Strait",
            "Lieutenant, drills often occur in this area. Only 110 miles separate the island from the mainland.", 
            "Military patrols and unfriendly coast guard and fisherman are a common sight in these waters. "
        },
        //Chapter 4
        new() {
            "Chapter 4:  Islands of the South Pacific",
             "Welcome to Guam Commander. The beaches have been fortified in case of invasion, no holiday for your crew unfortunately.",
              "The enemy is attempting to blockade the island and we must break through with our remaining fleet. Your acumen for battle will decide who controls the Pacific.",
        },
         //Chapter 5
        new() {
            "Chapter 5: Strait of Magellan",
             "Sir, we are passing by Cape Horn on the port side of the vessel. We should have easy passage as long as the winds cooperate.",
              "A Captain of the carrier strike group is quite an achievement. Let's hope the Atlantic is more friendly to us. ",
              "Spoke too soon. Looks like someone was waiting for our group. Arms ready!"
        },
         //Chapter 6
        new() {
            "Chapter 6: Northern Sea Route",
            "Admiral, what is a nuclear powered attack submarine doing in Greenland? Nevermind, it's probably classified...",
            "We shouldn't encouter much ice this time of year but I'm sure we won't be alone once we enter the disputed economic zone.",
            "There have been reports of fighter jets attacking ships. Be decisive, hate to get a scratch on this billion dollar submarine."
        },

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
                SceneManager.LoadScene($"Level{level}");
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
