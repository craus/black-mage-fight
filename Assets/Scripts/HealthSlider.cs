﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HealthSlider : MonoBehaviour
{
    public Unit unit;
    public Slider slider;

    void Update() {
        slider.value = unit.health;
        slider.maxValue = unit.maxHealth;
    }
}
