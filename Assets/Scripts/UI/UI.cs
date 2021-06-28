using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;
using RSG;

public class UI : Singletone<UI> {
    public MenuPanel menu;
    public GameObject customLevel;
	public DifficultySelectionPanel difficultySelector;
    public StatisticsPanel statistics;
    public ProfileSelectionPanel profileSelector;
    public Warning warning;
    public Warning soloWarning;
    public GameObject profileName;
    public AudioSource battleMusic;
	public GameObject arrowButtons;

	[Space]
    public GameObject heroHealth;
    public GameObject blackMageHealth;
    public GameObject heartstopperPeriod;
    public GameObject heartstopperPeriodText;
	public GameObject heartstopperDamage;
	public GameObject statuesCounter;

	public GameObject heartHeal;

    public GameObject poisonCounter;
    public GameObject secondPoisonCounter;
	public GameObject poisonDamage;

    public GameObject bombCreationCounter;
	public GameObject bombTimerIcons;
	public GameObject bombOneTimerIcon;
	public GameObject bombRandomTimerIcon;
	public GameObject bombDamage;

	public GameObject fireCreationCounter;
	public GameObject fireDamage;

	public GameObject doorCreationCounter;
	public PeriodicUI doorCreationPeriodic;
	public GameObject doorCreationCounter2;
	public GameObject doorDamage;

    public GameObject monsterCreationCounter;
	public GameObject monsterDamage;
	public GameObject monsterCount;

	public GameObject barrelDamage;

    public GameObject evilEyesCreationCounter;
    public GameObject evilEyesDamage;
    public GameObject statuesCreationCounter;
	public GameObject statuesDamage;
    public GameObject fireExtinguisherCounter;
    public GameObject ankhCounter;
	public GameObject timeCounter;
    public GameObject turnCounter;
	public GameObject totalTimeCounter;
    public GameObject skullSpawnSpeed;
	public GameObject keysUI;
	public List<KeyImage> keyImages;

	[Space]
    public GameObject floatMessage;
    public Text floatMessageText;

    public GameObject loseMessage;
    public GameObject winMessage;

	public GameObject mobileArrows;
	public GameObject mobileContinue;

    public SoundControls volumes;

    public List<Blur> blur;

    void Awake() {
		volumes.Init();
		arrowButtons.SetActive(false);
		#if UNITY_ANDROID
		arrowButtons.SetActive(true);
		#endif
    }

    void Start() {
		keysUI.SetActive(false);
    }

    public void ChooseProfile() {
        CloseAll();
        profileSelector.Show();
    }

    public void AskName() {
        CloseAll();
        profileName.SetActive(true);
    }

    public void AskDifficulty() {
        CloseAll();
        difficultySelector.Show();
    }

    public void Escape() {
		if (GameManager.instance.gameState.CurrentProfile == null) {
			return;
		}
		if (profileName.activeSelf) {
			ProfileNamePanel.instance.Go();
			return;
		}
		if (difficultySelector.gameObject.activeSelf) {
			return;
		}
		if (menu.gameObject.activeSelf) {
            CloseAll();
            return;
        } 
        Menu();
    }

    public void Menu() {
        CloseAll();
        menu.Show();
    }

    public void CloseAll() {
        floatMessage.SetActive(false);
        menu.gameObject.SetActive(false);
        customLevel.SetActive(false);
        difficultySelector.gameObject.SetActive(false);
        profileSelector.gameObject.SetActive(false);

        profileName.SetActive(false);
        warning.Hide();
        soloWarning.Hide();
        volumes.gameObject.SetActive(false);
		statistics.Hide();
    }

    public void Volumes() {
        CloseAll();
        volumes.gameObject.SetActive(true);
    }

    public void Win() {
        blur.ForEach(b => b.enabled = true);
        winMessage.SetActive(true);
		mobileContinue.SetActive(true);
		mobileArrows.SetActive(false);
    }

