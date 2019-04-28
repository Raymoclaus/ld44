using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
	private List<List<RoomData>> dataList = new List<List<RoomData>>();
	public RoomData firstRoom, endRoom;
	private List<Room> rooms = new List<Room>();
	[SerializeField] private Room roomPrefab;
	private Transform roomHolder;

	public int minRooms = 10;
	private int maxRooms { get { return (int)((float)minRooms * 1.5f); } }
	private int roomCount;
	[SerializeField] private int numEndRooms = 1, numShopRooms = 1;
	[SerializeField]
	private float monsterRoomChance = 0.5f, treasureRoomChance = 0.1f,
		puzzleRoomChance = 0.1f;

	private bool RandomBool { get { return Random.value > 0.5f; } }

	public static float CELL_WIDTH
	{
		get { return (Room.WIDTH + Room.CORRIDOR_LENGTH * 2) * Tile.WIDTH; }
	}
	public static float CELL_HEIGHT
	{
		get { return (Room.HEIGHT + Room.CORRIDOR_LENGTH * 2) * Tile.HEIGHT; }
	}

	public void GenerateDungeon()
	{
		do
		{
			roomCount = 0;
			dataList.Clear();
			CreateDungeon();
			List<RoomData> mainRooms = GetMainRooms();
			DeleteRoomsExcluding(mainRooms);
		} while (CountDataList() < minRooms || CountDataList() > maxRooms);

		CleanupCorridors();

		SetRoomTypes();

		//for (int i = 0; i < dataList.Count; i++)
		//{
		//	for (int j = 0; j < dataList[i].Count; j++)
		//	{
		//		if (dataList[i][j] == null) continue;
		//		Debug.Log(dataList[i][j].type);
		//	}
		//}

		if (roomHolder != null)
		{
			Destroy(roomHolder.gameObject);
		}
		roomHolder = new GameObject("RoomHolder").transform;
		CreateRooms();
	}

	private void CreateDungeon()
	{
		IntPair coordinates = new IntPair(0, 0);
		firstRoom = new RoomData(false, true, false, false,
			coordinates, RoomType.Empty);
		CreateData(firstRoom, coordinates);

		int gridSize = GetAppropriateSize();
		for (int i = 0; i < gridSize; i++)
		{
			for (int j = 0; j < gridSize; j++)
			{
				coordinates = new IntPair(i, j);
				if (RoomExists(coordinates)) continue;

				RoomData leftRoom = GetLeftOfRoom(coordinates);
				RoomData upRoom = GetUpOfRoom(coordinates);
				RoomData rightRoom = GetRightOfRoom(coordinates);
				RoomData downRoom = GetDownOfRoom(coordinates);
				RoomData newData = new RoomData(
					i != 0 && (leftRoom != null ? leftRoom.rightExit : RandomBool),
					j < gridSize - 1 && (upRoom != null ? upRoom.downExit : RandomBool),
					i < gridSize - 1 && (rightRoom != null ? rightRoom.leftExit : RandomBool),
					j != 0 && (downRoom != null ? downRoom.upExit : RandomBool),
					coordinates,
					RoomType.Empty);

				CreateData(newData, coordinates);
				if (roomCount >= maxRooms) return;
			}
		}
	}

	private List<RoomData> GetMainRooms()
	{
		IntPair coordinates = firstRoom.coordinates;
		List<RoomData> mainRooms = new List<RoomData> { firstRoom };
		GetNeighbours(firstRoom, mainRooms);
		return mainRooms;
	}

	private void DeleteRoomsExcluding(List<RoomData> exclusions)
	{
		for (int i = 0; i < dataList.Count; i++)
		{
			for (int j = 0; j < dataList[i].Count; j++)
			{
				IntPair coordinates = new IntPair(i, j);
				RoomData currentRoom = GetRoomData(coordinates);

				if (Contains(exclusions, currentRoom)) continue;

				dataList[coordinates.x][coordinates.y] = null;
			}
		}
	}

	private int CountDataList()
	{
		int count = 0;
		for (int i = 0; i < dataList.Count; i++)
		{
			for (int j = 0; j < dataList[i].Count; j++)
			{
				if (dataList[i][j] == null) continue;
				count++;
			}
		}
		return count;
	}

	private void CleanupCorridors()
	{
		for (int i = 0; i < dataList.Count; i++)
		{
			for (int j = 0; j < dataList[i].Count; j++)
			{
				IntPair coordinates = new IntPair(i, j);
				RoomData currentRoom = GetRoomData(coordinates);
				if (currentRoom == null) continue;

				RoomData leftRoom = GetLeftOfRoom(currentRoom);
				RoomData upRoom = GetUpOfRoom(currentRoom);
				RoomData rightRoom = GetRightOfRoom(currentRoom);
				RoomData downRoom = GetDownOfRoom(currentRoom);

				if (currentRoom.leftExit && (leftRoom == null || !leftRoom.rightExit))
				{
					currentRoom.leftExit = false;
				}
				if (currentRoom.upExit && (upRoom == null || !upRoom.downExit))
				{
					currentRoom.upExit = false;
				}
				if (currentRoom.rightExit && (rightRoom == null || !rightRoom.leftExit))
				{
					currentRoom.rightExit = false;
				}
				if (currentRoom.downExit && (downRoom == null || !downRoom.upExit))
				{
					currentRoom.downExit = false;
				}
			}
		}
	}

	private void CreateRooms()
	{
		for (int i = 0; i < dataList.Count; i++)
		{
			for (int j = 0; j < dataList[i].Count; j++)
			{
				RoomData currentRoom = dataList[i][j];
				if (currentRoom == null) continue;

				Room newRoom = Instantiate(roomPrefab);
				newRoom.transform.parent = roomHolder;
				newRoom.SetData(currentRoom);
				rooms.Add(newRoom);
			}
		}
	}

	private void SetRoomTypes()
	{
		List<RoomData> rooms = new List<RoomData>();
		for (int i = 0; i < dataList.Count; i++)
		{
			for (int j = 0; j < dataList[i].Count; j++)
			{
				if (dataList[i][j] == firstRoom || dataList[i][j] == null) continue;
				rooms.Add(dataList[i][j]);
			}
		}

		int random = 0;
		for (int i = 0; i < numEndRooms; i++)
		{
			random = Random.Range(0, rooms.Count);
			rooms[random].type = RoomType.End;
			endRoom = rooms[random];
			rooms.RemoveAt(random);
		}
		for (int i = 0; i < numShopRooms; i++)
		{
			random = Random.Range(0, rooms.Count);
			rooms[random].type = RoomType.Shop;
			rooms.RemoveAt(random);
		}

		for (int i = 0; i < rooms.Count; i++)
		{
			float randomVal = Random.value;
			if (randomVal < monsterRoomChance)
			{
				rooms[i].type = RoomType.Monsters;
				continue;
			}
			randomVal -= monsterRoomChance;
			if (randomVal < treasureRoomChance)
			{
				rooms[i].type = RoomType.Treasure;
				continue;
			}
			randomVal -= treasureRoomChance;
			if (randomVal < puzzleRoomChance)
			{
				rooms[i].type = RoomType.Puzzle;
				continue;
			}
		}
	}

	/// <summary>
	/// returns a integer where the square of that integer is greater than minRooms
	/// </summary>
	private int GetAppropriateSize()
	{
		int value = 1;
		while (value * value < minRooms)
		{
			value++;
		}
		return value;
	}

	private void GetNeighbours(RoomData data, List<RoomData> neighbours)
	{
		RoomData leftRoom = GetLeftOfRoom(data);
		RoomData upRoom = GetUpOfRoom(data);
		RoomData rightRoom = GetRightOfRoom(data);
		RoomData downRoom = GetDownOfRoom(data);

		if (data.leftExit && leftRoom != null && leftRoom.rightExit)
		{
			if (!Contains(neighbours, leftRoom))
			{
				neighbours.Add(leftRoom);
				GetNeighbours(leftRoom, neighbours);
			}
		}
		if (data.upExit && upRoom != null && upRoom.downExit)
		{
			if (!Contains(neighbours, upRoom))
			{
				neighbours.Add(upRoom);
				GetNeighbours(upRoom, neighbours);
			}
		}
		if (data.rightExit && rightRoom != null && rightRoom.leftExit)
		{
			if (!Contains(neighbours, rightRoom))
			{
				neighbours.Add(rightRoom);
				GetNeighbours(rightRoom, neighbours);
			}
		}
		if (data.downExit && downRoom != null && downRoom.upExit)
		{
			if (!Contains(neighbours, downRoom))
			{
				neighbours.Add(downRoom);
				GetNeighbours(downRoom, neighbours);
			}
		}
	}

	private bool Contains(List<RoomData> list, RoomData room)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == room) return true;
		}
		return false;
	}

	public static Vector2 GetPosition(IntPair coordinates)
		=> new Vector2(coordinates.x * CELL_WIDTH, coordinates.y * CELL_HEIGHT);

	private void CreateData(RoomData data, IntPair coordinates)
	{
		if (RoomExists(coordinates)) return;

		while (dataList.Count <= coordinates.x)
		{
			dataList.Add(new List<RoomData>());
		}
		while (dataList[coordinates.x].Count <= coordinates.y)
		{
			dataList[coordinates.x].Add(null);
		}
		dataList[coordinates.x][coordinates.y] = data;
		roomCount++;
	}

	private bool RoomExists(IntPair coordinates)
	{
		if (coordinates.x < 0 || dataList.Count <= coordinates.x) return false;
		if (coordinates.y < 0 || dataList[coordinates.x].Count <= coordinates.y) return false;
		return dataList[coordinates.x][coordinates.y] != null;
	}

	private RoomData GetRoomData(IntPair coordinates)
	{
		if (!RoomExists(coordinates)) return null;
		return dataList[coordinates.x][coordinates.y];
	}

	private RoomData GetLeftOfRoom(IntPair coordinates)
	{
		coordinates.x--;
		return GetRoomData(coordinates);
	}

	private RoomData GetUpOfRoom(IntPair coordinates)
	{
		coordinates.y++;
		return GetRoomData(coordinates);
	}

	private RoomData GetRightOfRoom(IntPair coordinates)
	{
		coordinates.x++;
		return GetRoomData(coordinates);
	}

	private RoomData GetDownOfRoom(IntPair coordinates)
	{
		coordinates.y--;
		return GetRoomData(coordinates);
	}

	private RoomData GetLeftOfRoom(RoomData data) => GetLeftOfRoom(data.coordinates);

	private RoomData GetUpOfRoom(RoomData data) => GetUpOfRoom(data.coordinates);

	private RoomData GetRightOfRoom(RoomData data) => GetRightOfRoom(data.coordinates);

	private RoomData GetDownOfRoom(RoomData data) => GetDownOfRoom(data.coordinates);

	public Vector2 GetCenter(RoomData roomData)
	{
		int x = roomData.coordinates.x;
		int y = roomData.coordinates.y;
		return new Vector2(x * CELL_WIDTH + CELL_WIDTH / 2f - Room.CORRIDOR_LENGTH * Tile.WIDTH - Tile.WIDTH / 2f,
			y * CELL_HEIGHT + CELL_HEIGHT / 2f - Room.CORRIDOR_LENGTH * Tile.HEIGHT - Tile.HEIGHT);
	}

	public Vector2 GetBottom(RoomData roomData)
	{
		int x = roomData.coordinates.x;
		int y = roomData.coordinates.y;
		return new Vector2(x * CELL_WIDTH + CELL_WIDTH / 2f - Room.CORRIDOR_LENGTH * Tile.WIDTH - Tile.WIDTH / 2f,
			y * CELL_HEIGHT + 2 - Tile.HEIGHT);
	}
}
