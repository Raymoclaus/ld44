using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class ShopUI : MonoBehaviour
{
	private static ShopUI instance;
	private static ShopUI Instance
	{
		get { return instance ?? (instance = FindObjectOfType<ShopUI>()); }
	}

	public static bool IsActive { get { return Instance.activated; } }
	public static bool IsTransitioning { get; set; }

	private CanvasGroup cGroup;
	private CanvasGroup CGroup
	{
		get { return cGroup ?? (cGroup = GetComponent<CanvasGroup>()); }
	}

	[SerializeField] private List<ShopItem> shopItems;
	[SerializeField] private float slideInDuration = 1f;
	private bool activated = false;

	private void Awake()
	{
		if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		CGroup.alpha = 0f;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void Update()
	{
		if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
			&& !IsTransitioning && activated)
		{
			Deactivate();
		}
	}

	public static void Activate()
	{
		Instance.StartCoroutine(Instance.SlideIn(true));
		GameController.ActivateEscToPauseCanvas(false);
	}

	private void Deactivate()
	{
		Instance.StartCoroutine(Instance.SlideIn(false));
		GameController.ActivateEscToPauseCanvas(true);
	}

	private IEnumerator SlideIn(bool enter)
	{
		IsTransitioning = true;
		if (enter)
		{
			Time.timeScale = 0f;
			Instance.activated = true;
		}

		float timer = 0f;
		while (timer < slideInDuration)
		{
			timer += Time.unscaledDeltaTime;
			float delta = timer / slideInDuration;
			delta = enter ? delta : 1f - delta;
			CGroup.alpha = delta;
			for (int i = 0; i < shopItems.Count; i++)
			{
				shopItems[i].Move(delta);
			}
			yield return null;
		}
		
		if (!enter)
		{
			Time.timeScale = 1f;
			activated = false;
		}
		IsTransitioning = false;
	}
}
