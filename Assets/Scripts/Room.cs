using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	public const int WIDTH = 11, HEIGHT = 11, CORRIDOR_WIDTH = 3, CORRIDOR_LENGTH = 3;
	private RoomType type = RoomType.Empty;
	private Vector2 position;
	[SerializeField] private Tile tilePrefab;
	[SerializeField] private Tile
		ulInnerWallPrefab, ulOuterWallPrefab, uWallPrefab, urInnerWallPrefab, urOuterWallPrefab,
		lWallPrefab, rWallPrefab,
		dlInnerWallPrefab, dlOuterWallPrefab, dWallPrefab, drInnerWallPrefab, drOuterWallPrefab;
	[SerializeField] private Ladder ladderPrefab;
	[SerializeField] private Shop shopPrefab;
	[SerializeField] private List<Enemy> enemies;
	[SerializeField] private IntPair enemySpawnAmount = new IntPair(1, 6);
	private RoomData data;

	private int CorridorWidthOffset { get { return WIDTH / 2 - CORRIDOR_WIDTH / 2; } }
	private int CorridorHeightOffset { get { return WIDTH / 2 - CORRIDOR_WIDTH / 2; } }

	private void SetType(RoomType type) => this.type = type;

	public void SetData(RoomData data)
	{
		this.data = data;
		SetType(data.type);
		SetPosition(DungeonGenerator.GetPosition(data.coordinates));
	}

	private void SetPosition(Vector2 position) => transform.localPosition = position;

	private void Start()
	{
		CreateTiles();
		switch (type)
		{
			case RoomType.Monsters:
				int randomCount = Random.Range(enemySpawnAmount.x,
					Mathf.Min(GameController.difficulty - 5, enemySpawnAmount.y + 1));
				for (int i = 0; i < randomCount; i++)
				{
					int randomEnemy = Random.Range(0, enemies.Count);
					Enemy enemy = Instantiate(enemies[randomEnemy]);
					enemy.transform.parent = transform;
					enemy.transform.localPosition = GetPosition(
						new IntPair(Random.Range(1, WIDTH), Random.Range(1, HEIGHT)));
				}
				break;
			case RoomType.Shop:
				Shop shop = Instantiate(shopPrefab);
				shop.transform.parent = transform;
				Vector2 shopPos = GetPosition(
					new IntPair(WIDTH - 3, HEIGHT));
				shopPos.x += Tile.WIDTH / 2f;
				shop.transform.localPosition = shopPos;
				break;
			case RoomType.Treasure:
				break;
			case RoomType.Puzzle:
				break;
			case RoomType.End:
				Ladder ladder = Instantiate(ladderPrefab);
				ladder.transform.parent = transform;
				ladder.transform.localPosition = GetPosition(
					new IntPair((int)(WIDTH / 2f), (int)(HEIGHT / 2f)));
				break;
			case RoomType.Empty:
				break;
		}
	}

	private void CreateTiles()
	{
		for (int i = 0; i < WIDTH; i++)
		{
			for (int j = 0; j < HEIGHT; j++)
			{
				IntPair coordinates = new IntPair(i, j);
				CreateTile(tilePrefab, coordinates);

				if (i == 0
					&& (j < CorridorHeightOffset - 1
					|| j >= CorridorHeightOffset + CORRIDOR_WIDTH + 1
					|| !data.leftExit))
				{
					coordinates.x -= 1;
					CreateTile(lWallPrefab, coordinates);
					if (j == 0)
					{
						coordinates.y -= 1;
						CreateTile(dlInnerWallPrefab, coordinates);
					}
					else if (j == HEIGHT - 1)
					{
						coordinates.y += 1;
						CreateTile(ulInnerWallPrefab, coordinates);
					}
				}
				if (i == WIDTH - 1
					&& (j < CorridorHeightOffset - 1
					|| j >= CorridorHeightOffset + CORRIDOR_WIDTH + 1
					|| !data.rightExit))
				{
					coordinates.x += 1;
					CreateTile(rWallPrefab, coordinates);
					if (j == 0)
					{
						coordinates.y -= 1;
						CreateTile(drInnerWallPrefab, coordinates);
					}
					else if (j == HEIGHT - 1)
					{
						coordinates.y += 1;
						CreateTile(urInnerWallPrefab, coordinates);
					}
				}
				coordinates.x = i;
				coordinates.y = j;
				if (j == 0
					&& (i < CorridorWidthOffset - 1
					|| i >= CorridorWidthOffset + CORRIDOR_WIDTH + 1
					|| !data.downExit))
				{
					coordinates.y -= 1;
					CreateTile(dWallPrefab, coordinates);
				}
				if (j == HEIGHT - 1
					&& (i < CorridorWidthOffset - 1
					|| i >= CorridorWidthOffset + CORRIDOR_WIDTH + 1
					|| !data.upExit))
				{
					coordinates.y += 1;
					CreateTile(uWallPrefab, coordinates);
				}
			}
		}
		
		CreateCorridor(left: data.leftExit);
		CreateCorridor(up: data.upExit);
		CreateCorridor(right: data.rightExit);
		CreateCorridor(down: data.downExit);
	}

	private void CreateCorridor(
		bool left = false, bool up = false, bool right = false, bool down = false)
	{
		for (int i = 0; i < CORRIDOR_WIDTH; i++)
		{
			for (int j = 0; j < CORRIDOR_LENGTH; j++)
			{
				IntPair coordinates = new IntPair();
				if (left)
				{
					coordinates.x = -1 - j;
					coordinates.y = CorridorHeightOffset + i;
				}
				else if (up)
				{
					coordinates.x = CorridorHeightOffset + i;
					coordinates.y = HEIGHT + j;
				}
				else if (right)
				{
					coordinates.x = WIDTH + j;
					coordinates.y = CorridorHeightOffset + i;
				}
				else if (down)
				{
					coordinates.x = CorridorWidthOffset + i;
					coordinates.y = -1 - j;
				}
				if (left || up || right || down)
				{
					CreateTile(tilePrefab, coordinates);
				}

				if ((left || right) && i == 0)
				{
					coordinates.y--;
					CreateTile(
						left ? (j == 0 ? urOuterWallPrefab : dWallPrefab)
						: (j == 0 ? ulOuterWallPrefab : dWallPrefab), coordinates);
				}
				if ((left || right) && i == CORRIDOR_WIDTH - 1)
				{
					coordinates.y++;
					CreateTile(left ? (j == 0 ? drOuterWallPrefab : uWallPrefab)
						: (j == 0 ? dlOuterWallPrefab : uWallPrefab), coordinates);
				}
				if ((up || down) && i == 0)
				{
					coordinates.x--;
					CreateTile(up ? (j == 0 ? drOuterWallPrefab : lWallPrefab)
						: (j == 0 ? urOuterWallPrefab : lWallPrefab), coordinates);
				}
				if ((up || down) && i == CORRIDOR_WIDTH - 1)
				{
					coordinates.x++;
					CreateTile(up ? (j == 0 ? dlOuterWallPrefab : rWallPrefab)
						: (j == 0 ? ulOuterWallPrefab : rWallPrefab), coordinates);
				}
			}
		}
	}

	private void CreateTile(Tile prefab, IntPair coordinates)
	{
		Tile newTile = Instantiate(prefab);
		newTile.transform.parent = transform;
		newTile.SetPosition(GetPosition(coordinates));
	}

	private Vector2 GetPosition(IntPair coordinates)
		=> new Vector2(coordinates.x * Tile.WIDTH, coordinates.y * Tile.HEIGHT);
}
