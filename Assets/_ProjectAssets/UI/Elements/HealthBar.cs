using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : VisualElement
{
    private VisualElement healthFill;
    private float maxHealth;
    private float currentHealth;

    public HealthBar()
    {
        maxHealth = 100;
        currentHealth = maxHealth;

        // Setup health bar structure
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.width = new Length(100, LengthUnit.Percent);
        container.style.height = 10;
        container.style.backgroundColor = Color.gray;

        healthFill = new VisualElement();
        healthFill.style.height = new Length(100, LengthUnit.Percent);
        healthFill.style.backgroundColor = Color.red;

        container.Add(healthFill);
        Add(container);
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        float healthPercent = currentHealth / maxHealth;
        healthFill.style.width = new Length(healthPercent * 100, LengthUnit.Percent);
    }
}