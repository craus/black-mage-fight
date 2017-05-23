﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour {
    const string GAMESTATE_FILE = "gamestate.dat";
    const int MAX_HISTORY = 1000;

    public static GameManager instance;

    public GameState gameState;

    GameObject lastLevel;
    GameObject currentLevel;

    public AudioSource loseSound;
    public AudioSource winSound;

    public event Action<Unit, Cell, Cell, IntVector2> onHeroMove = (h, a, b, d) => { };

    public List<Material> portalMaterials;

    public int wins;
    public int losses;

    public void Win() {
        ++wins;
        this.TryPlay(winSound);
        UI.instance.Win();
    }

    public void Lose() {
        ++losses;
        this.TryPlay(loseSound);
        UI.instance.Lose();
    }

    public bool GameOver() {
        return !LevelIsRunning();
    }

    public void DropSaveFile() {
        gameState = new GameState();
        Save();
        UpdateState();
    }

    void Awake() {
        instance = this;
    }

    public void Load() {
        gameState = FileManager.LoadFromFile<GameState>(GAMESTATE_FILE);
        if (gameState != null) {

        } else {
            gameState = new GameState();
        }
    }

    public void UpdateState() {
        if (gameState.CurrentProfile.name == "") {
            UI.instance.AskName();
            return;
        }
        if (gameState.CurrentProfile.currentRuns.Count == 0) {
            UI.instance.AskDifficulty();
            return;
        }
    }

    void Start() {
        Load();
        UpdateState();
    }
    
    public void Clear() {
        FindObjectsOfType<Token>().ForEach(x => {
            x.gameObject.SetActive(false);
            Destroy(x.gameObject);
        });
        if (currentLevel != null) {
            currentLevel.SetActive(false);
            Destroy(currentLevel);
        }
    }

    public void ResetPositions() {
        FindObjectsOfType<Figure>().ForEach(f => f.SetPosition(null));
        FindObjectsOfType<Figure>().ForEach(f => f.Blink());
    }

    public void NewGame(GameObject level) {
        lastLevel = level;
        Clear();
        currentLevel = Instantiate(level);
        currentLevel.SetActive(true);
        Controls.instance.activeUnit = Hero.instance;
        UI.instance.CloseAll();
        UI.instance.UpdateHUD();

        Board.instance.Restore();

        FindObjectsOfType<Figure>().ForEach(f => {
            if (f.Position == null) {
                f.Blink();
            }
        });


        FindObjectsOfType<OnLevelStart>().ForEach(t => t.Run());
    }

    public void HeroMoved(Unit hero, Cell from, Cell to, IntVector2 direction) {
        onHeroMove(hero, from, to, direction);
    }

    public void Restart() {
        NewGame(lastLevel);
    }

    public void Save() {
        FileManager.SaveToFile(gameState, GAMESTATE_FILE);
    }

    void OnDestroy() {
        Save();
    }

    bool LevelIsRunning() {
        return Hero.instance != null && BlackMage.instance != null;
    }

    void Update() {
        if (LevelIsRunning()) {
            if (BlackMage.instance.Dead) {
                Destroy(BlackMage.instance.gameObject);
                Win();
            } else if (Hero.instance.Dead) {
                Debug.LogFormat("Destroying hero: {0}", Hero.instance.transform.Path());
                Destroy(Hero.instance.gameObject);
                Lose();
            }
        }
    }
}
