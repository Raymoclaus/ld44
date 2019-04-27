using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public DungeonGenerator generatorPrefab;
	public Movement playerPrefab;

	private DungeonGenerator generator;
	private Movement player;

	private void Awake()
	{
		generator = Instantiate(generatorPrefab);
		generator.GenerateDungeon();
		player = Instantiate(playerPrefab);
		player.transform.position = generator.GetCenter(generator.firstRoom);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			generator.GenerateDungeon();
		}
	}
}
