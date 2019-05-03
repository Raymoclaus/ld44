using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LevelCounter : MonoBehaviour
{
	private Text t;

	private void Awake()
	{
		t = GetComponent<Text>();
	}

	void Update()
    {
		t.text = $"Level: {GameController.difficulty - 7}";

	}
}
