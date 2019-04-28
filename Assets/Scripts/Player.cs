using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private Rigidbody2D rb;
	private Vector2 velocity;
	[SerializeField] private float speedMultiplier = 1f;
	[SerializeField] private Animator anim;
	[SerializeField] private SpriteRenderer sprRend;

	private bool canMove = true;
	private bool canAttack = true;
	private int baseDamage = 10;

	private void Update()
	{
		anim.speed = GameController.playerMovementMultiplier;

		if (Input.GetMouseButtonDown(0) && canAttack)
		{
			anim.SetTrigger("Attack");
			StopMovement();
		}

		if (canMove && Time.timeScale != 0f)
		{
			Vector2 acceleration = GetMovementInput();
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
}
