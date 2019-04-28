using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Clock : MonoBehaviour
{
	[SerializeField] private List<Sprite> clockImages;
	private Image img;
	private Image Img { get { return img ?? (img = GetComponent<Image>()); } }
	[SerializeField] private Text textUI;
	private float delta = 0f;

	public void SetDelta(float value)
	{
		delta = Mathf.Clamp01(value);
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		int choose = Mathf.Clamp((int)(delta * clockImages.Count), 0, clockImages.Count - 1);
		Img.sprite = clockImages.Count == 0 ? Img.sprite : clockImages[choose];
	}

	public void UpdateTextCounter(float currentTime, float maxTime)
	{
		if (textUI != null)
		{
			textUI.text = $"{(int)currentTime}/{(int)maxTime}s";
		}
	}
}
