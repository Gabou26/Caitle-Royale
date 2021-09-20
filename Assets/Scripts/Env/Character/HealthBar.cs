using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{

    public Slider slider;
    [SerializeField] private Color fullHealthColor = new Color(0.16f, 0.6f, 0.25f);
    [SerializeField] private Color lowHealthColor = new Color(0.67f, 0.24f, 0.21f);
    [SerializeField] private GameObject fillBar;

    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetMaxHealth(int maxHealth, float health)
    {
        if (rect == null)
            return;
        rect.sizeDelta = new Vector2(1175 + (maxHealth / 3), rect.sizeDelta.y);
        slider.maxValue = maxHealth;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        fillBar.GetComponent<Image>().color = Color.Lerp(lowHealthColor, fullHealthColor, slider.value / slider.maxValue);
    }
}
