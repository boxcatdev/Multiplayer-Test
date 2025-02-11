using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] float effectDuration = 0.25f;

    private float timeElapsed;
    private bool startEffect = false;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        image.color = AdjustTransparency(Color.red, timeElapsed / effectDuration);
        timeElapsed = effectDuration;
    }
    private void Update()
    {
        if (startEffect)
        {
            timeElapsed -= Time.deltaTime;

            image.color = Color.red;
            image.color = AdjustTransparency(image.color, timeElapsed / effectDuration);

            if (timeElapsed <= 0)
            {
                startEffect = false;
                timeElapsed = effectDuration;
            }
        }
        else
        {
            if (image.color != Color.clear) image.color = Color.clear;
        }
    }

    public void DoDamageEffect()
    {
        startEffect = true;
    }

    private Color AdjustTransparency(Color color, float alphaValue)
    {
        return new Color(color.r, color.g, color.b, alphaValue);
    }
}
