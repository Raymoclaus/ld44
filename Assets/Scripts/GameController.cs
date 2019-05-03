using UnityEngine;
using System;
using System.Collections;

public class GameController : MonoBehaviour
{
	private static GameController instance;
	private static GameController Instance
	{
		get { return instance ?? (instance = FindObjectOfType<GameController>()); }
	}

	[SerializeField] private CanvasGroup cGroup;

	public DungeonGenerator generatorPrefab;
	public Player playerPrefab;

	private DungeonGenerator generator;
	private Player player;
	[SerializeField] private Clock clock;
	[SerializeField] private Timer timerPrefab;
	private Timer timer;

	[SerializeField] private CanvasGroup escToPause, pauseCanvas;
	[SerializeField] private SceneLoader sceneLoader;
	[SerializeField] private CanvasGroup clockCanvasGroup;

	public int defaultDifficulty = 8;
	public static int difficulty = 8;

	public float defaultStartTimer = 60f;
	public static float startTimer = 60f;

	public float defaultPlayerDamageMultiplier = 1f;
	public static float playerDamageMultiplier = 1f;

	public float defaultPlayerMovementMultiplier = 1f;
	public static float playerMovementMultiplier = 1f;

	public static bool IsPaused { get; set; }
	public static bool IsTransitioning { get; set; }

	private void Awake()
	{
		if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		generator = Instantiate(generatorPrefab);
		player = Instantiate(playerPrefab);
		timer = Instantiate(timerPrefab);
		difficulty = defaultDifficulty;
		startTimer = defaultStartTimer;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void Start()
	{
		ResetDefaults();
		ResetDungeon();
		cGroup.alpha = 1f;
		StartCoroutine(Transition(false, null, () => StartTimer()));
		pauseCanvas.alpha = 0f;
		escToPause.alpha = 1f;
		Time.timeScale = 0f;
		clockCanvasGroup.alpha = 0f;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !IsTransitioning && !ShopUI.IsActive)
		{
			IsPaused = !IsPaused;
			Time.timeScale = IsPaused ? 0f : 1f;
			escToPause.alpha = IsPaused ? 0f : 1f;
			pauseCanvas.alpha = IsPaused ? 1f : 0f;
		}

		if (Input.GetKeyDown(KeyCode.Q) && IsPaused)
		{
			sceneLoader.LoadScene("MainMenuScene");
		}
	}

	private void SaveHighScore()
	{
		int highscore = PlayerPrefs.GetInt("Highscore", 0);
		if (difficulty - 7 > highscore)
		{
			PlayerPrefs.SetInt("Highscore", difficulty - 7);
		}
	}

	private void ResetDungeon()
	{
		generator.minRooms = CalculateRooms();
		generator.GenerateDungeon();
		player.transform.position = generator.GetBottom(generator.firstRoom);
	}

	private void StartTimer()
	{
		timer.StartTimer(startTimer, startTimer, (float d) =>
		{
			clock.SetDelta(d);
			clock.UpdateTextCounter(GetTime(), startTimer);
		}, () => GameOver());
	}

	public static void AddTime(float time)
	{
		Instance.timer.AddTime(time);
	}

	public static void MultiplyTime(float multiply)
	{
		Instance.timer.MultiplyTime(multiply);
	}

	public static float GetTime() => Instance.timer.GetTime();

	private int CalculateRooms()
	{
		return difficulty * difficulty / 16 + 1;
	}

	public static void EndReached()
	{
		Instance.Save();
		Time.timeScale = 0f;
		Instance.StartCoroutine(Instance.Transition(true, () =>
		{
			difficulty++;
			startTimer += 5;
			Instance.ResetDungeon();
		}, () => Instance.StartTimer()));
	}

	private void Save()
	{
		PlayerPrefs.SetInt("Saved", 1);
		PlayerPrefs.SetInt("Difficulty", difficulty);
		PlayerPrefs.SetFloat("MaxTime", startTimer);
		PlayerPrefs.SetFloat("Damage", playerDamageMultiplier);
		PlayerPrefs.SetFloat("Speed", playerMovementMultiplier);
		Instance.SaveHighScore();
	}

	private void GameOver()
	{
		PlayerPrefs.SetInt("Saved", 0);
		Time.timeScale = 0f;
		StartCoroutine(Transition(true, () =>
		{
			sceneLoader.LoadScene("MainMenuScene");
		}, null));
	}

	private void ResetDefaults()
	{
		if (PlayerPrefs.GetInt("Saved", 0) == 1)
		{
			difficulty = PlayerPrefs.GetInt("Difficulty", defaultDifficulty);
			startTimer = PlayerPrefs.GetFloat("MaxTime", defaultStartTimer);
			playerDamageMultiplier
				= PlayerPrefs.GetFloat("Damage", defaultPlayerDamageMultiplier);
			playerMovementMultiplier
				= PlayerPrefs.GetFloat("Speed", defaultPlayerMovementMultiplier);
		}
		else
		{
			difficulty = defaultDifficulty;
			startTimer = defaultStartTimer;
			playerDamageMultiplier = defaultPlayerDamageMultiplier;
			playerMovementMultiplier = defaultPlayerMovementMultiplier;
		}
	}

	public static void TeleportPlayerToLadderRoom()
	{
		Instance.player.transform.position
			= Instance.generator.GetBottom(Instance.generator.endRoom);
	}

	public static float GetDamage()
		=> Instance.player.GetDamage() * playerDamageMultiplier;

	public static Vector2 GetPlayerPosition() => Instance.player.transform.position;

	public static void DealDamageToPlayer(float damage, Vector2 enemyPos)
	{
		Vector2 dir = enemyPos - (Vector2)Instance.player.transform.position;
		Instance.player.DealDamage(damage, dir.normalized);
	}

	private IEnumerator Transition(bool fadeOut, Action action, Action action2)
	{
		IsTransitioning = true;
		float timer = 0f;
		float time = 2f;
		while (timer < time)
		{
			timer += Time.unscaledDeltaTime;
			float delta = timer / time;
			cGroup.alpha = fadeOut ? delta : 1f - delta;
			yield return null;
		}

		if (fadeOut)
		{
			action?.Invoke();
			StartCoroutine(Transition(false, null, action2));
			clockCanvasGroup.alpha = 0f;
		}
		else
		{
			action2?.Invoke();
			Time.timeScale = 1f;
			IsTransitioning = false;
			clockCanvasGroup.alpha = 1f;
		}
	}

	public static void ActivateInteractIcon(bool activate)
	{
		Instance.player.ActivateInteractIcon(activate);
	}

	public static void ActivateEscToPauseCanvas(bool activate)
	{
		Instance.escToPause.alpha = activate ? 1f : 0f;
	}
}
