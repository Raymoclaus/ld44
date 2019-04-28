using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : Enemy
{
	[SerializeField] private Animator anim;
	[SerializeField] private int timerRewardOnHit = 1;

	private void Start()
	{
		baseHp = (GameController.difficulty - 5) * 10;
		hp = baseHp;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Attack"))
		{
			TakeDamage(GameController.GetDamage());
		}
	}

	public void TakeDamage(float damage)
	{
		hp -= damage;
		if (hp <= 0f)
		{
			GameController.AddTime(timerRewardOnHit);
			Die();
		}
	}

	private void Die()
	{
		anim.SetTrigger("Die");
	}

	public void Dead()
	{
		Destroy(gameObject);
	}

	protected override void ReactToPlayerPresence(float dist)
	{
		Vector2 playerPos = GameController.GetPlayerPosition();
		Vector2 dirToPlayer = playerPos - GetPosition();
		rb.velocity = dirToPlayer.normalized * speedMultiplier;
		sprRend.flipX = rb.velocity.x < 0f;
	}
}
