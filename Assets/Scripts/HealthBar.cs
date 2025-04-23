using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image HealthBarFill;

    public void UpdateHealthBar(float health)
    {
        HealthBarFill.fillAmount = health;
    }
}
