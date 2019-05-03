using UnityEngine;

public class Eye : Enemy
{
	private void Start()
	{
		baseHp = (GameController.difficulty - 5) * 10;
		hp = baseHp;
	}

	protected override void ReactToPlayerPresence(float dist)
	{
		if (!canMove) return;
		Vector2 playerPos = GameController.GetPlayerPosition();
		Vector2 dirToPlayer = playerPos - GetPosition();
		rb.velocity = dirToPlayer.normalized * speedMultiplier;
		sprRend.flipX = rb.velocity.x < 0f;
	}

	public override void Die()
	{
		base.Die();
		anim.SetTrigger("Death");
	}
}
