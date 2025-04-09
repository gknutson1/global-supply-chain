using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class StoryText : MonoBehaviour
{
    void Start()
    {
        var storyText = generateStoryText("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla elementum ante at elit aliquam gravida. Ut id elit lacus. Etiam malesuada ante in venenatis venenatis. Ut feugiat tincidunt magna vitae vehicula. Fusce id arcu sapien. Maecenas ante velit, dignissim non odio nec, consequat accumsan est.");
        var coroutine = StartCoroutine(AnimateStoryText(storyText));
    }

    List<string> generateStoryText(string text)
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

    IEnumerator AnimateStoryText(List<string> text)
    {
        var textComponent = GetComponent<TextMeshProUGUI>();
        foreach (var character in text)
        {
            textComponent.text += character;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
