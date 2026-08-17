using UnityEngine;

public class HealthBarSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    //Bar Size
    [SerializeField] private float width = 100f;
    [SerializeField] private float height = 10f;

    //References
    [SerializeField] private RectTransform healthBarFill;

    [SerializeField]
    private RectTransform healthBar;

    public void SetMaxHealth(float value)
    {
        maxHealth = Mathf.Max(1f, value);
    }

    public void SetHealth(float value)
    {
        if (healthBarFill == null)
        {
            Debug.LogWarning("HealthBarSystem: healthBarFill not assigned.");
            return;
        }

        float clamped = Mathf.Clamp(value, 0f, maxHealth);
        float percent = clamped / maxHealth;

        healthBarFill.sizeDelta = new Vector2(width * percent, height);
    }
}