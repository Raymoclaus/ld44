using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Player player;
	private Player Player
	{
		get { return player ?? (player = FindObjectOfType<Player>()); }
	}
	
	void Update()
	{
		Vector3 pos = Player?.transform.position ?? Vector3.zero;
		pos.z -= 1f;
		transform.position = pos;
	}
}
