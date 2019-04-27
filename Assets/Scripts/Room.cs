using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	public const int WIDTH = 10, HEIGHT = 10, CORRIDOR_WIDTH = 2, CORRIDOR_LENGTH = 2;
	private RoomType type = RoomType.Monsters;
	private List<Tile> tiles = new List<Tile>();
	private Vector2 position;
	[SerializeField] private Tile tilePrefab, wallPrefab;
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
		CreateWalls();
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
					&& (j < CorridorHeightOffset
					|| j >= CorridorHeightOffset + CORRIDOR_WIDTH
					|| !data.leftExit))
				{
					coordinates.x -= 1;
					CreateTile(wallPrefab, coordinates);
				}
				if (i == WIDTH - 1
					&& (j < CorridorHeightOffset
					|| j >= CorridorHeightOffset + CORRIDOR_WIDTH
					|| !data.rightExit))
				{
					coordinates.x += 1;
					CreateTile(wallPrefab, coordinates);
				}
				coordinates.x = i;
				if (j == 0
					&& (i < CorridorWidthOffset
					|| i >= CorridorWidthOffset + CORRIDOR_WIDTH
					|| !data.downExit))
				{
					coordinates.y -= 1;
					CreateTile(wallPrefab, coordinates);
				}
				if (j == HEIGHT - 1
					&& (i < CorridorWidthOffset
					|| i >= CorridorWidthOffset + CORRIDOR_WIDTH
					|| !data.upExit))
				{
					coordinates.y += 1;
					CreateTile(wallPrefab, coordinates);
				}
			}
		}

		CreateCorridor(left: data.leftExit);
		CreateCorridor(up: data.upExit);
		CreateCorridor(right: data.rightExit);
		CreateCorridor(down: data.downExit);
	}

	private void CreateWalls()
	{

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
				CreateTile(tilePrefab, coordinates);

				if ((left || right) && i == 0 && j > 0)
				{
					coordinates.y--;
					CreateTile(wallPrefab, coordinates);
				}
				if ((left || right) && i == CORRIDOR_WIDTH - 1 && j > 0)
				{
					coordinates.y++;
					CreateTile(wallPrefab, coordinates);
				}
				if ((up || down) && i == 0 && j > 0)
				{
					coordinates.x--;
					CreateTile(wallPrefab, coordinates);
				}
				if ((up || down) && i == CORRIDOR_WIDTH - 1 && j > 0)
				{
					coordinates.x++;
					CreateTile(wallPrefab, coordinates);
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
