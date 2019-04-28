using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
	public const float WIDTH = 1f, HEIGHT = 1f;
	private SpriteRenderer sprRend;
	private SpriteRenderer SprRend
	{
		get { return sprRend ?? (sprRend = GetComponent<SpriteRenderer>()); }
	}
	[SerializeField] private List<Sprite> sprites;

	protected void Awake()
	{
		transform.localScale = new Vector3(WIDTH, HEIGHT, 1f);
		PickRandomSprite();
	}

	private void PickRandomSprite()
	{
		int random = Random.Range(0, sprites.Count);
		SprRend.sprite = sprites.Count == 0 ? SprRend.sprite : sprites[random];
	}

	public void SetPosition(Vector2 position)
	{
		transform.localPosition = position;
	}
}
