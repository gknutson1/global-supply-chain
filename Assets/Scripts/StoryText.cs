using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class StoryText : MonoBehaviour
{
    GameManager gameManager;
    TextMeshProUGUI textComponent;
    int level = 0;
    int line = -1;
    bool isAnimating = false;
    Coroutine coroutine;

    readonly List<List<string>> storyText = new() {
        // Chapter 0 text
        new() {
            "This is one line of text for Chapter 0.",
            "Here's a second line of text for chapter 0."
        },
        //Chapter 1
        new() {
            "Chapter 1"
        }
    };

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        DisplayNextLine();
    }

    void DisplayNextLine()
    {
        textComponent.text = string.Empty;
        line++;
        coroutine = StartCoroutine(AnimateText());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            if (isAnimating)
                SkipAnimation();
            else if (line < storyText[level].Count - 1)
                DisplayNextLine();
    }

    List<string> FormatStoryTextForAnimation(string text)
    {
        var storyText = new List<string>();

        foreach (var character in text)
            storyText.Add($"{character}");

        if (PlayerPrefs.GetInt("ADHD-friendly Text", 0) == 1)
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
            yield return new WaitForSeconds(0.05f);
        }
        isAnimating = false;
    }

    void SkipAnimation()
    {
        StopCoroutine(coroutine);
        isAnimating = false;
        textComponent.text = storyText[level][line];
    }
}
