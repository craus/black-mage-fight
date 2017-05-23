﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;

public class UI : MonoBehaviour {
    public static UI instance;
    public GameObject newGame;
    public GameObject customLevel;
    public GameObject intro;
    public GameObject difficultySelector;
    public GameObject profileSelector;
    public GameObject warning;
    public GameObject profileName;
    public AudioSource battleMusic;

    public HealthSlider heroHealthSlider;
    public HealthSlider blackMageHealthSlider;
    public HealthSlider gambochkaHealthSlider;

    public GameObject floatMessage;
    public Text floatMessageText;

    public GameObject loseMessage;
    public GameObject winMessage;


    public GameObject volumes;

    public List<Blur> blur;

    void Awake() {
        instance = this;
    }

    void Start() {
        CloseAll();
#if UNITY_EDITOR
        //intro.SetActive(true);
#else
        intro.SetActive(true);
#endif
    }

    public void AskName() {
        CloseAll();
        profileName.SetActive(true);
    }

    public void AskDifficulty() {
        CloseAll();
        difficultySelector.SetActive(true);
    }

    void Escape() {
        if (intro.activeSelf) {
            intro.SetActive(false);
            return;
        }
        if (newGame.activeSelf) {
            CloseAll();
            return;
        } 
        NewGame();
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Escape();
        }
        battleMusic.mute = newGame.activeSelf || intro.activeSelf || customLevel.activeSelf || GameManager.instance.GameOver();
    }

    public void CloseAll() {
        floatMessage.SetActive(false);
        newGame.SetActive(false);
        customLevel.SetActive(false);
        difficultySelector.SetActive(false);
        profileSelector.SetActive(false);

        profileName.SetActive(false);
        warning.SetActive(false);
        volumes.SetActive(false);
    }

    public void Volumes() {
        CloseAll();
        volumes.SetActive(true);
    }

    public void CustomLevel() {
        newGame.SetActive(false);
        customLevel.SetActive(true);
    }

    public void NewGame() {
        newGame.SetActive(true);
        customLevel.SetActive(false);
    }

    public void Win() {
        blur.ForEach(b => b.enabled = true);
        winMessage.SetActive(true);
    }

    public void Lose() {
        blur.ForEach(b => b.enabled = true);
        loseMessage.SetActive(true);
    }

    public void UpdateHUD() {
        blur.ForEach(b => b.enabled = false);
        winMessage.SetActive(false);
        loseMessage.SetActive(false);
        heroHealthSlider.unit = Hero.instance;
        blackMageHealthSlider.unit = BlackMage.instance;
        gambochkaHealthSlider.unit = Gambochka.instance;
    }

    public void ShowMessage(string message) {
        StopAllCoroutines();
        StartCoroutine(ShowMessageForTime(message));
    }

    IEnumerator ShowMessageForTime(string message) {
        floatMessage.SetActive(true);
        floatMessageText.text = message;
        yield return new WaitForSeconds(1);
        floatMessage.SetActive(false);
    }
}
