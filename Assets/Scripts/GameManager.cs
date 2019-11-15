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

    public void Win() {
		BlackMage.instance.health = 0;
		BlackMage.instance.gameObject.SetActive(false);
        ++wins;
        this.TryPlay(winSound);
        UI.instance.Win();
		Debug.LogFormat("Win");
        gameState.CurrentRun.levelsCompleted++;
		levelIsRunning = false;
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
        this.TryPlay(loseSound);
        UI.instance.Lose();
        Save();
    }

    public bool GameOver() {
        return !LevelIsRunning();
    }

    [ContextMenu("Drop Save")]
    public void DropSaveFile() {
        gameState = new GameState();
        Save();
        UpdateState();
    }

    void Awake() {
        ending.Hide();
    }

    public void Load() {
        gameState = FileManager.LoadFromFile<GameState>(GAMESTATE_FILE);
        if (gameState != null) {

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
            UpdateState();
        });
    }

    void FailGame() {
        badEnding.Show().Then(() => {
            gameState.CurrentProfile.currentRuns.Remove(gameState.CurrentRun);
            UpdateState();
        });
    }

    void Start() {
        Load();
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

    public void RunLevel(Level level, bool restarted = false) {
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
		currentLevel.name = level.name;
        currentLevel.gameObject.SetActive(true);
        var commonObjects = Instantiate(GameLevels.instance.commonObjects);
        commonObjects.transform.SetParent(currentLevel.transform);
        commonObjects.SetActive(true);
        Controls.instance.activeUnit = Hero.instance;
        UI.instance.CloseAll();
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

		ExistByCondition.AwakeAll();
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
    }

	public void HeroMoved(Unit hero, Cell from, Cell to, IntVector2 direction) {
        onHeroMove(hero, from, to, direction);
    }

	internal void BeforeHeroMove() {
		Hero.instance.recentDamage = 0;
		BlackMage.instance.recentDamage = 0;
	}

    public void Restart() {
        RunLevel(lastLevel, restarted: true);
    }

    public void Save() {
        FileManager.SaveToFile(gameState, GAMESTATE_FILE);
    }

    public override void OnDestroy() {
		base.OnDestroy();
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
