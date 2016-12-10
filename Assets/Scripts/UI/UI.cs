﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI : MonoBehaviour {
    public static UI instance;
    public GameObject newGame;
    public GameObject customLevel;
    public GameObject intro;
    public AudioSource battleMusic;

    public HealthSlider heroHealthSlider;
    public HealthSlider blackMageHealthSlider;
    public HealthSlider gambochkaHealthSlider;

    void Awake() {
        instance = this;
    }

    void Start() {
        CloseMenu();
#if UNITY_EDITOR
#else
        intro.SetActive(true);
#endif
    }

    void Escape() {
        if (intro.activeSelf) {
            intro.SetActive(false);
            return;
        }
        if (newGame.activeSelf) {
            CloseMenu();
            return;
        } 
        NewGame();
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Escape();
        }
        battleMusic.mute = newGame.activeSelf || intro.activeSelf || customLevel.activeSelf;
    }

    public void CloseMenu() {
        newGame.SetActive(false);
        customLevel.SetActive(false);
    }

    public void CustomLevel() {
        newGame.SetActive(false);
        customLevel.SetActive(true);
    }

    public void NewGame() {
        newGame.SetActive(true);
        customLevel.SetActive(false);
    }

    public void UpdateHUD() {
        heroHealthSlider.unit = Hero.instance;
        blackMageHealthSlider.unit = BlackMage.instance;
        gambochkaHealthSlider.unit = Gambochka.instance;
    }
}