using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
	public const float WIDTH = 1f, HEIGHT = 1f;
	private SpriteRenderer sprRend;
	private SpriteRenderer SprRend { get { return sprRend ?? (sprRend = GetComponent<SpriteRenderer>()); } }

	protected void Awake()
	{
		transform.localScale = new Vector3(WIDTH, HEIGHT, 1f);
	}

	public void SetPosition(Vector2 position)
	{
		transform.localPosition = position;
	}
}
