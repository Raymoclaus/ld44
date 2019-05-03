using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ShowHighscore : MonoBehaviour
{
	private void Start()
	{
		int highScore = PlayerPrefs.GetInt("Highscore", 0);
		string s = $"High Score: {highScore}";
		GetComponent<Text>().text = s;
	}
}
