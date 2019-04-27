using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
	[SerializeField] private Rigidbody2D rb;
	private Vector2 velocity;
	[SerializeField] private float speedMultiplier = 1f;

	private void Update()
	{
		Vector2 acceleration = GetMovementInput();
		rb.velocity = acceleration * speedMultiplier;
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
}
