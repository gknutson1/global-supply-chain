using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public List<Canvas> canvases;

    public void UpdateTextSettings()
    {
        foreach (var canvas in canvases)
            canvas.BroadcastMessage("UpdateTextSettings");
    }
}
