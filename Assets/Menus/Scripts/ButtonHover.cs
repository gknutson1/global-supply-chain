using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    TextMeshProUGUI text;
    Color baseColor;
    Color hoverColor = Color.white;

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        baseColor = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = baseColor;
    }
}
