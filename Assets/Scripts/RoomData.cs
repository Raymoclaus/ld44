using UnityEngine;

public class RoomData
{
	public bool leftExit, upExit, rightExit, downExit;
	public IntPair coordinates;
	public RoomType type;

	public RoomData(
		bool leftExit, bool upExit, bool rightExit, bool downExit,
		IntPair coordinates,
		RoomType type)
	{
		this.leftExit = leftExit;
		this.upExit = upExit;
		this.rightExit = rightExit;
		this.downExit = downExit;
		this.coordinates = coordinates;
		this.type = type;
	}
}