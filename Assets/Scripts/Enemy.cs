using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField] protected Rigidbody2D rb;
	[SerializeField] protected SpriteRenderer sprRend;
	protected float baseHp = 10;
	protected float hp;
	[SerializeField] protected float playerDetectionRadius = 5f;
	[SerializeField] protected float speedMultiplier = 1f;

	private void Update()
	{
		float distToPlayer = DistanceToPlayer();
		if (distToPlayer <= playerDetectionRadius)
		{
			ReactToPlayerPresence(distToPlayer);
		}
	}

	protected virtual void ReactToPlayerPresence(float dist)
	{

	}

	protected float DistanceToPlayer()
		=> Vector2.Distance(transform.position, GameController.GetPlayerPosition());

	protected Vector2 GetPosition() => transform.position;
}
