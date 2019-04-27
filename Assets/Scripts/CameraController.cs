using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Movement player;
	private Movement Player
	{
		get { return player ?? (player = FindObjectOfType<Movement>()); }
	}
	
	void Update()
	{
		Vector3 pos = Player?.transform.position ?? Vector3.zero;
		pos.z -= 1f;
		transform.position = pos;
	}
}
