using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public float lerpSpeed = 2;
    public Slider health;
    public Slider h_vis;
    public Slider focus;
    public Slider f_vis;
    public Slider stamina;
    public Slider s_vis;

    public float sizeMultiplier = 2;

    private void Start()
    {
        InitHealth(100);
    }

    public void InitSlider(StatSlider t, int value)
    {
        Slider s = null;
        Slider v = null;

        switch(t)
        {
            case StatSlider.health:
                s = health;
                v = h_vis;
                break;
            case StatSlider.foucs:
                s = focus;
                v = f_vis;
                break;
            case StatSlider.stamina:
                s = stamina;
                v = s_vis;
                break;
            default:
                break;
        }

        s.maxValue = value;
        v.minValue = value;
        RectTransform r = s.GetComponent<RectTransform>();
        RectTransform r_v = v.GetComponent<RectTransform>();
        float value_actual = value * sizeMultiplier;
        value_actual = Mathf.Clamp(value_actual, 0, 1000);
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
        r_v.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
    }

    public void Tick(CharacterStats stats, float delta)
    {
        health.value = stats._health;
        focus.value = stats._focus;
        stamina.value = stats._stamina;

        h_vis.value = Mathf.Lerp(h_vis.value, stats._health, delta * lerpSpeed);
        f_vis.value = Mathf.Lerp(f_vis.value, stats._focus, delta * lerpSpeed);
        s_vis.value = Mathf.Lerp(s_vis.value, stats._stamina, delta * lerpSpeed);
    }

    public void InitHealth(int v)
    {
        RectTransform r = health.GetComponent<RectTransform>();
        r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v);
    }

    public void AffectAll(int h, int f, int s)
    {
        InitSlider(StatSlider.health, h);
        InitSlider(StatSlider.foucs, f);
        InitSlider(StatSlider.stamina, s);
    }
    public enum StatSlider
    { 
        health,foucs,stamina
    }

    public static UiManager singleton;
    private void Awake()
    {
        singleton = this;
    }
}
