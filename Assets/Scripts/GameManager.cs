using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using RSG;

public class GameManager : Singletone<GameManager> {
    const string GAMESTATE_FILE = "gamestate.dat";
    const int MAX_HISTORY = 1000;

    public GameObject startLevel;

    public GameState gameState;

    public Level lastLevel;
    public Level currentLevel;



    public AudioSource loseSound;

	public AudioSource winSound;

    public Intermission ending;
    public Intermission badEnding;

    public event Action<Unit, Cell, Cell, IntVector2> onHeroMove = (h, a, b, d) => { };

    public List<Material> portalMaterials;

    public int wins;
    public int losses;

	public bool levelIsRunning;
	public LevelRun currentLevelRun;

    public void Win() {
		BlackMage.instance.health = 0;
		BlackMage.instance.gameObject.SetActive(false);
        ++wins;
        this.TryPlay(winSound);
        UI.instance.Win();
		Debug.LogFormat("Win");
        gameState.CurrentRun.levelsCompleted++;
		levelIsRunning = false;
		UpdateLevelRun();
        if (gameState.CurrentRun.continuousRun) {
            gameState.CurrentRun.triesLeft++;
        } 
        Save();
    }

    public void Lose() {
		Hero.instance.health = 0;
		Hero.instance.gameObject.SetActive(false);
        ++losses;
		levelIsRunning = false;
		UpdateLevelRun();
        this.TryPlay(loseSound);
        UI.instance.Lose();
        Save();
	}

	public void DecreaseDifficulty() {
		if (GameManager.instance.gameState.CurrentRun.difficulty > 0) {
			GameManager.instance.gameState.CurrentRun.difficulty--;
            restarted = true;
            GameManager.instance.UpdateState();
		}
	}

	public void IncreaseDifficulty() {
		if (GameManager.instance.gameState.CurrentRun.difficulty < GameLevels.instance.difficulties.Count-1) {
			GameManager.instance.gameState.CurrentRun.difficulty++;
            restarted = true;
            GameManager.instance.UpdateState();
		}
	}

	public void PreviousLevel() {
        if (GameManager.instance.gameState.CurrentRun.levelsCompleted <= 0) {
            return;
        }
        restarted = true;
        GameManager.instance.gameState.CurrentRun.levelsCompleted--;
		Debug.LogFormat("Level changed -");
		GameManager.instance.UpdateState();
	}

	public void NextLevel() {
        if (GameManager.instance.gameState.CurrentRun.levelsCompleted > GameLevels.instance.commonLevels.Count - 1) {
            return;
        }
        restarted = true;
		GameManager.instance.gameState.CurrentRun.levelsCompleted++;
		Debug.LogFormat("Level changed +");
		GameManager.instance.UpdateState();
	}

    public bool GameOver() {
        return !LevelIsRunning();
    }

    [ContextMenu("Drop Save")]
    public void DropSaveFile() {
        gameState = new GameState();
		Save();
		UI.instance.CloseAll();
        UpdateState();
    }

    void Awake() {
        ending.Hide();
    }

    public void Load() {
        gameState = FileManager.LoadFromFile<GameState>(GAMESTATE_FILE);
        if (gameState != null) {
			gameState.profiles.ForEach(p => {
				p.completedRuns.ForEach(r => {
					if (r.levelRuns == null) {
						r.levelRuns = new List<LevelRun>();
					}
				});
				p.currentRuns.ForEach(r => {
					if (r.levelRuns == null) {
						r.levelRuns = new List<LevelRun>();
					}
				});
			});
        } else {
            gameState = new GameState();
        }
    }

	public void Continue() {
		if (!GameOver()) {
			return;
		}
		if (Won()) {
			ConfirmWin();
		} else {
			ConfirmLose();
		}
	}

	public void ConfirmWin() {
		UI.instance.CloseAll();
        UpdateState();
    }

    public void ConfirmLose() {
        if (gameState.CurrentRun.continuousRun) {
            if (gameState.CurrentRun.triesLeft > 0) {
                Restart();
            } else {
                FailGame();
            }
        } else {
            Restart();
        }
    }

    public void UpdateState() {
        if (GameManager.instance.gameState.CurrentProfile == null) {
            UI.instance.ChooseProfile();
            return;
        }
        if (gameState.CurrentProfile.name == "") {
            UI.instance.AskName();
            return;
        }
        if (gameState.CurrentProfile.currentRuns.Count == 0) {
            UI.instance.AskDifficulty();
            return;
        }
        ContinueGame(gameState.CurrentProfile.currentRuns.First());
    }

    void ContinueGame(GameRun run) {
        if (run.levelsCompleted == GameLevels.instance.commonLevels.Count) {
            FinishGame(run);
            return;
        }
        RunLevel(GameLevels.instance.commonLevels[run.levelsCompleted]);
    }

    void FinishGame(GameRun run) {
        ending.Show().Then(() => {
            gameState.CurrentProfile.completedRuns.Add(run);
			gameState.CurrentProfile.currentRuns.Remove(run);
			UI.instance.CloseAll();
            UpdateState();
        });
    }

    void FailGame() {
		badEnding.Show().Then(() => {
			gameState.CurrentProfile.completedRuns.Add(gameState.CurrentRun);
			gameState.CurrentProfile.currentRuns.Remove(gameState.CurrentRun);
			UI.instance.CloseAll();
            UpdateState();
        });
    }

    void Start() {
		Load();
		UI.instance.CloseAll();
        UpdateState();
        if (startLevel != null) {
            startLevel.GetComponent<Level>().Run();
        }
    }
    
