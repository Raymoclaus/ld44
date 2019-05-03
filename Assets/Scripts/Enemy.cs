using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
	[SerializeField] protected Rigidbody2D rb;
	[SerializeField] protected SpriteRenderer sprRend;
	protected float baseHp = 10;
	protected float hp;
	[SerializeField] protected float playerDetectionRadius = 5f;
	[SerializeField] protected float speedMultiplier = 1f;
	[SerializeField] private int timerRewardOnHit = 1;
	[SerializeField] private float attackDamage = 2f;
	[SerializeField] protected Animator anim;
	[SerializeField] protected Collider2D col;

	protected bool canMove = true;

	private void Update()
	{
		float distToPlayer = DistanceToPlayer();
		if (distToPlayer <= playerDetectionRadius)
		{
			ReactToPlayerPresence(distToPlayer);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Attack"))
		{
			Vector2 dir = GameController.GetPlayerPosition() - GetPosition();
			TakeDamage(GameController.GetDamage(), dir.normalized);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			GameController.DealDamageToPlayer(attackDamage * GameController.difficulty - 7, GetPosition());
			StartCoroutine(Stop());
		}
	}

	public virtual void TakeDamage(float damage, Vector2 dir)
	{
		hp -= damage;
		GameController.AddTime(timerRewardOnHit);
		canMove = false;
		StartCoroutine(Damaged(-dir * 100f));
		if (hp <= 0f)
		{
			Die();
		}
	}

	public virtual void Die()
	{
		col.enabled = false;
	}

	public void Dead()
	{
		Destroy(gameObject);
	}

	protected virtual void ReactToPlayerPresence(float dist)
	{

	}

	protected float DistanceToPlayer()
		=> Vector2.Distance(transform.position, GameController.GetPlayerPosition());

	protected Vector2 GetPosition() => transform.position;

	private IEnumerator Damaged(Vector2 dir)
	{
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			rb.velocity = dir;
			dir *= 0.5f;
			bool flash = (timer * 5f) % 2f < 1f;
			sprRend.color = flash ? Color.red : Color.white;
			yield return null;
		}

		sprRend.color = Color.white;
		canMove = true;
	}

	private IEnumerator Stop()
	{
		canMove = false;
		float timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		canMove = true;
	}
}