    public void Lose() {
        blur.ForEach(b => b.enabled = true);
		loseMessage.SetActive(true);
		mobileContinue.SetActive(true);
		mobileArrows.SetActive(false);
    }

    public void UpdateHUD() {
        blur.ForEach(b => b.enabled = false);
        winMessage.SetActive(false);
		loseMessage.SetActive(false);
		mobileContinue.SetActive(false);
		mobileArrows.SetActive(true);
    }

	public bool RandomBombs() {
		var bomb = BombSetter.instance.GetComponent<Spawner>().sample.GetComponent<Bomb>();
		if (bomb && bomb.steps.Count > 1) {
			return true;
		}
		return false;
	}

    public void Update() {
        statuesCounter.SetActive(StatuesCounter.instance);
        statuesCreationCounter.SetActive(StatueSetter.instance);
		statuesDamage.SetActive(StatueSetter.instance);

		skullSpawnSpeed.SetActive(SkullSetter.instance);

        poisonCounter.SetActive(Poison.instance);
		poisonDamage.SetActive(Poison.instance);
        secondPoisonCounter.SetActive(Poison.secondInstance);

		heartstopperPeriod.SetActive(HeartStopperPeriodic.instance || HeartStopper.instance);
		heartHeal.SetActive(Heart.instance);

		barrelDamage.SetActive(BarrelSetter.instance);

        bombCreationCounter.SetActive(BombSetter.instance);
		bombTimerIcons.SetActive(BombSetter.instance);
		bombDamage.SetActive(BombSetter.instance);
		if (BombSetter.instance) {
			bombRandomTimerIcon.SetActive(RandomBombs());
			bombOneTimerIcon.SetActive(!RandomBombs());
		}

		doorCreationCounter.SetActive(DoorSpawner.instance);
		doorCreationCounter2.SetActive(DoorSpawner.instance && DoorSpawner.instance.periodicCounters.Count >= 2);
		doorDamage.SetActive(DoorSpawner.instance);

		fireCreationCounter.SetActive(FireSpawner.instance);
		fireDamage.SetActive(FireSpawner.instance);
        fireExtinguisherCounter.SetActive(FireExtinguisherCounter.instance);

        evilEyesCreationCounter.SetActive(EvilEyesSetter.instance);
		evilEyesDamage.SetActive(EvilEyesSetter.instance);

		monsterCreationCounter.SetActive(MonsterSetter.instance && MonsterSetter.instance.periodicCounter.MaxValue() < 100500);
		monsterCount.SetActive(TokenCounter.cnt[Marks.Monster] > 0);
		monsterDamage.SetActive(MonsterSetter.instance || TokenCounter.cnt[Marks.Monster] > 0);
        timeCounter.SetActive(TimeCounter.instance);
        ankhCounter.SetActive(GameManager.instance.gameState.CurrentRun != null && GameManager.instance.gameState.CurrentRun.continuousRun);

		if (BarrelSetter.instance) {
			var text = barrelDamage.GetComponentInChildren<Text>();
			var damageEffect = Hero.instance.GetComponentInChildren<DamageEffect>();
			text.text = string.Format("<b>{0}</b>", damageEffect.Damage);
		}

		if (SkullSetter.instance) {
			var text = skullSpawnSpeed.GetComponentInChildren<Text>();
			text.text = string.Format("x{0}", SkullSetter.instance.GetComponent<MultipleTimes>().Times);
		}

		keysUI.SetActive(KeyCounter.instance);

		if (BattleMusicChanger.instance.current != null) {
			BattleMusicChanger.instance.current.mute = 
				menu.gameObject.activeSelf || 
				Intermission.active || 
				customLevel.activeSelf || 
				GameManager.instance.GameOver() || 
				GameManager.instance.gameState.CurrentRun == null;
		}

		if (DoorSpawner.instance) {
			doorCreationPeriodic.UpdatePeriodic(DoorSpawner.instance.periodicCounter);
			//if (DoorSpawner.instance.periodicCounter.Multiple()) {
			//	doorCreationCounter.GetComponentInChildren<Text>().text = string.Format(
			//		"<b>{0}</b>",
			//		DoorSpawner.instance.periodicCounter.Value()
			//	);
			//} else {
			//	doorCreationCounter.GetComponentInChildren<Text>().text =
			//		DoorSpawner.instance.periodicCounter.Format();
			//}
			//if (DoorSpawner.instance.periodicCounters.Count >= 2) {
			//	doorCreationCounter2.GetComponentInChildren<Text>().text =
			//		DoorSpawner.instance.periodicCounters[1].Format();
			//}
			var text = doorDamage.GetComponentInChildren<Text>();
			text.text = string.Format("<b>{0}</b>", DoorSpawner.instance.GetComponent<Spawner>().sample.GetComponentInChildren<DamageEffect>().Damage);
		}
		if (FireSpawner.instance) {
			fireCreationCounter.GetComponentInChildren<Text>().text = string.Format(
				"<b>{0}/{1}</b>",
				FireSpawner.instance.periodicCounter.Value(),
				FireSpawner.instance.periodicCounter.MaxValue()
			);
			var text = fireDamage.GetComponentInChildren<Text>();
			text.text = string.Format("<b>{0}</b>", FireSpawner.instance.GetComponent<Spawner>().sample.GetComponentInChildren<DamageEffect>().Damage);
		}
		if (MonsterSetter.instance && MonsterSetter.instance.periodicCounter.MaxValue() < 100500) {
			monsterCreationCounter.GetComponentInChildren<Text>().text = string.Format(
				"<b>{0}/{1}</b>",
				MonsterSetter.instance.periodicCounter.Value(),
				MonsterSetter.instance.periodicCounter.MaxValue()
			);
		}
		if (monsterDamage.activeSelf) {
			var text = monsterDamage.GetComponentInChildren<Text>();
			var monster =
			   	MonsterSetter.instance ?
			   	MonsterSetter.instance.GetComponent<Spawner>().sample :
				TokenCounter.list[Marks.Monster][0];
			text.text = string.Format("<b>{0}</b>", monster.GetComponent<DamageEffect>().Damage);
		}
		if (monsterCount.activeSelf) {
			monsterCount.GetComponentInChildren<Text>().text = 
				string.Format("<b>{0}</b>", TokenCounter.cnt[Marks.Monster]);
		}
        if (BombSetter.instance) {
			var times = BombSetter.instance.GetComponent<MultipleTimes>().Times;
            bombCreationCounter.GetComponentInChildren<Text>().text = string.Format(
				"<b>{0}/{1}{2}</b>", 
				BombSetter.instance.GetComponent<PeriodicCounter>().Value(), 
				BombSetter.instance.GetComponent<PeriodicCounter>().MaxValue(),
				times != 1 ? " x{0}".i(times) : ""
			);
			var text = bombDamage.GetComponentInChildren<Text>();
			var damage = 0;
			var damageHero = Hero.instance.GetComponentInChildren<DamageUnit>();
			if (damageHero != null) {
				damage = Mathf.Max(damage, damageHero.Damage);
			}
			var bomb = BombSetter.instance.GetComponent<Spawner>().sample.GetComponent<Bomb>();
			if (bomb != null) {
				damage = Mathf.Max(damage, bomb.Damage);
			}
			var explosionDamage = Hero.instance.GetComponentInChildren<DamageEffect>();
			if (explosionDamage != null) {
				damage = Mathf.Max(damage, explosionDamage.Damage);
			}
			text.text = string.Format("<b>{0}</b>", damage);
        }       
        if (Poison.instance) {
            poisonCounter.GetComponentInChildren<Text>().text = string.Format("<b>{0}/{1}</b>", Poison.instance.Timeout-Poison.instance.spent, Poison.instance.Timeout);
			var text = poisonDamage.GetComponentInChildren<Text>();
			text.text = string.Format("<b>{0}</b>", Poison.instance.Damage);
        }     
        if (Poison.secondInstance) {
            secondPoisonCounter.GetComponentInChildren<Text>().text = string.Format("<b>{0}/{1}</b>", Poison.secondInstance.Timeout-Poison.secondInstance.spent, Poison.secondInstance.Timeout);
        }
		if (HeartStopperPeriodic.instance) {
			var text = heartstopperPeriodText.GetComponentInChildren<Text>();
			var counter = HeartStopperPeriodic.instance.periodicCounter;
			if (counter != null) {
				text.text = string.Format("<b>{0}/{1}</b>", counter.Value(), counter.MaxValue());
			} else {
				text.text = "";
			}
			var damageText = heartstopperDamage.GetComponentInChildren<Text>();
			damageText.text = HeartStopperPeriodic.instance.GetComponent<DamageEffect>()?.damage.ToString();
		}
		if (HeartStopper.instance) {
			var text = heartstopperPeriodText.GetComponentInChildren<Text>();
			text.text = "";
			var damageText = heartstopperDamage.GetComponentInChildren<Text>();
			damageText.text = HeartStopper.instance.damage.ToString();
		}
		if (Heart.instance) {
			heartHeal.GetComponentInChildren<Text>().text = Heart.instance.Heal.ToString();
		}

        if (StatueSetter.instance) {
            var text = statuesCreationCounter.GetComponentInChildren<Text>();
            var counter = StatueSetter.instance.GetComponent<PeriodicCounter>();
            text.text = string.Format("<b>{0}/{1}</b>", counter.Value(), counter.MaxValue());
			var damageText = statuesDamage.GetComponentInChildren<Text>();
			damageText.text = string.Format("<b>{0}</b>", StatueSetter.instance.GetComponent<DamageEffect>().Damage);
        }
		if (EvilEyesSetter.instance) {
			var text = evilEyesCreationCounter.GetComponentInChildren<Text>();
			var counter = EvilEyesSetter.instance.periodicCounter;
			text.text = string.Format("<b>{0}/{1}</b>", counter.Value(), counter.MaxValue());
			var damageText = evilEyesDamage.GetComponentInChildren<Text>();
			damageText.text = string.Format("<b>{0}</b>", EvilEyesSetter.instance.GetComponent<Spawner>().sample.GetComponent<EvilEye>().Damage);
		}
        if (StatuesCounter.instance) {
            statuesCounter.GetComponentInChildren<Text>().text = string.Format("<b>{0}/{1}</b>", StatuesCounter.instance.counter.value, StatuesCounter.instance.max);
        }      
        if (FireExtinguisherCounter.instance) {
            fireExtinguisherCounter.GetComponentInChildren<Text>().text = string.Format("<b>{0}/{1}</b>", FireExtinguisherCounter.instance.counter.value, FireExtinguisherCounter.instance.counter.MaxValue);
        }      
        if (GameManager.instance.gameState.CurrentRun != null && GameManager.instance.gameState.CurrentRun.continuousRun) {
            ankhCounter.GetComponentInChildren<Text>().text = string.Format("<b>{0}</b>", GameManager.instance.gameState.CurrentRun.triesLeft);
        }     
        if (TimeCounter.instance) {
            timeCounter.GetComponentInChildren<Text>().text = string.Format("<b>{0}/{1}</b>", TimeCounter.instance.counter.MaxValue-TimeCounter.instance.counter.value, TimeCounter.instance.counter.MaxValue);
        }
		if (TurnCounter.instance) {
			turnCounter.GetComponentInChildren<Text>().text = "<b>{0}</b>".i(TurnCounter.instance.counter.value);
		}
		if (TotalTimeCounter.instance) {
			totalTimeCounter.GetComponentInChildren<Text>().text = "<b>{0}</b>".i(TotalTimeCounter.instance.counter.value);
		}
    }

    public IPromise Confirm(string text) {
        return warning.Show(text);
    }

    public IPromise SoloConfirm(string text) {
        return soloWarning.Show(text);
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