    public void Clear() {
        FindObjectsOfType<Token>().ForEach(x => {
            x.gameObject.SetActive(false);
            Destroy(x.gameObject);
        });
        if (currentLevel != null) {
            currentLevel.gameObject.SetActive(false);
			Destroy(currentLevel.gameObject);
        }
		if (Board.instance) {
			Board.instance.OnDestroy();
		}
    }

    public void ResetPositions() {
        FindObjectsOfType<Figure>().ForEach(f => f.SetPosition(null));
        FindObjectsOfType<Figure>().ForEach(f => f.Blink());
    }

    public bool restarted = false;
    public void RunLevel(Level level) {
		if (levelIsRunning && currentLevelRun != null) {
			UpdateLevelRun();
		}
		Controls.instance.Reset();
		Intermission.active = false;
        if (gameState.CurrentRun.continuousRun) {
            if (gameState.CurrentRun.triesLeft <= 0) {
                FailGame();
                return;
            }
            gameState.CurrentRun.triesLeft--;
            if (gameState.CurrentRun.levelsCompleted == 0) {
                gameState.CurrentRun.triesLeft = 4;
            }
        }
        lastLevel = level;
        Clear();
		currentLevel = Instantiate(level);
		ExistByCondition.AwakeAll(currentLevel);
		currentLevel.name = level.name;
        currentLevel.gameObject.SetActive(true);
        var commonObjects = Instantiate(GameLevels.instance.commonObjects);
        commonObjects.transform.SetParent(currentLevel.transform);
        commonObjects.SetActive(true);
        Controls.instance.activeUnit = Hero.instance;
        TimeManager.Wait(0).Then(() => {
            UI.instance.UpdateHUD();
        });

        Board.instance.Restore();
		Controls.instance.lockers.Clear();

        FindObjectsOfType<Figure>().ForEach(f => {
			if (f.Position == null || !f.Position.gameObject.activeInHierarchy) {
                f.Blink();
            }
        });

        FindObjectsOfType<OnLevelStart>().ForEach(t => t.Run()); // then run existent onlevelstart triggers

		if (BlackMage.instance.GetComponent<HealthScale>() == null) {
			BlackMage.instance.maxHealth += 5 * (gameState.CurrentRun.difficulty - 4);
			BlackMage.instance.health += 5 * (gameState.CurrentRun.difficulty - 4);
			if (BlackMage.instance.health < 1) {
				BlackMage.instance.health = BlackMage.instance.maxHealth = 1;
			}
		}

		Controls.instance.Ready();

        var intro = currentLevel.GetComponentsInChildren<Intermission>().FirstOrDefault(i => !i.ending);
        if (intro != null)
        {
			if (restarted || gameState.CurrentProfile.skipIntros) {
                intro.Hide();
            } else {
                intro.Show();
            }
        }

		levelIsRunning = true;
		currentLevelRun = new LevelRun();
		gameState.CurrentRun.levelRuns.Add(currentLevelRun);
		currentLevelRun.continuousRun = gameState.CurrentRun.continuousRun;
		currentLevelRun.difficulty = gameState.CurrentRun.difficulty;
		currentLevelRun.levelID = currentLevel.gameObject.name;
		currentLevelRun.panicMode = gameState.CurrentRun.panicMode;
		currentLevelRun.triesLeft = gameState.CurrentRun.triesLeft;
		UpdateLevelRun();
    }

	public void UpdateLevelRun() {
		currentLevelRun.blackMageHealth = BlackMage.instance.health;
		currentLevelRun.heroHealth = Hero.instance.health;
		if (TurnCounter.instance != null) {
			currentLevelRun.steps = TurnCounter.instance.counter.value;
		}
		if (TotalTimeCounter.instance != null) {
			currentLevelRun.seconds = TotalTimeCounter.instance.counter.value;
		}
	}

	public void HeroMoved(Unit hero, Cell from, Cell to, IntVector2 direction) {
        onHeroMove(hero, from, to, direction);
    }

	internal void BeforeHeroMove() {
		Hero.instance.recentDamage = 0;
		BlackMage.instance.recentDamage = 0;
	}

	public void Restart() {
		UI.instance.CloseAll();

		gameState.CurrentRun.levelsCompleted = GameLevels.instance.commonLevels.IndexOf(lastLevel);
        restarted = true;
        RunLevel(lastLevel);
    }

    public void Save() {
        FileManager.SaveToFile(gameState, GAMESTATE_FILE);
    }

    public override void OnDestroy() {
		base.OnDestroy();
		UpdateLevelRun();
        Save();
    }

    public bool LevelIsRunning() {
		return
			Hero.instance != null &&
			Hero.instance.gameObject.activeSelf &&
			BlackMage.instance != null &&
			BlackMage.instance.gameObject.activeSelf;
    }

    public bool Won() {
        return GameOver() && Hero.instance != null;
    }

    public bool Lost() {
        return GameOver() && BlackMage.instance != null;
    }

    void Update() {
        if (LevelIsRunning()) {
            if (BlackMage.instance.Dead) {
				if (!levelIsRunning) {
					Debug.LogError("levelIsRunning == false");
					return;
				}
                Win();
            } else if (Hero.instance.Dead) {
				if (!levelIsRunning) {
					Debug.LogError("levelIsRunning == false");
					return;
				}
                Debug.LogFormat("Destroying hero: {0}", Hero.instance.transform.Path());
                Lose();
            }
        }
    }
}
