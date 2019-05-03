using System.Collections;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private Rigidbody2D rb;
	private Vector2 velocity;
	[SerializeField] private float speedMultiplier = 1f;
	[SerializeField] private Animator anim;
	[SerializeField] private SpriteRenderer sprRend;
	[SerializeField] private Collider2D attackBox;
	[SerializeField] private SpriteRenderer interactIcon;
	private Vector2 lastmoveDirection = Vector2.right;

	private bool canMove = true;
	private bool canAttack = true;
	private bool isInvulnerable = false;
	private int baseDamage = 10;

	private void Start()
	{
		attackBox.enabled = false;
		ActivateInteractIcon(false);
	}

	private void Update()
	{
		anim.speed = GameController.playerMovementMultiplier;

		if (canMove && Time.timeScale != 0f)
		{
			Vector2 acceleration = GetMovementInput();
			if (acceleration != Vector2.zero)
			{
				lastmoveDirection = acceleration.normalized;
			}
			anim.SetBool("Running", acceleration != Vector2.zero);
			rb.velocity = acceleration * speedMultiplier * GameController.playerMovementMultiplier;
			
			if (Input.GetKey(KeyCode.A))
			{
				sprRend.flipX = true;
			}
			if (Input.GetKey(KeyCode.D))
			{
				sprRend.flipX = false;
			}
		}
		else
		{
			rb.velocity = Vector2.zero;
		}

		if (Input.GetMouseButtonDown(0) && canAttack && !GameController.IsPaused && !ShopUI.IsActive)
		{
			anim.SetTrigger("Attack");
			StopMovement();
		}
	}

	private Vector2 GetMovementInput()
	{
		Vector2 accel = Vector2.zero;
		if (Input.GetKey(KeyCode.W))
		{
			accel.y += 1f;
		}
		if (Input.GetKey(KeyCode.A))
		{
			accel.x -= 1f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			accel.y -= 1f;
		}
		if (Input.GetKey(KeyCode.D))
		{
			accel.x += 1f;
		}
		return accel;
	}

	public void AllowMovement()
	{
		canMove = true;
		canAttack = true;
	}

	public void StopMovement()
	{
		canMove = false;
		canAttack = false;
	}

	public float GetDamage() => baseDamage;

	public void ActivateAttackBox()
	{
		attackBox.transform.localPosition = lastmoveDirection * 0.5f;
		attackBox.enabled = true;
	}

	public void DeactivateAttackBox() => attackBox.enabled = false;

	public void FinishAttack()
	{
		AllowMovement();
		DeactivateAttackBox();
	}

	public void DealDamage(float damage, Vector2 dir)
	{
		canMove = false;
		canAttack = false;
		isInvulnerable = true;
		GameController.AddTime(-damage);
		StartCoroutine(Damaged(-dir * 100f));
	}

	private IEnumerator Damaged(Vector2 dir)
	{
		float timer = 0f;
		float time = 0.4f;
		while (timer < time)
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
		canAttack = true;
		StartCoroutine(Counter(1f, () => isInvulnerable = false));
	}

	private IEnumerator Counter(float time, Action action)
	{
		float timer = 0f;
		while (timer < time)
		{
			timer += Time.deltaTime;
			yield return null;
		}
		action?.Invoke();
	}

	public void ActivateInteractIcon(bool activate)
	{
		interactIcon.enabled = activate;
	}
}
