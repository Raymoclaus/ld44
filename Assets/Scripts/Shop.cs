using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shop : MonoBehaviour
{
	private SpriteRenderer sprRend;
	private bool playerNearby = false;
	[SerializeField] private List<Sprite> sprites;

	private void Awake()
	{
		sprRend = GetComponent<SpriteRenderer>();
		sprRend.sprite = sprites[Random.Range(0, sprites.Count)];
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			playerNearby = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			playerNearby = false;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E) && !ShopUI.IsTransitioning)
		{
			ShopUI.Activate();
		}
	}
}
