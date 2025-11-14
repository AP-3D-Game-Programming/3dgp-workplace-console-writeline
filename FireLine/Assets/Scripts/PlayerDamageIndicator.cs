using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageIndicator : MonoBehaviour
{
    public Image damageOverlay;
    public float flashAlpha = 0.6f;   // Hoe rood het wordt
    public float fadeSpeed = 2f;      // Hoe snel het wegfade

    private float currentAlpha = 0f;

    void Update()
    {
        if (currentAlpha > 0f)
        {
            currentAlpha -= Time.deltaTime * fadeSpeed;
            currentAlpha = Mathf.Clamp01(currentAlpha);

            damageOverlay.color = new Color(1f, 0f, 0f, currentAlpha);
        }
    }

    public void ShowDamage()
    {
        currentAlpha = flashAlpha;
        damageOverlay.color = new Color(1f, 0f, 0f, currentAlpha);
    }
}
