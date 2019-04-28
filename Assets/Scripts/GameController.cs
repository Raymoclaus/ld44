using UnityEngine;

public class GameController : MonoBehaviour
{
	private static GameController instance;
	private static GameController Instance
	{
		get { return instance ?? (instance = FindObjectOfType<GameController>()); }
	}

	public DungeonGenerator generatorPrefab;
	public Player playerPrefab;

	private DungeonGenerator generator;
	private Player player;
	[SerializeField] private Clock clock;
	[SerializeField] private Timer timerPrefab;
	private Timer timer;

	public int defaultDifficulty = 8;
	public static int difficulty = 8;

	public float defaultStartTimer = 60f;
	public static float startTimer = 60f;

	public float defaultPlayerDamageMultiplier = 1f;
	public static float playerDamageMultiplier = 1f;

	public float defaultPlayerMovementMultiplier = 1f;
	public static float playerMovementMultiplier = 1f;

	private void Awake()
	{
		if (Instance!= this)
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

	private void Start()
	{
		GameOver();
	}

	private void ResetDungeon()
	{
		generator.minRooms = CalculateRooms();
		generator.GenerateDungeon();
		player.transform.position = generator.GetBottom(generator.firstRoom);
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
		difficulty++;
		startTimer += 5;
		Instance.ResetDungeon();
	}

	private void GameOver()
	{
		difficulty = defaultDifficulty;
		startTimer = defaultStartTimer;
		playerDamageMultiplier = defaultPlayerDamageMultiplier;
		playerMovementMultiplier = defaultPlayerMovementMultiplier;
		ResetDungeon();
	}

	public static void TeleportPlayerToLadderRoom()
	{
		Instance.player.transform.position
			= Instance.generator.GetBottom(Instance.generator.endRoom);
	}

	public static float GetDamage()
		=> Instance.player.GetDamage() * playerDamageMultiplier;

	public static Vector2 GetPlayerPosition() => Instance.player.transform.position;
}
