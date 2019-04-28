using System;
using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
	private float maxTime = 1f;
	private float currentTime = 1f;
	private Action<float> deltaAction;
	private Action finishAction;
	private bool active = false;

	public void StartTimer(float maxTime, float currentTime,
		Action<float> deltaAction, Action finishAction)
	{
		active = true;
		this.maxTime = maxTime;
		this.currentTime = currentTime;
		this.deltaAction = deltaAction;
		this.finishAction = finishAction;
	}

	public float GetTime() => currentTime;

	public void AddTime(float time)
	{
		currentTime += time;
	}

	public void MultiplyTime(float multiply)
	{
		currentTime *= multiply;
	}

	private void Update()
	{
		if (active)
		{
			currentTime -= Time.deltaTime;
			float delta = 1f - (currentTime / maxTime);
			deltaAction?.Invoke(delta);
			if (currentTime <= 0f)
			{
				active = false;
				finishAction?.Invoke();
			}
		}
	}
}
