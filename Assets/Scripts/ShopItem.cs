using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class ShopItem : MonoBehaviour
{
	private RectTransform rect;
	private RectTransform Rect
	{
		get { return rect ?? (rect = GetComponent<RectTransform>()); }
	}

	[SerializeField] private TextMeshProUGUI descriptionTextMesh;
	[TextArea(1, 2)] [SerializeField] private string description;
	public int abilityID;

	[SerializeField] private Vector2 endPos;
	private Vector2 startPos;

	[SerializeField] private AnimationCurve curve;

	private void Awake()
	{
		startPos = Rect.anchoredPosition;
	}

	public void Move(float delta)
	{
		UpdateDescription();
		delta = curve?.Evaluate(delta) ?? delta;
		Rect.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, delta);
	}

	private void UpdateDescription()
	{
		int cost = GameController.difficulty - 3;
		descriptionTextMesh.text = string.Format(description, cost, 2 * cost, 3 * cost);
		gameObject.SetActive(false);
		gameObject.SetActive(true);
	}

	public void Activate()
	{
		int x = GameController.difficulty - 3;
		switch (abilityID)
		{
			case 0:
				if (GameController.GetTime() <= 2 * x) return;
				GameController.startTimer += x;
				GameController.AddTime(-2 * x);
				break;
			case 1:
				if (GameController.startTimer <= x) return;
				GameController.startTimer -= x;
				GameController.AddTime(x);
				break;
			case 2:
				if (GameController.startTimer <= 2 * x) return;
				GameController.playerDamageMultiplier *= 3f;
				GameController.startTimer -= 2 * x;
				break;
			case 3:
				GameController.playerDamageMultiplier *= 0.5f;
				GameController.AddTime(2 * x);
				break;
			case 4:
				GameController.playerMovementMultiplier *= 1.1f;
				GameController.MultiplyTime(0.5f);
				break;
			case 5:
				if (GameController.startTimer <= 60f) return;
				GameController.TeleportPlayerToLadderRoom();
				GameController.startTimer -= 60f;
				break;
		}
	}
}
